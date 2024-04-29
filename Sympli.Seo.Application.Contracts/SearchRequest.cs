namespace Sympli.Seo.Application.Contracts
{
    public class SearchRequest
    {
        public Provider Provider { get; set; }
        public string Keywords { get; set; } = "e-settlements";
        public string Url { get; set; } = "www.sympli.com.au";
    }
}
