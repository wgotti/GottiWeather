using System;
using System.Collections.Generic;
using System.Text;

namespace AccuWeather.Exceptions
{
    public class AccuWeatherException : Exception
    {
        public AccuWeatherException()
        {
        }

        public AccuWeatherException(string message) : base(message)
        {
        }

        public AccuWeatherException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
