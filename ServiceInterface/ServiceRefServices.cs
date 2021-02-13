using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Text;

namespace Apps.ServiceInterface
{
    [Route("/gists")]
    [Route("/gists/{Slug}/{Lang}")]
    [Route("/gists/{Slug}/{Lang}/{Request}")]
    public class GistRef
    {
        public string Slug { get; set; }
        public string Lang { get; set; }
        public string Request { get; set; }
    }

    public class LangInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string LineComment { get; set; } = "//";
        public Dictionary<string, string> Files { get; set; } = new();
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

        private static LangInfo CSharp = new() {
            Code = "csharp",
            Name = "C#",
            Ext = "cs",
            Files = {
                ["MyApp.csproj"] = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <NoWarn>1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""ServiceStack.Client"" Version=""5.*"" />
  </ItemGroup>

</Project>",
                ["Program.cs"] = @"using MyApp;
using ServiceStack;
using ServiceStack.Text;

var client = new JsonServiceClient({BASE_URL});

{API_COMMENT}var response = client.Get(new {REQUEST} {
{API_COMMENT}});
{API_COMMENT}response.PrintDump();
"
            }
        };
        private static LangInfo TypeScript = new() {
            Code = "typescript",
            Name = "TypeScript",
            Ext = "ts",
        };
        private static LangInfo Swift = new() {
            Code = "swift",
            Name = "Swift",
            Ext = "swift",
        };
        private static LangInfo Java = new() {
            Code = "java",
            Name = "Java",
            Ext = "java",
        };
        private static LangInfo Kotlin = new() {
            Code = "kotlin",
            Name = "Kotlin",
            Ext = "kt",
        };
        private static LangInfo Dart = new() {
            Code = "dart",
            Name = "Dart",
            Ext = "dart",
        };
        private static LangInfo FSharp = new() {
            Code = "fsharp",
            Name = "F#",
            Ext = "fs",
        };
        private static LangInfo VbNet = new() {
            Code = "vbnet",
            Name = "VB.NET",
            Ext = "vb",
            LineComment = "'",
        };
        
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

            var requestDto = string.IsNullOrEmpty(request.Request)
                ? null
                : request.Request;

            var baseUrl = request.Slug;
            if (baseUrl.IndexOf("://", StringComparison.Ordinal) == -1)
            {
                if (baseUrl.StartsWith("http.") || baseUrl.StartsWith("https."))
                    baseUrl = baseUrl.LeftPart('.') + "://" + baseUrl.RightPart('.');
                else
                    baseUrl = "https://" + baseUrl;
            }

            var key = $"{nameof(GistRef)}:{baseUrl}:{lang.Code}:{requestDto}.gist";
            var gist = await CacheAsync.GetOrCreateAsync(key, TimeSpan.FromMinutes(10), async () => {
                var site = await Sites.GetSiteAsync(request.Slug);
                var langInfo = await site.Languages.GetLanguageInfoAsync(request.Lang);
                if (requestDto != null)
                    langInfo = await langInfo.ForRequestAsync(requestDto);
                var langTypesContent = langInfo.Content;
                
                var baseUrlTitle = baseUrl.RightPart("://").LeftPart("/");
                var files = new Dictionary<string, GistFile>();
                lang.Files.Each((k, v) => {
                    var file = new GistFile {
                        Filename = k,
                        Content = v
                            .Replace("{BASE_URL}", '"' + baseUrl + '"')
                            .Replace("{REQUEST}", requestDto ?? "MyRequest")
                            .Replace("{API_COMMENT}", request.Request != null ? "" : lang.LineComment),
                        Type = MimeTypes.PlainText,
                        Raw_Url = new GistRefFile { Slug = request.Slug, Lang = lang.Code, File = k }.ToAbsoluteUri(Request),
                    };
                    file.Size = file.Content.Length;
                    files[k] = file;
                });
                var dtoFileName = $"dtos.{lang.Ext}";
                files[dtoFileName] = new GistFile {
                    Filename = dtoFileName,
                    Content = langTypesContent,
                    Size = langTypesContent.Length,
                    Type = MimeTypes.PlainText,
                    Raw_Url = langInfo.Url,
                };
                var to = new GithubGist {
                    Description = $"{baseUrlTitle} {lang.Name} API",
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