using Microsoft.AspNetCore.Mvc;
using Sympli.Seo.Application.Contracts;
using Sympli.Seo.Application.SearchServices;

namespace Sympli.Seo.Application.HttpApi.Controllers
{
    [ApiController]
    [Route("seo")]
    public class SeoController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        public SeoController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        [HttpGet("positions")]
        public async Task<SearchResponse> SearchPositionsAsync([FromQuery]SearchRequest request)
        {
            try
            {
                var searchSerice = _serviceProvider.GetKeyedService<ISearchService>(request.Provider);

                if (searchSerice == null)
                    throw new ArgumentException("Invalid request");

                var positions = await searchSerice.SearchPositionsAsync(request.Keywords, request.Url);

                return new SearchResponse
                {
                    Positions = positions
                };
            }
            catch (Exception ex)
            {
                return new SearchResponse
                {
                    Error = ex.Message
                };
            }
        }
    }
}
