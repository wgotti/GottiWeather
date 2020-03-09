using System;
using System.Collections.Generic;
using System.Text;

namespace AccuWeather.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static string AddEndingSlash(this string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.EndsWith("/"))
                    url = string.Concat(url, "/");
            }

            return url;
        }

        public static string RemoveEndingSlash(this string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.EndsWith("/"))
                    url = url.Remove(url.Length-1);
            }

            return url;
        }
    }
}
