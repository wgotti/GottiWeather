using System;
using System.Collections.Generic;
using System.Text;

namespace AccuWeather.Exceptions
{
    public class AccuWeatherFaultyRequestException : Exception
    {
        public AccuWeatherFaultyRequestException()
        {
        }

        public AccuWeatherFaultyRequestException(string message) : base(message)
        {
        }

        public AccuWeatherFaultyRequestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
