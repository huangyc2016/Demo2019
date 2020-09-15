using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace SignalrApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    //[Authorize("auth1")]
    public class WeatherForecastController : ControllerBase
    {
        private IDistributedCache _cache { get; set; }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var value = _cache.Get("name-key");
            string valStr = string.Empty;
            if (value == null)
            {
                valStr = "孙悟空三打白骨精！";
                // 存储的数据必须为字节，所以需要转换一下
                var encoded = Encoding.UTF8.GetBytes(valStr);
                // 配置类：30秒过时
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
                _cache.Set("name-key", encoded, options);
            }
            else
            {
                valStr = Encoding.UTF8.GetString(value);
            }

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
