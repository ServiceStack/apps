using ServiceStack;

namespace Apps.ServiceModel
{
    [Route("/notebook")]
    [Route("/notebook/{Lang}/{Slug}/{Request}")]
    public class CreateNotebook
    {
        /// <summary>
        /// Language code of notebook to create (default python)
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Site BaseUrl Slug
        /// </summary>
        public string Slug { get; set; }
 
        /// <summary>
        /// The Type Pattern to include (if any)
        /// </summary>
        public string IncludeTypes { get; set; }
 
        /// <summary>
        /// Request DTO name
        /// </summary>
        public string Request { get; set; }
 
        /// <summary>
        /// Any Request DTO Args in JS Object Literal Format
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// Save notebook as {Name}.ipynb
        /// </summary>
        public string Name { get; set; }
    }
}