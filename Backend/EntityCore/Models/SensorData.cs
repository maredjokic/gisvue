using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_App.EntityCore.Models
{
    public class SensorData : SensorClass
    {
        [Key]
        public int Id { get; set; }

        public int VID { get; set; }

        public DateTime DateTime { get; set; }   
    }

    public class SensorClass
    {
        public SensorType Sensor { get; set; }

        public double Value { get; set; }

        public static SensorType ParseSensorType(string sensor)
        {
            switch (sensor)
            {
                case "Battery level": return SensorType.BateryLevel;
                case "RPM": return SensorType.RPM;
                case "Ignition": return SensorType.Ignition;
                case "Odometar": return SensorType.Odometar;
                case "Backup batery voltage": return SensorType.BackupBateryVoltage;
                case "Backup batery current": return SensorType.BackupBateryCurrent;
                case "Movement sensor": return SensorType.Movement;
                case "Fuel level": return SensorType.FuelLevel;
                case "Fuel rate (MPG)": return SensorType.FuelRateMPG;
                case "Mileage": return SensorType.Mileage;
            }
            throw new Exception("Sensor does not exists");
        }


        
    }

    public enum SensorType
    {
        /// <summary>
        /// Battery level
        /// </summary>
        BateryLevel,
        /// <summary>
        /// RPM
        /// </summary>
        RPM,
        /// <summary>
        /// Ignition
        /// </summary>
        Ignition,
        /// <summary>
        /// Odometar
        /// </summary>
        Odometar,
        /// <summary>
        /// Backup batery current
        /// </summary>
        BackupBateryCurrent,
        /// <summary>
        /// Backup batery current
        /// </summary>
        BackupBateryVoltage,
        /// <summary>
        /// Movement
        /// </summary>
        Movement,
        /// <summary>
        /// Fuel level
        /// </summary>
        FuelLevel,
        /// <summary>
        /// Fuel rate (MPG)
        /// </summary>
        FuelRateMPG,
        /// <summary>
        /// Mileage
        /// </summary>
        Mileage
    }   
}
