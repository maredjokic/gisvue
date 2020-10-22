using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_App.EntityCore.Models
{ 
    public class DistancePointData //: SensorClass
    {
        /// <summary>
        ///  Vehicle identifer.
        /// </summary>
        public int VID { get; set; }

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
        ///  Distance from point
        /// </summary>
        public double Distance { get; set; }


        public double Value { get; set; }
    }
}
