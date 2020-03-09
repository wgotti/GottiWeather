using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// All the classe from this namespace are a representation of the AccuWeather Forecast API results.
/// Didn´t have enough time to get the descriptions on AccuWeather documentation
/// </summary>
namespace AccuWeather.Models.Forecast
{
    public class Forecast
    {
        public List<DailyForecast> DailyForecasts = new List<DailyForecast>();
    }
    public class DailyForecast
    {
        public long EpochDate { get; set; }
        public Information Day { get; set; }
        public Information Night { get; set; }
        public MinimuMaximum Temperature { get; set; }
    }

    public class MinimuMaximum
    {
        public Measure Maximum { get; set; }
        public Measure Minimum { get; set; }
    }

    public class Measure
    {
        public decimal? Value { get; set; }
        public string Unit { get; set; }
    }

    public class Information
    {
        public int? PrecipitationProbability { get; set; }
        public int Icon { get; set; }
        public string IconPhrase { get; set; }
        public string ShortPhrase { get; set; }
        public string LongPhrase { get; set; }
        public Wind Wind { get; set; }
    }

    public class Wind
    {
        public Speed Speed;
    }

    public class Speed
    {
        public float? Value { get; set; }
        public string Unit { get; set; }
    }
}