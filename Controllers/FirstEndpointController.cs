using System;
using asp_net_po_schedule_management_server.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route(nameof(Configuration.API_PREFIX) + nameof(Configuration.WEATHER_FORECAST_ENDPOINT))]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Action<string> Get([FromRoute] string message)
        {
            return 
        }
    }
}