using AccuWeather.APIHelpers;
using AccuWeather.Models;
using AccuWeather.Models.Location;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AccuWeatherAPIHelper.APIHelpers
{
    public class LocationAPIHelper : BaseAPIHelper
    {
        /// <summary>
        /// Child class from BaseAPIHelper, wrapping all the methods os AccuWeather Location API
        /// </summary>
        public LocationAPIHelper(AccuWeatherConfigurations accuWeatherConfigurations) : base(accuWeatherConfigurations)
        {
            ValidateConfiguration(accuWeatherConfigurations.LocationsAPI, "LocationsAPI");
            ValidateConfiguration(accuWeatherConfigurations.LocationsAPI.GeopositionSearchUrl, "GeopositionSearchUrl");
        }

        /// <summary>
        /// Get the Location, based on latitude and longitude
        /// </summary>
        /// <param name="latitude">Latitude of the location to be retrieved</param>
        /// <param name="longitude">Longitude of the location to be retrieved</param>
        /// <param name="language">Language to the results</param>
        /// <returns>Object containing all the location details</returns>
        public async Task<Location> GetLocation(double latitude, double longitude, string language = "en-us")
        {
            // This DEBUG is not with ! before. Leave this way so that you can test in Visual Studio, using the real API. Just remember my license with AccuWeather has just 50 calls/day. You can change de Api-key on appsettings.json
#if !DEBUG
            Location loc;

            using (StreamReader srFile = new StreamReader(string.Concat(System.IO.Directory.GetCurrentDirectory(), "/Mock/Location.json"), Encoding.GetEncoding(28591)))
            {
                loc = JsonConvert.DeserializeObject<Location>(srFile.ReadToEnd());
            }
            // Used to simulate slow API response
            //await Task.Delay(5000);
            return loc;

#else

            Dictionary<string, string> querystringParameters = new Dictionary<string, string>();
            querystringParameters.Add("q", string.Concat(latitude, ",", longitude));
            querystringParameters.Add("language", language.ToString().ToLower());

            return await GetAsync<Location>(AccuWeatherConfigurations.LocationsAPI.GeopositionSearchUrl, querystringParameters);
#endif
        }
    }
}
