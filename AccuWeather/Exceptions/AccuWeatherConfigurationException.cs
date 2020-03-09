using System;
using System.Collections.Generic;
using System.Text;

namespace AccuWeather.Exceptions
{
    public class AccuWeatherConfigurationException : Exception
    {
        public AccuWeatherConfigurationException()
        {
        }

        public AccuWeatherConfigurationException(string message) : base(message)
        {
        }

        public AccuWeatherConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
