using Foundation;
using System;
using System.Collections.Generic;

namespace WeatherWidget
{
    /// <summary>
    /// A struct representing a single day of weather data.
    /// </summary>
    public class WeatherForecast
    {
        public int daysFromNow;

        public WeatherForecast(int daysFromNow, Weather forecast)
        {
            this.daysFromNow = daysFromNow;
            this.Forecast = forecast;
        }

        /// <summary>
        /// The weather forecast for the day.
        /// </summary>
        public Weather Forecast { get; set; }

        /// <summary>
        /// The number of days from now as a text description.
        /// </summary>
        public string DaysFromNowDescription
        {
            get
            {
                switch (this.daysFromNow)
                {
                    case 0: return "Today";
                    case 1: return "Tomorrow";
                    default: return $"In {daysFromNow} Days";
                }
            }
        }

        public static List<WeatherForecast> LoadSharedData()
        {
            // In case this is the first time the app has been run, create some initial data.
            if (!NSFileManager.DefaultManager.FileExists(SharedDataFileUrl.Path))
            {
                SaveSharedData(GenerateRandomForecastData());
            }

            var json = System.IO.File.ReadAllText(SharedDataFileUrl.Path);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeatherForecast>>(json);
        }

        /// <summary>
        /// Saves weather forecast data from the App Group container.
        /// </summary>
        public static void SaveSharedData(List<WeatherForecast> data)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            System.IO.File.WriteAllText(SharedDataFileUrl.Path, json);
        }

        /// <summary>
        /// Returns the URL of a data file in the shared App Group container.
        /// </summary>
        internal static NSUrl SharedDataFileUrl
        {
            get
            {

                var appGroupIdentifier = "group.com.xamarin.weather-widget";
                var url = NSFileManager.DefaultManager.GetContainerUrl(appGroupIdentifier);
                if (url == null)
                {
                    throw new Exception("Expected a valid app group container");
                }

                return url.Append("Data.plist", false);
            }
        }

        public static List<WeatherForecast> GenerateRandomForecastData()
        {
            var random = new Random();
            var maximum = Enum.GetNames(typeof(Weather)).Length;

            var data = new List<WeatherForecast>();
            for (var daysFromNow = 0; daysFromNow < 4; daysFromNow++)
            {
                var randomWeather = (Weather)random.Next(maximum);
                data.Add(new WeatherForecast(daysFromNow, randomWeather));
            }

            return data;
        }
    }
}