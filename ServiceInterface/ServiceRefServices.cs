using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.IO;
using ServiceStack.Text;
using Apps.ServiceInterface.Langs;

namespace Apps.ServiceInterface
{
    [Route("/gists")]
    [Route("/gists/{Slug}/{Lang}")]
    [Route("/gists/{Slug}/{Lang}/{IncludeTypes}")]
    public class GistRef
    {
        public string Slug { get; set; }
        public string Lang { get; set; }
        public string IncludeTypes { get; set; }
        public bool? NoCache { get; set; }
    }

    [Route("/gists/files/{Slug}/{Lang}/{File}")]
    public class GistRefFile
    {
        public string Slug { get; set; }
        public string Lang { get; set; }
        public string File { get; set; }
    }

    public class ServiceRefServices : Service
    {
        public Sites Sites { get; set; }

        private static LangInfo CSharp = new CSharpLangInfo();
        private static LangInfo TypeScript = new TypeScriptLangInfo();
        private static LangInfo Swift = new SwiftLangInfo();
        private static LangInfo Java = new JavaLangInfo();
        private static LangInfo Kotlin = new KotlinLangInfo();
        private static LangInfo Dart = new DartLangInfo();
        private static LangInfo FSharp = new FSharpLangInfo();
        private static LangInfo VbNet = new VbNetLangInfo();
        
        private static Dictionary<string, LangInfo> LangAliases { get; set; } = new() {
            ["csharp"] = CSharp,
            ["cs"] = CSharp,
            ["typescript"] = TypeScript,
            ["ts"] = TypeScript,
            ["swift"] = Swift,
            ["sw"] = Swift,
            ["java"] = Java,
            ["ja"] = Java,
            ["kotlin"] = Kotlin,
            ["kt"] = Kotlin,
            ["dart"] = Dart,
            ["da"] = Dart,
            ["fsharp"] = FSharp,
            ["fs"] = FSharp,
            ["vbnet"] = VbNet,
            ["vb"] = VbNet,
        };
        
        public async Task<object> Get(GistRef request)
        {
            if (string.IsNullOrEmpty(request.Slug))
                throw new ArgumentNullException(nameof(request.Slug));
            if (string.IsNullOrEmpty(request.Lang))
                throw new ArgumentNullException(nameof(request.Lang));
            if (!LangAliases.TryGetValue(request.Lang, out var lang))
                throw UnknownLanguageError();
            
            var includeTypes = string.IsNullOrEmpty(request.IncludeTypes)
                ? null
                : request.IncludeTypes;

            var requestDto = includeTypes;
            Dictionary<string, string> args = null;
            if (includeTypes != null && includeTypes.IndexOf('(') >= 0)
            {
                var kvps = includeTypes.RightPart('(');
                kvps = '{' + kvps.Substring(0, kvps.Length - 1) + '}';
                args = kvps.FromJsv<Dictionary<string, string>>();
                includeTypes = includeTypes.LeftPart('(');
                requestDto = includeTypes.LastRightPart(',');
                
                //If any includeTypes were given (e.g. tag) use that instead of just Request DTO:
                if (includeTypes.IndexOf(',') >= 0)
                {
                    includeTypes = includeTypes.LastLeftPart(',');
                    //Replace URL-friendly brackets with braces
                    includeTypes = includeTypes.Replace('[', '{').Replace(']', '}'); 
                    //Treat '*' as All DTOs, i.e. don't limit included DTO Types 
                    if (includeTypes == "*")
                        includeTypes = null;
                }
                else if (!includeTypes.EndsWith(".*"))
                {
                    includeTypes += ".*";
                }
            }

            var baseUrl = request.Slug;
            if (baseUrl.IndexOf("://", StringComparison.Ordinal) == -1)
            {
                if (baseUrl.StartsWith("http.") || baseUrl.StartsWith("https."))
                    baseUrl = baseUrl.LeftPart('.') + "://" + baseUrl.RightPart('.');
                else
                    baseUrl = "https://" + baseUrl;
            }

            var key = $"{nameof(GistRef)}:{baseUrl}:{lang.Code}:{request.IncludeTypes??"*"}.gist";
            if (request.NoCache == true)
                await CacheAsync.RemoveAsync(key);
            var gist = await CacheAsync.GetOrCreateAsync(key, TimeSpan.FromMinutes(10), async () => {
                var site = await Sites.GetSiteAsync(request.Slug);
                var langInfo = await site.Languages.GetLanguageInfoAsync(request.Lang);
                var baseUrlTitle = baseUrl.RightPart("://").LeftPart("/");
                if (includeTypes != null)
                {
                    baseUrlTitle += $" {requestDto}";
                    langInfo = await langInfo.ForRequestAsync(includeTypes);
                }
                var langTypesContent = langInfo.Content;
                
                var files = new Dictionary<string, GistFile>();
                var description = $"{baseUrlTitle} {lang.Name} API";
                var requestOp = site.Metadata.Api.Operations.FirstOrDefault(x => x.Request.Name == requestDto);
                var authTemplate = requestOp?.RequiresAuth == true
                    ? lang.RequiresAuthTemplate
                    : "";
                lang.Files.Each((string k, string v) => {
                    var content = v
                        .Replace("{BASE_URL}", baseUrl)
                        .Replace("{REQUEST}", requestDto ?? "MyRequest")
                        .Replace("{RESPONSE}", lang.GetResponse(requestOp))
                        .Replace("{API_COMMENT}", request.IncludeTypes != null ? "" : lang.LineComment)
                        .Replace("{REQUIRES_AUTH}", authTemplate)
                        .Replace("{DESCRIPTION}",description)
                        .Replace("{INSPECT_VARS}", requestDto != null ? lang.InspectVarsResponse : null);

                    var textCase = site.Metadata.App.JsTextCase != null
                        ? (TextCase)Enum.Parse(typeof(TextCase), site.Metadata.App.JsTextCase, ignoreCase:true)
                        : TextCase.CamelCase;
                    using var jsScope = JsConfig.With(new Config { TextCase = textCase });
                    {
                        content = args != null
                            ? content.Replace("{REQUEST_BODY}", lang.RequestBody(requestDto, args, site.Metadata.Api))
                            : content.Replace("{REQUEST_BODY}", "");
                    }

                    var file = new GistFile {
                        Filename = k,
                        Content = content,
                        Type = MimeTypes.PlainText,
                        Raw_Url = new GistRefFile { Slug = request.Slug, Lang = lang.Code, File = k }.ToAbsoluteUri(Request),
                    };
                    file.Size = file.Content.Length;
                    files[k] = file;
                });

                var langFiles = VirtualFiles.GetDirectory($"files/{lang.Code}");
                if (langFiles != null)
                {
                    foreach (var file in langFiles.GetAllFiles())
                    {
                        var content = file.ReadAllText();
                        lang.Files[file.Name] = content;
                    }
                }
                
                var dtoFileName = $"{lang.DtosPathPrefix}dtos.{lang.Ext}";
                files[dtoFileName] = new GistFile {
                    Filename = dtoFileName,
                    Content = langTypesContent,
                    Size = langTypesContent.Length,
                    Type = MimeTypes.PlainText,
                    Raw_Url = langInfo.Url,
                };
                var to = new GithubGist {
                    Description = description,
                    Created_At = DateTime.UtcNow,
                    Files = files,
                    Public = true,
                    Url = Request.AbsoluteUri,
                    Owner = new GithubUser {
                        Id = 76883648,
                        Login = "gistcafe",
                        Avatar_Url = "https://avatars2.githubusercontent.com/u/76883648?v=4",
                        Url = "https://api.github.com/users/gistcafe",
                        Html_Url = "https://github.com/gistcafe",
                        Type = "User"
                    }
                };
                var hashCode = new HashCode();
                hashCode.Add(to.Description);
                files.Each(entry => {
                    hashCode.Add(entry.Key);
                    hashCode.Add(entry.Value);
                });
                // to.Id = $"{Math.Abs(hashCode.ToHashCode())}";
                var scheme = Request.AbsoluteUri.LeftPart("://");
                to.Id = scheme == "http" 
                    ? "http." + Request.AbsoluteUri.RightPart("://")
                    : Request.AbsoluteUri.RightPart("://");
                return to;
            });
            return new HttpResult(gist) {
                ContentType = MimeTypes.Json,
                ResultScope = () => JsConfig.With(new Config { DateHandler = DateHandler.ISO8601DateTime })
            };
        }

        public object Get(GistRefFile request)
        {
            if (string.IsNullOrEmpty(request.Lang))
                throw new ArgumentNullException(nameof(request.Lang));
            if (!LangAliases.TryGetValue(request.Lang, out var lang))
                throw UnknownLanguageError();

            if (!lang.Files.TryGetValue(request.File, out var file))
                throw HttpError.NotFound("File was not found");

            Response.ContentType = MimeTypes.PlainText;
            return file;
        }

        private static ArgumentException UnknownLanguageError() => 
            new("Unknown Language, choose from: csharp, typescript, swift, java, kotlin, dart, fsharp or vbnet", nameof(GistRefFile.Lang));
    }
}