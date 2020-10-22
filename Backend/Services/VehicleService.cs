using Backend_App.EntityCore;
using Backend_App.EntityCore.Models;
using Backend_App.Models;
using Backend_App.QueryBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_App.Services
{
    public class VehicleService
    {
        public static double MPG_TO_LKM = 0.354006189934;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VehicleService> _logger;
        private readonly PostgreDbContext _postgreDbContext;
        private List<VehiclePoint> _vehicleList = null;

        public VehicleService(
            ILogger<VehicleService> logger,
            IWebHostEnvironment hostingEnvironment,
            PostgreDbContext dbContext,
            IConfiguration configuration)
        {

            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _postgreDbContext = dbContext;
            _configuration = configuration;

            bool ImportData = configuration.GetValue<bool>("ImportData");
            if (ImportData)
                LoadData();
        }

        public void LoadData()
        {
            StreamReader streamReader = null;
            _vehicleList = new List<VehiclePoint>();
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Data\\AVL_DataPoints.txt");
            try
            {
                int i = 0;
                string line = string.Empty;
                using (streamReader = new StreamReader(path))
                {
                    Task previousInsert = null;
                    line = streamReader.ReadLine();
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] parameters = line.Split('\t');
                        var point = new VehiclePoint()
                        {
                            VID = Int32.Parse(parameters[0]),
                            Valid = Convert.ToInt32(parameters[1]) > 0 ? true : false,
                            DateTime = DateTime.Parse(parameters[2]),
                            Lat = Double.Parse(parameters[3]),
                            Lon = Double.Parse(parameters[4]),
                            Speed = Double.Parse(parameters[5]),
                            Course = Double.Parse(parameters[6])
                        };

                        _postgreDbContext.Add<VehiclePoint>(point);
                        i++;

                        if (i % 20 == 0)
                        {
                            if (previousInsert != null)
                                previousInsert.Wait();

                            previousInsert = _postgreDbContext.SaveChangesAsync();
                        }
                    }
                    _postgreDbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VehicleService");
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader.Dispose();
                }
            }
        }

        public IList<VehiclePoint> GetByVehicleId(int vehicleId)
        {
            return _vehicleList.Where(x => x.VID == vehicleId).ToList();
        }

        public IList<VehiclePoint> GetVehiclePointsFirst(int vehicleId, int count, int skip)
        {
            return _postgreDbContext.VehiclePointDbSet
                  .Where(v => v.VID == vehicleId && v.Speed != 0.0)
                  .OrderBy(v => v.DateTime)
                  .Skip(skip)
                  .Take(count)
                  .ToList();
        }

        public IList<VehiclePoint> GetVehiclePointsByDate(int vid, DateTime dateTime)
        {
            return _postgreDbContext.VehiclePointDbSet
                  .Where(v => v.VID == vid && v.DateTime.Month == dateTime.Month && v.DateTime.Year == dateTime.Year && v.DateTime.Day == dateTime.Day)
                  .OrderBy(v => v.DateTime)
                  .ToList();
        }

        public int GetPointCount()
        {
            return _postgreDbContext.VehiclePointDbSet.Count();
        }

        public IList<int> GetVehiclesIds()
        {
            return _postgreDbContext.VehiclePointDbSet
                   .GroupBy(x => x.VID)
                   .Select(p => p.Key).ToList();                   
        }

        public double CalculateDistance(PointDouble point1, PointDouble point2)
        {
            var d1 = point1.Lat * (Math.PI / 180.0);
            var num1 = point1.Lon * (Math.PI / 180.0);
            var d2 = point2.Lat * (Math.PI / 180.0);
            var num2 = point2.Lon * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                    Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        } // in meters

        public double GetVehiclePointsLengthByDate(int vid, DateTime dateTime)
        {
            double length = 0.0;

            for(int i = 1; i < 32; i++)
            {
                IList<VehiclePoint> vehiclePointss = _postgreDbContext.VehiclePointDbSet
                    .Where(v => v.VID == vid && v.DateTime.Day == i && v.DateTime.Month == dateTime.Month && v.DateTime.Year == dateTime.Year && v.DateTime.Day == dateTime.Day)
                    .OrderBy(v => v.DateTime)
                    .ToList();

                length = length + ListVehiclePointToLength(vehiclePointss);
            }

            return length;
        }

        public double GetVehiclePointsLengthByMonth(int vid, DateTime dateTime)
        {
            IList<VehiclePoint> vehiclePointss = _postgreDbContext.VehiclePointDbSet
                  .Where(v => v.VID == vid && v.DateTime.Month == dateTime.Month && v.DateTime.Year == dateTime.Year)
                  .OrderBy(v => v.DateTime)
                  .ToList();

            return ListVehiclePointToLength(vehiclePointss);
        }

        public double ListVehiclePointToLength(IList<VehiclePoint> vehiclePointss)
        {
            double length = 0.0;

            for(int i = 0; i < vehiclePointss.Count - 1; i++)
            {
                PointDouble p1 = new PointDouble();
                p1.Lat = vehiclePointss[i].Lat;
                p1.Lon = vehiclePointss[i].Lon;
                PointDouble p2 = new PointDouble();
                p2.Lat = vehiclePointss[i + 1].Lat;
                p2.Lon = vehiclePointss[i + 1].Lon;
                length = length + CalculateDistance(p1, p2);
            }

            return length;
        }

        public PointDouble MidPoint(PointDouble posA, PointDouble posB)
        {
            PointDouble midPoint = new PointDouble();

            double dLon = DegreesToRadians(posB.Lon - posA.Lon);
            double Bx = Math.Cos(DegreesToRadians(posB.Lat)) * Math.Cos(dLon);
            double By = Math.Cos(DegreesToRadians(posB.Lat)) * Math.Sin(dLon);

            midPoint.Lat = RadiansToDegrees(Math.Atan2(
                            Math.Sin(DegreesToRadians(posA.Lat)) + Math.Sin(DegreesToRadians(posB.Lat)),
                            Math.Sqrt(
                                (Math.Cos(DegreesToRadians(posA.Lat)) + Bx) *
                                (Math.Cos(DegreesToRadians(posA.Lat)) + Bx) + By * By))); 

            midPoint.Lat = posA.Lon + RadiansToDegrees(Math.Atan2(By, Math.Cos(DegreesToRadians(posA.Lat)) + Bx));

            return midPoint;
        }

        public double DegreesToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public double RadiansToDegrees(double degrees)
        {
            return degrees * (Math.PI/180);
        }

        public IList<DistancePointData> FuelConsum(List<Point> Points, int LessThenMeters, DateTimeWhere dateTimeWhere, int VehicleId, SensorType SensorType, int Limit = 0)
        {
            string select, fromm, orderBy;
            StringBuilder stringBuilder = new StringBuilder();

            fromm = $" FROM public.\"SensorData\" AS SD INNER JOIN public.\"VehiclePoint\" AS VP  ON SD.\"DateTime\" = VP.\"DateTime\" ";

            string where = $"WHERE VP.\"VID\" = {VehicleId} AND SD.\"Sensor\" = {(int)SensorType} AND ";

            if (dateTimeWhere != null)
                where += QueryBuilder.QueryBuilder.DateTimeQuery(dateTimeWhere) + " AND ";

            string DistanceSelector = string.Empty;
            if (Points != null)
            {
                where += QueryBuilder.QueryBuilder.St_Distance($"ST_Transform(ST_SetSRID(ST_Point(VP.\"Lat\", VP.\"Lon\"), 4326), 2163)",
                    QueryBuilder.QueryBuilder.Linestring(Points), LessThenMeters);

                DistanceSelector = QueryBuilder.QueryBuilder.St_Distance($"ST_Transform(ST_SetSRID(ST_Point(VP.\"Lat\", VP.\"Lon\"), 4326), 2163)",
                    QueryBuilder.QueryBuilder.Linestring(Points), null);
            }

            select = $"SELECT {DistanceSelector} AS \"Distance\", VP.\"Lat\", VP.\"Lon\", VP.\"DateTime\", SD.\"Value\", VP.\"VID\", VP.\"Speed\" ";
            orderBy = $" ORDER BY VP.\"Id\" ASC LIMIT {Limit}";

            string query = select + fromm + where + orderBy;

            var pom = _postgreDbContext.DistancePoint.FromSqlRaw(query).ToList();

            return pom;
        }

        public double pointsNearbyAverageSpeed(int vid, PointDouble pd, DateTime dt)
        {
            IList<VehiclePoint> vp =  _postgreDbContext.VehiclePointDbSet
                .Where(v => v.VID == vid && v.Speed != 0.0)
                .ToList();

            IList<VehiclePoint> vpnew = new List<VehiclePoint>();

            foreach(VehiclePoint vpp in vp)
            {
                if((CalculateDistance(pd, new PointDouble(){Lat =vpp.Lat, Lon= vpp.Lon}) < 10.0))
                {
                    vpnew.Add(vpp);
                }
            }

            return averageSpeed(vpnew);
        }

        public double averageSpeed(IList<VehiclePoint> vp)
        {
            int number = 0;
            double speed = 0.0;

            foreach(VehiclePoint vpp in vp)
            {
                number++;
                speed += vpp.Speed;
            }
            speed = speed/number;
            return speed;
        }
    }
}
