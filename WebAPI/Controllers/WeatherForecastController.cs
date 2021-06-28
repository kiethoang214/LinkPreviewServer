using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebAPI;
using Newtonsoft.Json;
using OpenGraphNet;

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

        [HttpGet]
        public async Task<string> getWebContent(String link)
        {
            return getWeb(link);
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
            var getHtmlDoc = new HtmlWeb();
            var document = getHtmlDoc.Load(url);
            var metaTags = document.DocumentNode.SelectNodes("//meta");
            HtmlData data = new HtmlData();
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
            string stringjson = JsonConvert.SerializeObject(data);
            return stringjson;
        }
    }
}
