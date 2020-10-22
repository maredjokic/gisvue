using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend_App.EntityCore.Models;
using Backend_App.Models;
using Backend_App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend_App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PointController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly VehicleService _vehicleService;
        private readonly SensorsService _sensorService;

        public PointController(ILogger<WeatherForecastController> logger, VehicleService vehicleService, SensorsService sensorsService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
            _sensorService = sensorsService;
        }


        [HttpGet]
        public IEnumerable<VehiclePoint> Get(int count, int skip)
        {           
            return _vehicleService.GetVehiclePointsFirst(4573, count, skip);
        }

        [HttpGet]
        [Route("ByDate")]
        public IEnumerable<VehiclePoint> GetByDate(DateTime dateTime)
        {           
            return _vehicleService.GetVehiclePointsByDate(4573, dateTime);
        }

        [HttpGet]
        [Route("PointsByDate")]
        public IEnumerable<PointDouble> GetPointsByDate(DateTime dateTime, int vid)
        {           
            IList<VehiclePoint> lvp = _vehicleService.GetVehiclePointsByDate(vid, dateTime);
            List<PointDouble> lp = new List<PointDouble>();
            foreach(VehiclePoint vp in lvp)
            {
                PointDouble point = new PointDouble();
                point.Lon = vp.Lon;
                point.Lat = vp.Lat;
                lp.Add(point);
            }

            return lp;
        }

        [HttpGet]
        [Route("LengthByDate")]
        public double GetLenthByDate(DateTime dateTime, int vid)
        {
            return _vehicleService.GetVehiclePointsLengthByDate(vid, dateTime);
        }

        [HttpGet]
        [Route("LengthByMonth")]
        public double GetLenthByMonth(DateTime dateTime, int vid)
        {
            return _vehicleService.GetVehiclePointsLengthByMonth(vid, dateTime);
        }

        [HttpGet]
        [Route("AverageSpeedInPoint")]
        public double AverageSpeedInPoint(int vid, double Lon, double Lat, DateTime dt)
        {
            return _vehicleService.pointsNearbyAverageSpeed(vid, new PointDouble(){Lat = Lat, Lon = Lon}, dt);
        }

        [HttpGet]
        [Route("GetCount")]
        public int Count()
        {           
            return _vehicleService.GetPointCount();
        }

        [HttpPost]
        [Route("CalculateFuelConsumption")]
        public IActionResult CalculateFuelConsumption([FromBody] PointsViewModelcs model)
        {
            var pom = _vehicleService.FuelConsum(model.Points, 10, null, 4578, SensorType.FuelRateMPG, 10000);
            double sum = 0;
            foreach (var item in pom)
                sum += item.Value * VehicleService.MPG_TO_LKM;

            double avg = sum / pom.Count;

            return new JsonResult(avg);
        }

        [HttpPost]
        [Route("GetVehicles")]
        public IActionResult GetVehicles()
        {
            int i = 1;
            var vehicles = _vehicleService.GetVehiclesIds();
            IList<VehiclesViewModel> viewModel = new List<VehiclesViewModel>();
            foreach (var vId in vehicles)
            {
                viewModel.Add(new VehiclesViewModel()
                {
                    VID = vId,
                    Vehicle = $"Vozilo_{i}"
                });

                i++;
            }
            return new JsonResult(viewModel);
        }
    }
}
