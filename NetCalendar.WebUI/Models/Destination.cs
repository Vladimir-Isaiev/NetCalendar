using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetCalendar.WebUI.Models
{
    public class Destination
    {
        public double GeoLat { get; set; }
        public double GeoLong { get; set; }

        public Destination(double Geolat = 50.44, double Geolong = 30.5)
        {
            this.GeoLat = Geolat;
            this.GeoLong = GeoLong;
        }
    }
}