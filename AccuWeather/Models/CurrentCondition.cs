using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// All the classe from this namespace are a representation of the AccuWeather Current Conditions API results
/// Didn´t have enough time to get the descriptions on AccuWeather documentation
/// </summary>
namespace AccuWeather.Models.CurrentCondition
{
    public class CurrentCondition
    {
        public long EpochTime { get; set; }
        public string WeatherText { get; set; }
        public UnitSystems Temperature { get; set; }
        public int? WeatherIcon { get; set; }
        public int? RelativeHumidity { get; set; }
        public Wind Wind { get; set; }
        public bool IsDayTime { get; set; }
    }

    public class Measure
    {
        public decimal? Value { get; set; }
        public string Unit { get; set; }
    }

    public class Wind
    {
        public Direction Direction { get; set; }
        public UnitSystems Speed { get; set; }
    }

    public class UnitSystems
    {
        public Measure Metric { get; set; }
        public Measure Imperial { get; set; }
    }

    public class Direction
    {
        public string English { get; set; }
        public int? Degrees { get; set; }
    }
}