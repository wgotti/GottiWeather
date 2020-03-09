using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// All the classe from this namespace are a representation of the AccuWeather Location API results.
/// Didn´t have enough time to get the descriptions on AccuWeather documentation
/// </summary>
namespace AccuWeather.Models.Location
{
    public class Location
    {
        public string Key { get; set; }
        public string EnglishName { get; set; }
        public string LocalizedName { get; set; }
        public Info Country = new Info();
        public Info AdministrativeArea = new Info();
        public TimeZone TimeZone = new TimeZone();
    }

    public class Info
    {
        public string ID { get; set; }
        public string LocalizedName { get; set; }
        public string EnglishName { get; set; }
    }

    public class TimeZone
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal GmtOffset { get; set; }
    }
}
