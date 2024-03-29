using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.IO;
using ServiceStack.Text;
using Apps.ServiceInterface.Langs;
using Apps.ServiceModel;

namespace Apps.ServiceInterface;

public class ServiceRefServices : Service
{
    public Sites Sites { get; set; }

    public async Task<object> Get(GistRef request)
    {
        if (string.IsNullOrEmpty(request.Slug))
            throw new ArgumentNullException(nameof(request.Slug));
        if (string.IsNullOrEmpty(request.Lang))
            throw new ArgumentNullException(nameof(request.Lang));
        var lang = LangInfoUtils.AssertLangInfo(request.Lang);
            
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
            var meta = site.Metadata.Api;
            var requestOp = meta.Operations.FirstOrDefault(x => x.Request.Name == requestDto);
            var authTemplate = requestOp?.RequiresAuth == true
                ? lang.RequiresAuthTemplate
                : "";

            var types = new List<string> { requestDto ?? "MyRequest" };
            if (requestOp != null && args != null)
            {
                var props = requestOp.Request.GetFlattenedProperties(meta);
                foreach (var entry in args)
                {
                    var prop = props.FirstOrDefault(x =>
                        string.Equals(x.Name, entry.Key, StringComparison.OrdinalIgnoreCase));
                    var propType = prop?.Type != null ? meta.FindType(prop.Type, prop.Namespace) : null;
                    if (propType != null)
                        types.Add(propType.Name);
                }
            }

            lang.Files.Each((string k, string v) => {
                var content = v
                    .Replace("{BASE_URL}", baseUrl)
                    .Replace("{REQUEST}", requestDto ?? "MyRequest")
                    .Replace("{RESPONSE}", lang.GetResponse(requestOp))
                    .Replace("{TYPES}", string.Join(", ", types))
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
                        ? content.Replace("{REQUEST_BODY}", lang.RequestBody(requestDto, args, meta))
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

            var resolvedUrl = Request.IsSecureConnection
                ? "https://" + Request.AbsoluteUri.RightPart("://")
                : Request.AbsoluteUri;
            var to = new GithubGist {
                Description = description,
                Created_At = DateTime.UtcNow,
                Files = files,
                Public = true,
                Url = resolvedUrl,
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
            to.Id = resolvedUrl;
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
        var lang = LangInfoUtils.AssertLangInfo(request.Lang);

        if (!lang.Files.TryGetValue(request.File, out var file))
            throw HttpError.NotFound("File was not found");

        Response.ContentType = MimeTypes.PlainText;
        return file;
    }
}