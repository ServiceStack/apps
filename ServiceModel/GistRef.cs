using ServiceStack;

namespace Apps.ServiceModel
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
}