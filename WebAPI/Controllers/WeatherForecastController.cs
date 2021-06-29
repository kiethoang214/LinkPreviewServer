using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Web;

namespace TestAPI2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("getWebContent/{link}")]
        public String getWebContent(String link)
        {
            try {
                string newlink = HttpUtility.UrlDecode(link);
                return getWeb(newlink);
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        class HtmlData
        {
            public String title { get; set; }
            public String description { get; set; }
            public Uri images { get; set; }
            public Uri url { get; set; }
        }

        protected string getWeb(String url)
        {
            HtmlData data = new HtmlData();
            try
            {
                var getHtmlDoc = new HtmlWeb();
                var document = getHtmlDoc.Load(url);
                var metaTags = document.DocumentNode.SelectNodes("//meta");
                if (metaTags != null)
                {
                    foreach (var sitetag in metaTags)
                    {
                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["property"].Value == "og:title")
                        {
                            data.title = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["property"].Value == "og:description")
                        {
                            data.description = sitetag.Attributes["content"].Value;
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["property"].Value == "og:image")
                        {
                            data.images = new Uri(sitetag.Attributes["content"].Value, UriKind.Absolute);
                        }

                        if (sitetag.Attributes["property"] != null && sitetag.Attributes["property"].Value == "og:url")
                        {
                            data.url = new Uri(sitetag.Attributes["content"].Value, UriKind.Absolute);
                        }
                    }

                    if (data.url == null)
                    {
                        data.url = new Uri(url);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
            
            string stringjson = JsonConvert.SerializeObject(data);
            return stringjson;
        }
    }
}
