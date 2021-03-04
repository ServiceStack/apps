using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Apps.ServiceModel;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Web;

namespace Apps.ServiceInterface
{
    public class AppServices : Service
    {
        public Sites Sites { get; set; }
        
        public async Task<object> Get(GetSiteInfoLanguages request)
        {
            if (string.IsNullOrEmpty(request.BaseUrl) && string.IsNullOrEmpty(request.Slug))
                throw new ArgumentNullException(nameof(request.BaseUrl));
            
            var slug = request.Slug ?? SiteUtils.UrlToSlug(request.BaseUrl);
            var site = await Sites.GetSiteAsync(slug);

            var languageInfos = await site.Languages.GetLanguageInfosAsync();
            var languages = new Dictionary<string, string>();
            foreach (var entry in languageInfos)
            {
                languages[entry.Key] = entry.Value.Content;
            }
            return new GetSiteInfoLanguagesResponse {
                BaseUrl = request.BaseUrl ?? SiteUtils.UrlFromSlug(slug),
                Slug = slug,
                Languages = languages,
            };
        }
        
        // /json/reply/{Request} or /csv/reply/{Request} 
        private string CreateSiteRequestUrl(SiteInfo site, string requestDtoName) =>
            site.BaseUrl.CombineWith(Request.PathInfo.LastLeftPart('/'), requestDtoName);

        public async Task Any(SiteInvoke request)
        {
            var site = await Sites.AssertSiteAsync(request.Slug);
            if (request.Request == null)
                throw new ArgumentNullException(nameof(request.Request));

            var url = CreateSiteRequestUrl(site, request.Request);
            var qs = SiteUtils.ToUrlEncoded(request.Args);
            var sendInBody = HttpUtils.HasRequestBody(Request.Verb);
            if (!string.IsNullOrEmpty(qs) && !sendInBody)
            {
                url += "?" + qs;
            }

            var webReq = CreateSiteWebRequest(site, url);
            ProxyFeatureHandler.InitWebRequest(Request as IHttpRequest, webReq);

            if (!string.IsNullOrEmpty(qs) && sendInBody)
            {
                webReq.ContentType = MimeTypes.FormUrlEncoded;
                await using var requestStream = await webReq.GetRequestStreamAsync();
                await requestStream.WriteAsync(MemoryProvider.Instance.ToUtf8(qs));
            }
            
            var proxy = new ProxyFeatureHandler();
            await proxy.ProxyToResponse((IHttpResponse) Response, webReq);
        }

        public async Task Any(SiteProxy request)
        {
            var site = await Sites.AssertSiteAsync(request.Slug);
            if (request.Request == null)
                throw new ArgumentNullException(nameof(request.Request));

            var url = CreateSiteRequestUrl(site, request.Request);
            var qs = SiteUtils.ToUrlEncoded(request.Query);
            if (!string.IsNullOrEmpty(qs))
                url += "?" + qs;

            var webReq = CreateSiteWebRequest(site, url);

            var proxy = new ProxyFeatureHandler();
            await proxy.ProxyRequestAsync((IHttpRequest) Request, webReq);
        }
        
        private HttpWebRequest CreateSiteWebRequest(SiteInfo siteInfo, string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            var siteSession = SessionBag.Get<SiteSession>();
            if (siteSession != null)
            {
                if (siteSession.BearerToken != null)
                {
                    req.AddBearerToken(siteSession.BearerToken);
                }
                else if (siteSession.SessionId != null)
                {
                    var overrideParam = "X-" + Keywords.SessionId;
                    req.Headers[overrideParam] = siteSession.SessionId;
                }
                else if (siteSession.UserName != null && siteSession.Password != null)
                {
                    req.AddBasicAuth(siteSession.UserName, siteSession.Password);
                }
                else if (siteSession.AuthSecret != null)
                {
                    var overrideParam = HttpHeaders.XParamOverridePrefix + Keywords.AuthSecret;
                    req.Headers[overrideParam] = siteSession.AuthSecret;
                }
            }
            return req;
        }
        
    }
}
