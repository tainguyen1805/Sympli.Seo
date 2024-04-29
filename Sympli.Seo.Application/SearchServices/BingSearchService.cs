using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sympli.Seo.Application.Constants;
using Sympli.Seo.Application.Contracts;
using System.Web;

namespace Sympli.Seo.Application.SearchServices
{
    public class BingSearchService(
        IHtmlFetcher htmlFetcher, 
        IMemoryCache memoryCache, 
        IOptions<SeoOptions> seoOptions, 
        ILogger<BaseSearchService> logger) : BaseSearchService(htmlFetcher, memoryCache, seoOptions, logger)
    {
        //Bing only allow maximum of 10 records - when input 50, always get no results
        private const short MaxResult = 50;
        protected override string Xpath => "//ol[@id='b_results']/li";
        protected override Provider SearchProvider => Provider.Bing;
        protected override async Task<string> FetchHtmlAsync(string keywords, int offset)
        {
            var url = BuildSearchUrl(keywords, offset);
            return await Fetcher.FetchFrom(url, FetchMethod.WebDriver, "//li[@class='b_pag']");
        }

        protected override string BuildSearchUrl(string keywords, int offset = 1)
            => $"https://www.bing.com/search?q={HttpUtility.UrlEncode(keywords)}&count={MaxResult}&first={offset}&t={DateTime.Now.Ticks}&rdrig=ServerRedirect";
    }
}
