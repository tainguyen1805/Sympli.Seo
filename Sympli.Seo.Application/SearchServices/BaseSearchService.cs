using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sympli.Seo.Application.Constants;
using Sympli.Seo.Application.Contracts;

namespace Sympli.Seo.Application.SearchServices
{
    public abstract class BaseSearchService(
        IHtmlFetcher htmlFetcher, 
        IMemoryCache memoryCache, 
        IOptions<SeoOptions> seoOptions,
        ILogger<BaseSearchService> logger) : ISearchService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        
        protected readonly IHtmlFetcher Fetcher = htmlFetcher;
        protected readonly SeoOptions Options = seoOptions.Value;
        protected readonly ILogger Logger = logger;
        protected List<HtmlNode> Elements = [];

        /// <summary>
        /// The Xpath to the search result item
        /// </summary>
        protected abstract string Xpath { get; }
        protected abstract Provider SearchProvider { get; }
        protected abstract string BuildSearchUrl(string keywords, int offset = 1);
        
        public async Task<string> SearchPositionsAsync(string keywords, string url)
        {
            Validate(keywords, url);

            var key = new { keywords, url, SearchProvider };

            if (_memoryCache.TryGetValue<string>(key, out var cachedValue))
                return cachedValue ?? string.Empty;

            await FindElementsAsync(keywords);
            string value = GetOccurrences(url);

            _memoryCache.Set(key, value, new DateTimeOffset(DateTime.Now.AddMinutes(Options.CacheExpirationInMinutes)));

            return value;
        }

        private string GetOccurrences(string url)
        {
            if (Elements.Count == 0) return string.Empty;

            var result = new List<short>();

            for (short i = 0; i < Elements.Count; i++)
            {
                if (!Elements[i].InnerText.Contains(url, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                result.Add(i);
            }

            return string.Join(", ", result);
        }

        private async Task FindElementsAsync(string keywords)
        {
            var offset = 1;
            while (Elements.Count < Options.MaxCount)
            {
                string html = await FetchHtmlAsync(keywords, offset);

                var document = new HtmlDocument();
                document.LoadHtml(html);

                var nodes = document.DocumentNode.SelectNodes(Xpath);
                var elements = nodes?.ToList() ?? new List<HtmlNode>();

                if (elements.Count == 0)
                    break;

                offset += elements.Count;

                Elements.AddRange(elements);
            }

            Elements = Elements.Slice(0, Options.MaxCount);
        }

        protected virtual async Task<string> FetchHtmlAsync(string keywords, int offset)
        {
            var url = BuildSearchUrl(keywords, offset);
            return await Fetcher.FetchFrom(url, FetchMethod.HttpClient);
        }

        private static void Validate(string keywords, string url)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                throw new ArgumentNullException(nameof(keywords));

            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("Invalid Url", nameof(url));
        }
    }
}
