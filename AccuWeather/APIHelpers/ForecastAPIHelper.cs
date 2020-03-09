using AccuWeather.APIHelpers;
using AccuWeather.Models;
using AccuWeather.Models.Forecast;
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
    /// <summary>
    /// Child class from BaseAPIHelper, wrapping all the methods os AccuWeather Forecast API
    /// </summary>
    public class ForecastAPIHelper : BaseAPIHelper
    {
        public ForecastAPIHelper(AccuWeatherConfigurations accuWeatherConfigurations) : base(accuWeatherConfigurations)
        {
            ValidateConfiguration(accuWeatherConfigurations.CurrentConditionsAPI, "ForecastAPI");
            ValidateConfiguration(accuWeatherConfigurations.CurrentConditionsAPI.ConditionsNowUrl, "Next5DaysUrl");
        }

        /// <summary>
        /// Get the Forecast to the next 5 days from a specified location
        /// </summary>
        /// <param name="locationKey">Location Key</param>
        /// <param name="details">Include details on results</param>
        /// <param name="metric">Metric system</param>
        /// <param name="language">Language to the results</param>
        /// <returns>List containing 5 forecast objects, one per day</returns>
        public async Task<Forecast> Next5Days(string locationKey, bool details = false, bool metric = false, string language = "en-us")
        {
            // This DEBUG is not with ! before. Leave this way so that you can test in Visual Studio, using the real API. Just remember my license with AccuWeather has just 50 calls/day. You can change de Api-key on appsettings.json
#if !DEBUG
            string fileName;
            if (metric)
            {
                fileName = "/Mock/Forecast-M.json";
            }
            else
            {
                fileName = "/Mock/Forecast-I.json";
            }

            using (StreamReader srFile = new StreamReader(string.Concat(System.IO.Directory.GetCurrentDirectory(), fileName), Encoding.GetEncoding(28591)))
            {
                return JsonConvert.DeserializeObject<Forecast>(srFile.ReadToEnd());
            }
#else
            Dictionary<string, string> querystringParameters = new Dictionary<string, string>();
            querystringParameters.Add("details", details.ToString().ToLower());
            querystringParameters.Add("metric", metric.ToString().ToLower());
            querystringParameters.Add("language", language.ToString().ToLower());

            return await GetAsync<Forecast>(AccuWeatherConfigurations.ForecastAPI.Next5DaysUrl, querystringParameters, locationKey);
#endif
        }
    }
}
