using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend_App.Models;
using Backend_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend_App.Controllers
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
        private readonly VehicleService _vehicleService;
        private readonly SensorsService _sensorService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, VehicleService vehicleService, SensorsService sensorsService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
            _sensorService = sensorsService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
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
