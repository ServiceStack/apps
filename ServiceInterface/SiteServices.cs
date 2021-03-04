using System.Threading.Tasks;
using Apps.ServiceModel;
using ServiceStack;

namespace Apps.ServiceInterface
{
    public class SiteServices : Service
    {
        public Sites Sites { get; set; }
        
        public async Task<object> Any(SiteMeta request)
        {
            request.Slug.AssertNotEmpty(nameof(request.Slug));
            
            if (request.NoCache == true)
                Sites.RemoveSite(request.Slug);
            
            var site = await Sites.GetSiteAsync(request.Slug);
            var response = new SiteMetaResponse {
                Api = site.Metadata.Api
            };
            return response;
        }
    }
}