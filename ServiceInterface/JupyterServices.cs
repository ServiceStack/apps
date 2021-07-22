using System;
using System.Collections.Generic;
using System.Linq;
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
            if (string.IsNullOrEmpty(request.Slug))
                throw new ArgumentNullException(nameof(request.Slug));
            if (string.IsNullOrEmpty(request.Request))
                throw new ArgumentNullException(nameof(request.Request));

            var baseUrl = request.Slug;
            if (baseUrl.IndexOf("://", StringComparison.Ordinal) == -1)
            {
                if (baseUrl.StartsWith("http.") || baseUrl.StartsWith("https."))
                    baseUrl = baseUrl.LeftPart('.') + "://" + baseUrl.RightPart('.');
                else
                    baseUrl = "https://" + baseUrl;
            }

            var requestDto = request.Request;
            var lang = "python";
            var site = await Sites.GetSiteAsync(request.Slug);
            var meta = site.Metadata.Api;
            var requestOp = meta.Operations.FirstOrDefault(x => x.Request.Name == requestDto);

            if (requestOp == null)
                throw new ArgumentException($"{baseUrl} does not have an API named '{requestDto}'");
            
            var notebook = await Sites.CreateNotebookAsync(baseUrl, requestDto, request.Args);
            var fileName = request.Name ?? baseUrl.RightPart("://").LeftPart('/').SafeVarRef() +
                (requestDto != null ? "-" + requestDto.LeftPart('(') : "");
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