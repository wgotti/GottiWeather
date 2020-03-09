using System;
using System.Collections.Generic;
using System.Text;

namespace AccuWeather.Models
{
 
    /// <summary>
    /// This class represents the necessary configurations (from appsettings.json 
    /// on MVC project) to ease the creation of the classes based on BaseAPIHelper
    /// It is injected in all the Controllers on the MVC project, and instanciating 
    /// the child classes gets a lot faster, since all the necessary information 
    /// are here
    /// </summary>
    public class AccuWeatherConfigurations
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public LocationsAPI LocationsAPI { get; set; }
        public CurrentConditionsAPI CurrentConditionsAPI { get; set; }
        public ForecastAPI ForecastAPI { get; set; }
    }

    /// <summary>
    /// Contains all the endpoints to the methods in AccuWeather Locations API
    /// </summary>
    public class LocationsAPI
    {
        public string GeopositionSearchUrl { get; set; }
    }

    /// <summary>
    /// Contains all the endpoints to the methods in AccuWeather Current Conditions API
    /// </summary>
    public class CurrentConditionsAPI
    {
        public string ConditionsNowUrl { get; set; }
    }

    /// <summary>
    /// Contains all the endpoints to the methods in AccuWeather Forecast API
    /// </summary>
    public class ForecastAPI
    {
        public string Next5DaysUrl { get; set; }
    }
}
