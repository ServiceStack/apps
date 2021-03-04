using System.Collections.Generic;
using ServiceStack;

namespace Apps.ServiceModel
{
    [Route("/sites")]
    [Route("/sites/{Slug}/languages")]
    public class GetSiteInfoLanguages : IReturn<GetSiteInfoLanguagesResponse>
    {
        public string BaseUrl { get; set; }
        public string Slug { get; set; }
    }
    public class GetSiteInfoLanguagesResponse
    {
        public string BaseUrl { get; set; }
        public string Slug { get; set; }
        public Dictionary<string,string> Languages { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    [Route("/sites/{Slug}/languages/{Lang}")]
    public class GetSiteInfoLanguage : IReturn<GetSiteInfoLanguagesResponse>
    {
        public string Slug { get; set; }
        public string Lang { get; set; }
    }
}