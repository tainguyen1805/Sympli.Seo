using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sympli.Seo.Application.Constants;
using Sympli.Seo.Application.Contracts;
using Sympli.Seo.Application.SearchServices;

namespace Sympli.Seo.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterSearchServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHtmlFetcher, HtmlFetcher>();
            services.AddKeyedScoped<ISearchService, GoogleSearchService>(Provider.Google);
            services.AddKeyedScoped<ISearchService, BingSearchService>(Provider.Bing);

            services.Configure<SeoOptions>(configuration.GetSection("Seo"));

            services.AddMemoryCache();
        }
    }
}
