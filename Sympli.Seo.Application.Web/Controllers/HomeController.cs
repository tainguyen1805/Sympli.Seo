using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sympli.Seo.Application.Contracts;
using Sympli.Seo.Application.Web.Models;
using System.Diagnostics;

namespace Sympli.Seo.Application.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Options _options;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<Options> options, ILogger<HomeController> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SeoSearchModel model)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetFromJsonAsync<SearchResponse>(
                        $"{_options.SeoApiUrl}?provider={model.SearchProvider}&keywords={model.Keywords}&url={model.TargetSite}");

                    ViewData["Result"] = response?.Error ?? response?.Positions ?? "";
                }
            }
            catch (Exception ex)
            {
                var message = $"Unexpected error when calling SEO API.\r\nError details: {ex.Message}";
                ViewData["Result"] = message;
                _logger.LogError(ex, message);
            }
                
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
