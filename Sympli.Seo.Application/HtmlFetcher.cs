using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sympli.Seo.Application.Constants;

namespace Sympli.Seo.Application
{
    public class HtmlFetcher : IHtmlFetcher
    {
        /// <summary>
        /// Fetch Html from url
        /// </summary>
        /// <param name="url">Url to fetch from</param>
        /// <param name="method">Fetch method</param>
        /// <param name="xPath">Element to be waited for the whole page loaded. This is required if FetchMethod is WebDriver</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> FetchFrom(string url, FetchMethod method = FetchMethod.HttpClient, string xPath = "")
            => method switch
            {
                FetchMethod.HttpClient => await FetchByHttpClientAsync(url),
                FetchMethod.WebDriver => string.IsNullOrWhiteSpace(xPath)
                    ? throw new ArgumentException("Xpath is required when FetchMethod is WebDriver")
                    : await FetchByWebDriverAsync(url, xPath),
                _ => throw new ArgumentException("Unsupported fetch type"),
            };

        private static async Task<string> FetchByHttpClientAsync(string url)
        {
            using var client = new HttpClient();
            return await client.GetStringAsync(url);
        }

        private static async Task<string> FetchByWebDriverAsync(string url, string xPath)
        {
            //var edgeOptions = new EdgeOptions();
            //--headless means running the browser in background without open it up on UI
            //edgeOptions.AddArguments("--headless");

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless");

            using var driver = new ChromeDriver(chromeOptions);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl(url);

            try
            {
                driver.FindElement(By.XPath(xPath));

                //Give the browser a little bit to load the page
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                return driver.PageSource;
            }
            catch (NoSuchElementException)
            {
                throw new NotFoundException("Error loading search page");
            }
            finally
            {
                driver.GetScreenshot().SaveAsFile("screenshot.png");
                driver.Quit();
            }
        }
    }
}
