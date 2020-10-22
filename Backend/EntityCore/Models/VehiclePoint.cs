using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_App.Models
{
    public class VehiclePoint
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        ///  Vehicle identifer.
        /// </summary>
        public int VID { get; set; }

        /// <summary>
        ///  Is data valid for vehicle.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        ///  Datetime
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        ///  Latititude
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        ///  Vehicle speed
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Course
        /// </summary>
        public double Course { get; set; }
    }
}
