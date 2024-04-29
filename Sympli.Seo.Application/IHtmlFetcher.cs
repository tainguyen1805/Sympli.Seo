using Sympli.Seo.Application.Constants;

namespace Sympli.Seo.Application
{
    public interface IHtmlFetcher
    {
        Task<string> FetchFrom(string url, FetchMethod method = FetchMethod.HttpClient, string xPath = "");
    }
}
