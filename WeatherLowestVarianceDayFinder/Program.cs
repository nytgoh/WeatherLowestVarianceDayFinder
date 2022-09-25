using System;
using System.Collections.Generic;
using System.IO;

namespace WeatherLowestVarianceDayFinder
{
    class WeatherLowestVarianceDayFinder
    {
        private static string filePath = "..\\..\\..\\weather.txt";

        static void Main(string[] args)
        {
            int result = GetLowestVarianceDayFromFile(filePath);

            if (result == -1)
            {
                Console.WriteLine("Could not find valid data within the given file. Please double check the configs under WeatherVarianceLineService are correct.");
            } 
            else
            {
                Console.WriteLine("The day with the lowest temperature variance is: " + GetLowestVarianceDayFromFile(filePath));
            }
        }

        /// <summary>
        /// Gets the day number for the day that has the lowest variance between the max and min temperatures in a weather data set provided.
        /// </summary>
        /// <param name="filePath">Path for pointing towards the weather data set txt file.</param>
        /// <returns>Integer day number that has the lowest variance.</returns>
        private static int GetLowestVarianceDayFromFile(string filePath)
        {
            // Read
            IEnumerable<string> lines = File.ReadLines(filePath);

            // Parse
            List<WeatherDay> weatherDays = WeatherVarianceLineService.ParseLines(lines);

            // Process
            WeatherDay lowestVarianceDay = WeatherVarianceLineService.GetWeatherDayWithLowestVariance(weatherDays);

            return lowestVarianceDay == null ? -1 : lowestVarianceDay.Day;
        }
    }
}
