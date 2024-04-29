using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sympli.Seo.Application.Constants;
using Sympli.Seo.Application.Contracts;
using System.Web;

namespace Sympli.Seo.Application.SearchServices
{
    public class GoogleSearchService(
        IHtmlFetcher htmlFetcher, 
        IMemoryCache memoryCache, 
        IOptions<SeoOptions> options,
        ILogger<BaseSearchService> logger) : BaseSearchService(htmlFetcher, memoryCache, options, logger)
    {
        protected override string Xpath => "//div[@class = 'sCuL3']/div";
        protected override Provider SearchProvider => Provider.Google;

        protected override string BuildSearchUrl(string keywords, int offset = 1) 
            => $"https://www.google.com.au/search?q={HttpUtility.UrlEncode(keywords)}&num={Options.MaxCount}&start={offset}&t={DateTime.Now.Ticks}";
    }
}
