# GottiWeather

The project is mainly focused on confirming my skills as a C# developer, so I tried to use as many techniques possible: 

* Object oriented programming (abstract classes, hierarchy, reuse)
* Async programming (not a expert, yet)
* Dependency injection
* Generics
* Reflection (just a tiny drop)
* Extension methods (2 simple methods, just to remember you that I know how to use it)
* Globalization and localization (didn´t have the time to create all the resources)
* Exception handling (just the wireframe of that, to be continued)
* Cookies
* jQuery
* bootstrap
* css
* .net core
* mvc

dependency injection, generics, globalization, MVC, rest services, among others.  

I integrated AccuWeather API with .net Core, and created a simple responsive app to show a 5-day forecast. 

**AccuWeather API has 50 call limit by day. Set your own API key on appsetings.json, it´s free.**

Because this limitation on the free account at AccuWeather, I mocked the API with previous results. Tree files in the solution configure the use of this mock files:

* \AccuWeather\APIHelpers\CurrentConditionsAPIHelper.cs
* \AccuWeather\APIHelpers\ForecastAPIHelper.cs
* \AccuWeather\APIHelpers\LocationAPIHelper.cs

Each file has a method that calls the respective AccuWeatherAPI

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

That said, remember that the **!** before the **DEBUB** region isn´t used this way, it´s quite te opposite. I leave this way so that you can debug using the AccuWeather API, instead of the mocked files. But with just 50 calls/day, it will be necessary to use the files!

Have fun!
