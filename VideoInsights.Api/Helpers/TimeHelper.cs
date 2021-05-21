using System;
using System.Globalization;

namespace VideoInsights.Api.Helpers
{
    public static class TimeHelper
    {
        public static double ConvertTime(string time)
        {
            var culture = CultureInfo.CurrentCulture;
            var format = "H:mm:ss.f"; //0:00:02.7

            if (time.Length < 9)
                time += ".0";

            return DateTime.ParseExact(time.Substring(0, 9), format, culture).TimeOfDay.TotalSeconds;
        }
    }
}