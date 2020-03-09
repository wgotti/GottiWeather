using AccuWeather.APIHelpers;
using AccuWeather.Models;
using AccuWeather.Models.CurrentCondition;
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
    /// Child class from BaseAPIHelper, wrapping all the methods os AccuWeather CurrentConditions API
    /// </summary>
    public class CurrentConditionsAPIHelper : BaseAPIHelper
    {
        public CurrentConditionsAPIHelper(AccuWeatherConfigurations accuWeatherConfigurations) : base(accuWeatherConfigurations)
        {
            ValidateConfiguration(accuWeatherConfigurations.CurrentConditionsAPI, "CurrentConditionsAPI");
            ValidateConfiguration(accuWeatherConfigurations.CurrentConditionsAPI.ConditionsNowUrl, "ConditionsNowUrl");
        }

        /// <summary>
        /// Get the Current Conditions from a specified location at the moment
        /// </summary>
        /// <param name="locationKey">Location Key</param>
        /// <param name="details">Include details on results</param>
        /// <param name="language">Language to the results</param>
        /// <returns>Object containing all the current conditions</returns>
        public async Task<List<CurrentCondition>> CurrentConditionNow(string locationKey, bool details = false, string language = "en-us")
        {
            // This DEBUG is not with ! before. Leave this way so that you can test in Visual Studio, using the real API. Just remember my license with AccuWeather has just 50 calls/day. You can change de Api-key on appsettings.json
#if !DEBUG
            using (StreamReader srFile = new StreamReader(string.Concat(System.IO.Directory.GetCurrentDirectory(), "/Mock/CurrentConditions.json"), Encoding.GetEncoding(28591)))
            {
                return JsonConvert.DeserializeObject<List<CurrentCondition>>(srFile.ReadToEnd());
            }

#else
            Dictionary<string, string> querystringParameters = new Dictionary<string, string>();
            querystringParameters.Add("details", details.ToString().ToLower());
            querystringParameters.Add("language", language.ToString().ToLower());

            return await GetAsync<List<CurrentCondition>>(AccuWeatherConfigurations.CurrentConditionsAPI.ConditionsNowUrl, querystringParameters, locationKey);
#endif
        }
    }
}
