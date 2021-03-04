using ServiceStack;

namespace Apps.ServiceModel
{
    public class AddSite
    {
        public string BaseUrl { get; set; }
    }

    [Route("/sites/{Slug}/meta")]
    public class SiteMeta : IReturn<SiteMetaResponse>
    {
        public string Slug { get; set; }
        public bool? NoCache { get; set; }
    }

    public class SiteMetaResponse
    {
        public MetadataTypes Api { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}