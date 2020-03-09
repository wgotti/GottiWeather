using AccuWeather.Models;
using AccuWeather.Models.CurrentCondition;
using AccuWeather.Models.Forecast;
using AccuWeather.Models.Location;
using System;

namespace GottiWeather.Models
{
    public class HomeViewModel
    {
        public Location Location { get; set; }

        public CurrentCondition CurrentCondition { get; set; }

        public Forecast Forecast { get; set; }

        public bool IsMetricSystem { get; set; }

        public bool ErrorOccured { get; set; }
        public string Message { get; set; }
    }
}
