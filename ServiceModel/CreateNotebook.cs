using ServiceStack;

namespace Apps.ServiceModel
{
    [Route("/notebook/{Slug}/{Request}")]
    public class CreateNotebook
    {
        /// <summary>
        /// Site BaseUrl Slug
        /// </summary>
        public string Slug { get; set; }
 
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