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

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

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
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = WebRequestMethods.Http.Get;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());

            String responseString = reader.ReadToEnd();

            response.Close();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseString);

            String desc = (from x in doc.DocumentNode.Descendants()
                           where x.Name.ToLower() == "meta"
                           && x.Attributes["name"] != null
                           && x.Attributes["name"].Value.ToLower() == "description"
                           select x.Attributes["content"].Value).FirstOrDefault();

            OpenGraph graph = OpenGraph.ParseUrl(url);

            HtmlData data = new HtmlData();
            data.title = graph.Title;
            data.description = desc;
            data.images = graph.Image;
            data.url = new System.Uri(url);

            string stringjson = JsonConvert.SerializeObject(data);
            return stringjson;
        }


    }
}
