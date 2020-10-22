using Backend_App.EntityCore;
using Backend_App.EntityCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_App.Models
{
    public class SensorsService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SensorsService> _logger;
        private readonly PostgreDbContext _postgreDbContext;
        private List<SensorData> _sensorDataList = null;

        public SensorsService(
                   ILogger<SensorsService> logger,
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
            _sensorDataList = new List<SensorData>();
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Data\\OBD_DataPoints.txt");
            int i = 0;
            try
            {
                string line = string.Empty;
                using (streamReader = new StreamReader(path))
                {
                    Task previousInsert = null;
                    line = streamReader.ReadLine();
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] parameters = line.Split('\t');
                        var sensorData = new SensorData()
                        {
                            VID = Int32.Parse(parameters[0]),
                            DateTime = DateTime.Parse(parameters[1]),
                            Sensor = SensorData.ParseSensorType(parameters[2]),
                            Value = Convert.ToDouble(parameters[3])
                        };

                        if (i > 3990200)
                            _postgreDbContext.Add<SensorData>(sensorData);

                        i++;

                        if (i % 100 == 0)
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
                _logger.LogError(ex, "VehicleService" + i.ToString());
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
    }
}
