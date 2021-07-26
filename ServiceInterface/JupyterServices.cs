using System;
using System.Threading.Tasks;
using Apps.ServiceModel;
using ServiceStack;
using ServiceStack.Text;

namespace Apps.ServiceInterface
{
    public class JupyterServices : Service
    {
        public Sites Sites { get; set; }

        public async Task<object> Any(CreateNotebook request)
        {
            if (string.IsNullOrEmpty(request.Lang))
                throw new ArgumentNullException(nameof(request.Lang));
            if (string.IsNullOrEmpty(request.Slug))
                throw new ArgumentNullException(nameof(request.Slug));

            var baseUrl = request.Slug;
            if (baseUrl.IndexOf("://", StringComparison.Ordinal) == -1)
            {
                if (baseUrl.StartsWith("http.") || baseUrl.StartsWith("https."))
                    baseUrl = baseUrl.LeftPart('.') + "://" + baseUrl.RightPart('.');
                else
                    baseUrl = "https://" + baseUrl;
            }

            var requestDto = request.Request;
            var lang = LangInfoUtils.AssertLangInfo(request.Lang);
            var notebook = await Sites.CreateNotebookAsync(lang, baseUrl, request.IncludeTypes, requestDto, request.Args);

            var suffix = requestDto ?? (request.IncludeTypes != null
                ? request.IncludeTypes.Replace("{", "").Replace("}", "").Replace(".", "").Replace("*", "").Replace(",", "-")
                : "");
            var fileName = request.Name ?? 
                           baseUrl.RightPart("://").LeftPart('/').SafeVarRef() +
                           (string.IsNullOrEmpty(suffix) ? "" : "-" + suffix);
            if (!fileName.EndsWith(".ipynb"))
                fileName += ".ipynb";
            
            var json = notebook.ToJson().IndentJson();

            return new HttpResult(json, "application/x-ipynb+json") {
                Headers = {
                    [HttpHeaders.ContentDisposition] =
                        $"attachment; {HttpExt.GetDispositionFileName(fileName)}; size={json.Length}; modification-date={DateTime.UtcNow.ToString("R").Replace(",", "")}"
                }
            };
        }
    }
}