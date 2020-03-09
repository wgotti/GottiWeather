using System;
using System.Collections.Generic;
using System.Text;

namespace AccuWeather.Exceptions
{
    public class AccuWeatherArgumentException : ArgumentException
    {
        public AccuWeatherArgumentException()
        {
        }

        public AccuWeatherArgumentException(string message) : base(message)
        {
        }

        public AccuWeatherArgumentException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
