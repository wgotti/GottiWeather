using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GottiWeather.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using AccuWeatherAPIHelper.APIHelpers;
using AccuWeather.Models;
using Microsoft.Extensions.Configuration;
using AccuWeather.Models.Location;
using AccuWeather.Models.CurrentCondition;
using AccuWeather.Models.Forecast;
using AccuWeather.Exceptions;

namespace GottiWeather.Controllers
{
    public class HomeController : Controller
    {
        private const string CookieLatitude = "GottiWeather.Latitude";
        private const string CookieLongitude = "GottiWeather.Longitude";
        private const string CookieLocationKey = "GottiWeather.Location.Key";
        private const string CookieLocationLocalizedName = "GottiWeather.Location.LocalizedName";
        private const string CookieLocationAdministrativeAreaID = "GottiWeather.Location.AdministrativeArea.ID";
        private const string CookieLocationCountryID = "GottiWeather.Location.Country.ID";
        private const string CookieIsMetricSystem = "GottiWeather.IsMetricSystem";
        private const int CookiesDaysExpiration = 7;

        public HomeViewModel ViewModel { get; }

        public IConfiguration Configuration { get; }


        /// <summary>
        /// Singleton AccuWeather API Helper to the <i>Locations API</i>
        /// </summary>
        public LocationAPIHelper LocationAPIHelper { get; }

        /// <summary>
        /// Singleton AccuWeather API Helper to the <i>Current Conditions API</i>
        /// </summary>
        public CurrentConditionsAPIHelper CurrentConditionsAPIHelper { get; }

        /// <summary>
        /// Singleton AccuWeather API Helper to the <i>Current Conditions API</i>
        /// </summary>
        public ForecastAPIHelper ForecastAPIHelper { get; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration config, LocationAPIHelper locationAPIHelper, CurrentConditionsAPIHelper currentConditionsAPIHelper, ForecastAPIHelper forecastAPIHelper, ILogger<HomeController> logger)
        {
            ViewModel = new HomeViewModel();

            _logger = logger;
            Configuration = config;
            LocationAPIHelper = locationAPIHelper;
            CurrentConditionsAPIHelper = currentConditionsAPIHelper;
            ForecastAPIHelper = forecastAPIHelper;          
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            KeyValuePair<string, string> cookieLocation = Request.Cookies.FirstOrDefault(n => n.Key == CookieLocationKey);

            // No location defined (didn´t call AccuWeather Location API yet) ...
            if (string.IsNullOrEmpty(cookieLocation.Value))
            { 
                // But Javascript sent Latitude
                if (Request.Cookies.Any(n => n.Key == CookieLatitude))
                {
                    CultureInfo javascriptCulture = new CultureInfo("en-US");

                    double latitude = double.Parse(Request.Cookies.FirstOrDefault(n => n.Key == CookieLatitude).Value, javascriptCulture);
                    double longitude = double.Parse(Request.Cookies.FirstOrDefault(n => n.Key == CookieLongitude).Value, javascriptCulture);

                    // Call AccuWeather Location API
                    await LoadLocation(latitude, longitude);

                    // If no Exceptions ocurred and API results came back...
                    if (ViewModel.Location != null)
                    {
                        // Sets cookie with Location needed data, avoiding new API Calls (cost-effective)
                        CookieOptions cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(CookiesDaysExpiration) };
                        Response.Cookies.Append(CookieLocationKey, ViewModel.Location.Key, cookieOptions);
                        Response.Cookies.Append(CookieLocationLocalizedName, ViewModel.Location.LocalizedName, cookieOptions);
                        Response.Cookies.Append(CookieLocationAdministrativeAreaID, ViewModel.Location.AdministrativeArea.ID, cookieOptions);
                        Response.Cookies.Append(CookieLocationCountryID, ViewModel.Location.Country.ID, cookieOptions);
                    } 
                    else // In case of error, get out
                    {
                        return View(ViewModel);
                    }
                }
            }

            // Location defined, in this post or previous (AccuWeather Location API already called)
            if (ViewModel.Location != null || !string.IsNullOrEmpty(cookieLocation.Value))
            {
                // Get the data needed from cookies, avoiding new API Calls (cost-effective) 
                if (!string.IsNullOrEmpty(cookieLocation.Value))
                {
                    ViewModel.Location = new Location()
                    {
                        Key = Request.Cookies.FirstOrDefault(n => n.Key == CookieLocationKey).Value,
                        LocalizedName = Request.Cookies.FirstOrDefault(n => n.Key == CookieLocationLocalizedName).Value,
                        AdministrativeArea = new Info()
                        {
                            ID = Request.Cookies.FirstOrDefault(n => n.Key == CookieLocationAdministrativeAreaID).Value
                        },
                        Country = new Info()
                        {
                            ID = Request.Cookies.FirstOrDefault(n => n.Key == CookieLocationCountryID).Value
                        }
                    };
                }

                KeyValuePair<string, string> cookieMetricSystem = Request.Cookies.FirstOrDefault(n => n.Key == CookieIsMetricSystem);

                // No metric system defined ...
                if (string.IsNullOrEmpty(cookieMetricSystem.Value))
                {
                    CookieOptions cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(CookiesDaysExpiration) };

                    ViewModel.IsMetricSystem = (CultureInfo.CurrentUICulture.Name.Equals("pt-BR"));
                    Response.Cookies.Append(CookieIsMetricSystem, ViewModel.IsMetricSystem.ToString(), cookieOptions);
                } 
                else
                {
                    ViewModel.IsMetricSystem = Boolean.Parse(cookieMetricSystem.Value);
                }

                // Call others AccuWeather APIs in parallel
                await RefreshAccuWeatherResults();
            }

            return View(ViewModel);
        }

        #region API Calls
        /// <summary>
        /// Call the AccuWeather API methods in parallel (Current Conditions and Next 5 Day Forecast)
        /// </summary>
        /// <returns></returns>
        private async Task RefreshAccuWeatherResults()
        {
            Task currentConditionsTask = LoadCurrentConditions();
            Task loadNext5DaysForescastTask = LoadNext5DaysForescast();

            await Task.WhenAll(currentConditionsTask, loadNext5DaysForescastTask);
        }

        /// <summary>
        /// Call the AccuWeather CurrentConditions API, method that gets actual wheather conditions
        /// </summary>
        /// <returns></returns>
        private async Task LoadCurrentConditions()
        {
            try
            {
                List<CurrentCondition> conditionsNow = await CurrentConditionsAPIHelper.CurrentConditionNow(ViewModel.Location.Key, true, CultureInfo.CurrentUICulture.Name);

                ViewModel.CurrentCondition = conditionsNow.FirstOrDefault();
            }
            catch (AccuWeatherArgumentException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadCurrentConditions", ex);
            }
            catch (AccuWeatherConfigurationException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadCurrentConditions", ex);
            }
            catch (AccuWeatherException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadCurrentConditions", ex);
            }
            catch (AccuWeatherFaultyRequestException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadCurrentConditions", ex);
            }

        }

        /// <summary>
        /// Call the AccuWeather Forecast API, method that gets the next 5 days forecast
        /// </summary>
        /// <returns></returns>
        private async Task LoadNext5DaysForescast()
        {
            try
            {
                Forecast forecastNext5Days = await ForecastAPIHelper.Next5Days(ViewModel.Location.Key, true, ViewModel.IsMetricSystem, CultureInfo.CurrentUICulture.Name);

                ViewModel.Forecast = forecastNext5Days;
            }
            catch (AccuWeatherArgumentException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadNext5DaysForescast", ex);
            }
            catch (AccuWeatherConfigurationException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadNext5DaysForescast", ex);
            }
            catch (AccuWeatherException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadNext5DaysForescast", ex);
            }
            catch (AccuWeatherFaultyRequestException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadNext5DaysForescast", ex);
            }
        }

        /// <summary>
        /// Call the AccuWeather Locations API, method that gets the location based on lat and long
        /// </summary>
        /// <returns></returns>
        private async Task LoadLocation(double latitude, double longitude)
        {
            try
            {
                Location location = await LocationAPIHelper.GetLocation(latitude, longitude, CultureInfo.CurrentUICulture.Name);

                ViewModel.Location = location;
            }
            catch (AccuWeatherArgumentException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadLocation", ex);
            }
            catch (AccuWeatherConfigurationException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadLocation", ex);
            }
            catch (AccuWeatherException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadLocation", ex);
            }
            catch (AccuWeatherFaultyRequestException ex)
            {
                // Could do a specific action for each of the erros, but didn´t have the time to do it. Sorry!
                LogError("LoadLocation", ex);
            }
        }

        /// <summary>
        /// Log error. Sure a better job could be done here, but didn´t have the time to do it and the goal was only to show that I´m pretty capable of exception handling. Sorry!
        /// </summary>
        private void LogError(string description, Exception ex)
        {
            ViewModel.ErrorOccured = true;
            ViewModel.Message = "Sorry, but an unexpected error occured while communicating to AccuWeather! Try again later!";
            _logger.LogError(description, ex);
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Changes the current language. Unfortunately, I didn´t have enough time to create the 
        /// Resources in the View Layer, and use a little bit more of dependency injection.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public IActionResult SetLanguage(string language)
        {
            CookieOptions cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(CookiesDaysExpiration) };

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language, language)),
                cookieOptions
            );

            return RedirectToAction("Index");
        }

        public IActionResult SwitchMetric()
        {
            CookieOptions cookieOptions = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(CookiesDaysExpiration) };

            KeyValuePair<string, string> cookieMetricSystem = Request.Cookies.FirstOrDefault(n => n.Key == CookieIsMetricSystem);

            if (!string.IsNullOrEmpty(cookieMetricSystem.Value))
            {
                ViewModel.IsMetricSystem = !Boolean.Parse(cookieMetricSystem.Value);

                Response.Cookies.Append(CookieIsMetricSystem, ViewModel.IsMetricSystem.ToString(), cookieOptions);
            }


            return RedirectToAction("Index");
        }
        
    }
}
