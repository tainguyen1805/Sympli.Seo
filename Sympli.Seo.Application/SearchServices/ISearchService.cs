namespace Sympli.Seo.Application.SearchServices
{
    public interface ISearchService
    {
        Task<string> SearchPositionsAsync(string keywords, string url);
    }
}
