using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AccuWeather.ExtensionMethods;
using AccuWeather.Exceptions;
using System.Resources;
using System.Globalization;
using AccuWeather.Models;

namespace AccuWeather.APIHelpers
{
    /// <summary>
    /// Base API Helper class to use in AccuWeather API.
    /// For this project, I leave this class as abstract so that for each API in AccuWeather
    /// (Localization, Forecast, etc) we create a child class, wrapping all the methods from 
    /// the REST service. This is not a design usual nowadays, but I thought it was a good 
    /// way of showing skills in OO, generics, extension methods and a bit of reflection too 
    /// (to get the  right resources file, from the executing assembly, to globalization)
    /// </summary>
    public abstract class BaseAPIHelper
    {
        // A little bit of Globalization, Localization, Generics and a tiny drop of Reflection in this class :)

        public HttpClient Client { get; set; }

        public string APIKey { get; set; }

        public AccuWeatherConfigurations AccuWeatherConfigurations { get; set; }

        public ResourceManager LocalizedResourceManager { get; set; }


        public BaseAPIHelper(AccuWeatherConfigurations accuWeatherConfigurations)
        {
            AccuWeatherConfigurations = accuWeatherConfigurations;
            
            LocalizedResourceManager = new System.Resources.ResourceManager("AccuWeather.Resources.ExceptionMessages", System.Reflection.Assembly.GetExecutingAssembly());

            ValidateConfiguration(AccuWeatherConfigurations.ApiKey, "ApiKeyNotInformed");
            ValidateConfiguration(AccuWeatherConfigurations.BaseUrl, "BaseUrlNotInformed");

            Client = new HttpClient();
            Client.BaseAddress = new Uri(AccuWeatherConfigurations.BaseUrl.AddEndingSlash());
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            APIKey = AccuWeatherConfigurations.ApiKey;
        }

        /// <summary>
        /// Validate configurations and raise AccuWeatherConfigurationException in case of error.
        /// </summary>
        /// <typeparam name="T">Type of object to be validated</typeparam>
        /// <param name="obj">Object to be validated</param>
        /// <param name="resourceKey">Key for error message, located on localization Resource</param>
        protected void ValidateConfiguration<T>(T obj, string resourceKey)
        {
            if (obj == null)
                throw new AccuWeatherConfigurationException(LocalizedResourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture));

            if (typeof(T) == typeof(string) &&
                ((string.IsNullOrEmpty(obj.ToString()) || string.IsNullOrWhiteSpace(obj.ToString())))
                )
            { 
                throw new AccuWeatherConfigurationException(LocalizedResourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture));
            }
        }

        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> querystringParameters, params string[] urlParameters)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(url))
                sb.Append(url.RemoveEndingSlash());

            if (urlParameters != null)
            { 
                foreach (string item in urlParameters)
                {
                    if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
                        throw new AccuWeatherArgumentException(LocalizedResourceManager.GetString("EmptyUrlParameter", CultureInfo.CurrentUICulture));

                    sb.Append("/");
                    sb.Append(item);
                }
            }

            sb.Append("?apikey=");
            sb.Append(APIKey);

            if (querystringParameters != null)
            { 
                foreach (KeyValuePair<string, string> item in querystringParameters)
                {
                    if (string.IsNullOrEmpty(item.Key) || string.IsNullOrWhiteSpace(item.Key) || string.IsNullOrEmpty(item.Value) || string.IsNullOrWhiteSpace(item.Value))
                        throw new AccuWeatherArgumentException(LocalizedResourceManager.GetString("EmptyQueryStringParameter", CultureInfo.CurrentUICulture));

                    sb.Append("&");
                    sb.Append(item.Key);
                    sb.Append("=");
                    sb.Append(item.Value);
                }
            }

            try
            {
                using (HttpResponseMessage response = await Client.GetAsync(sb.ToString()))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        T result = JsonConvert.DeserializeObject<T>(responseString);
                        return result;
                    } else
                    {
                        // Unauthorized, Not Found, Incorret parameters, all these come from here
                        throw new AccuWeatherFaultyRequestException(response.ReasonPhrase);
                    }
                }
            }
            catch (AccuWeatherFaultyRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AccuWeatherException(ex.Message);
            }
        }

        public async Task<T> GetAsync<T>(Dictionary<string, string> querystringparameters)
        {
            return await GetAsync<T>(null, querystringparameters, null);
        }

        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> querystringparameters)
        {
            return await GetAsync<T>(url, querystringparameters, null);
        }

        public async Task<T> GetAsync<T>(string url)
        {
            return await GetAsync<T>(url, null, null);
        }

    }

}
