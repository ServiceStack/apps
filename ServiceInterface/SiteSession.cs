using Apps.ServiceModel;
using ServiceStack;
using ServiceStack.Web;

namespace Apps.ServiceInterface
{
    public class SiteSession
    {
        public string Slug { get; set; }
        public string SessionId { get; set; }
        public string BearerToken { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AuthSecret { get; set; }
        public AuthenticateResponse User { get; set; } 

        public static string GetKey(IRequest req, string slug)
        {
            return $"urn:sitesession:{slug}:{req.GetSessionId()}";
        }

        public static SiteSession Create(SiteAuthenticate request, AuthenticateResponse user)
        {
            var to = new SiteSession {
                Slug = request.Slug,
                User = user,
            };
            if (user != null && request.provider != "authsecret")
            {
                if (!string.IsNullOrEmpty(user.BearerToken))
                    to.BearerToken = user.BearerToken;
                else if (!string.IsNullOrEmpty(user.SessionId))
                    to.SessionId = user.SessionId;
            }
            if (request.AccessToken != null)
            {
                if (request.provider == "bearer")
                    to.BearerToken = request.AccessToken;
                else if (request.provider == "session")
                    to.SessionId = request.AccessToken;
                else if (request.provider == "authsecret")
                    to.AuthSecret = request.AccessToken;
            }
            return to;
        }
    }
}