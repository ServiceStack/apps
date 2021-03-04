using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;

namespace Apps.ServiceInterface
{
    public class SiteInfo
    {
        public SiteInfo() => Languages = new Languages(this);
        public string BaseUrl { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public AppMetadata Metadata { get; set; }
        public List<string> Plugins { get; set; } 
        public List<string> Auth { get; set; } 
        public DateTime AddedDate { get; set; }
        public DateTime AccessDate { get; set; }
        public Languages Languages { get; }
    }

    public class Languages
    {
        public SiteInfo Site { get; }
        public Languages(SiteInfo site) => Site = site;

        public ConcurrentDictionary<string, LanguageInfo> Map { get; set; } = new();

        public async Task<LanguageInfo> GetLangContentAsync(string lang, string requestDto=null)
        {
            try
            {
                var langTypesUrl = Site.BaseUrl.CombineWith("types", lang);
                var useGlobalNs = lang == "csharp" || lang == "fsharp" || lang == "vbnet"; 
                if (useGlobalNs)
                    langTypesUrl += "?GlobalNamespace=MyApp";
                if (lang == "java" || lang == "kotlin")
                    langTypesUrl += "?Package=myapp";
                
                if (requestDto != null)
                    langTypesUrl += (langTypesUrl.IndexOf('?') >= 0 ? "&" : "?") + $"IncludeTypes={requestDto}.*";

                var content = await langTypesUrl
                    .GetStringFromUrlAsync(requestFilter:req => req.UserAgent = "apps.servicestack.net");
                return new LanguageInfo(this, lang, langTypesUrl, content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LanguageInfo> GetLanguageInfoAsync(string lang)
        {
            if (Map.TryGetValue(lang, out var info))
                return info;

            info = await GetLangContentAsync(lang);
            Map[lang] = info;
            return info;
        }

        public async Task<IDictionary<string, LanguageInfo>> GetLanguageInfosAsync()
        {
            var langTasks = Sites.Languages
                .Where(lang => !Map.ContainsKey(lang))
                .Map(async lang => KeyValuePair.Create(lang, await GetLangContentAsync(lang)));

            if (langTasks.Count > 0)
            {
                var results = await Task.WhenAll(langTasks);
                foreach (var result in results)
                {
                    Map[result.Key] = result.Value;
                }
            }
            return Map;
        }
    }
    
    public class LanguageInfo
    {
        public LanguageInfo(Languages languages, string code, string url, string content)
        {
            Languages = languages;
            Code = code;
            Url = url;
            Content = content;
        }

        public Languages Languages { get; }
        public string Code { get; }
        public string Url { get; }
        public string Content { get; }

        public ConcurrentDictionary<string, LanguageInfo> RequestMap { get; set; } = new();

        public async Task<LanguageInfo> ForRequestAsync(string requestDto)
        {
            if (RequestMap.TryGetValue(requestDto, out var requestLanguage))
                return requestLanguage;

            requestLanguage = await Languages.GetLangContentAsync(Code, requestDto);
            RequestMap[requestDto] = requestLanguage;
            return requestLanguage;
        }
    }


    public class Sites
    {
        public static string[] Languages = {
            "typescript",
            "csharp",
            "dart",
            "java",
            "kotlin",
            "swift",
            "vbnet",
            "fsharp",
        };

        internal ConcurrentDictionary<string, SiteInfo> Map = new(StringComparer.OrdinalIgnoreCase);

        public SiteInfo FindSite(string slug) => Map.Values.FirstOrDefault(x => x.Slug == slug);

        public void RemoveSite(string slug)
        {
            var useBaseUrl = SiteUtils.UrlFromSlug(slug);
            Map.TryRemove(useBaseUrl, out _);
        }

        public async Task<SiteInfo> GetSiteAsync(string slug)
        {
            var site = FindSite(slug);
            if (site != null)
                return site;

            var useBaseUrl = SiteUtils.UrlFromSlug(slug);
            var appMetadata = await useBaseUrl.GetAppMetadataAsync();
            
            var siteInfo = new SiteInfo {
                BaseUrl = useBaseUrl,
                Slug = slug,
                Metadata = appMetadata,
                AddedDate = DateTime.Now,
                AccessDate = DateTime.Now,
            };

            Map[useBaseUrl] = siteInfo;
            return siteInfo;
        }

        public async Task<SiteInfo> AssertSiteAsync(string slug) => string.IsNullOrEmpty(slug) 
            ? throw new ArgumentNullException(nameof(SiteInfo.Slug)) 
            : await GetSiteAsync(slug) ?? throw HttpError.NotFound("Site does not exist");
    }
}