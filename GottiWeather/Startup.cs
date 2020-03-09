using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AccuWeather.Models;
using AccuWeatherAPIHelper.APIHelpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GottiWeather
{
    public class Startup
    {
        /// <summary>
        /// Supported cultures for the app
        /// </summary>
        private CultureInfo[] SupportedCultures { get; set; }

        /// <summary>
        /// Default culture for the app
        /// </summary>
        private RequestCulture DefaultRequestCulture { get; set; }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            SupportedCultures = new CultureInfo[]
            {
                new CultureInfo("pt-BR"),
                new CultureInfo("en-US")
            };

            DefaultRequestCulture = new RequestCulture("en-US", "en-US");
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Dependency Injection of AccuWeatherConfigurations (from appsetings.json)
            AccuWeatherConfigurations awConfigurations = Configuration.GetSection("AccuWeatherConfigurations").Get<AccuWeatherConfigurations>();

            // Dependency injection singletons: 
            //  - AccuWeatherConfigurations (from appsetings.json)
            //  - API Helpers
            services.AddSingleton<AccuWeatherConfigurations>(awConfigurations);
            services.AddSingleton<LocationAPIHelper>();
            services.AddSingleton<CurrentConditionsAPIHelper>();
            services.AddSingleton<ForecastAPIHelper>();
          

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                opts.DefaultRequestCulture = DefaultRequestCulture;
                // Formatting numbers, dates, etc.
                opts.SupportedCultures = SupportedCultures;
                // UI strings that we have localized.
                opts.SupportedUICultures = SupportedCultures;
                // Culture established by cookie
                opts.RequestCultureProviders = new IRequestCultureProvider[]
                {
                    new CookieRequestCultureProvider() { CookieName = CookieRequestCultureProvider.DefaultCookieName }
                };
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
