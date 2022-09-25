using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace WeatherLowestVarianceDayFinder
{
    internal class WeatherDay
    {
        public int Day { get; set; }

        public int Max { get; set; }

        public int Min { get; set; }

        public WeatherDay(int day, int max, int min)
        {
            Day = day;
            Max = max;
            Min = min;
        }
    }

    class WeatherVarianceLineService
    {
        private static string tableTag = "pre";

        private static string toFilterFromTable = "*";

        // The below can be altered if the column in which the specific values are held changes
        private static int dayColumn = 1;

        private static int maxColumn = 2;

        private static int minColumn = 3;

        /// <summary>
        /// Parses the raw information from the text file lines into a list of weather days to be processed
        /// </summary>
        /// <param name="lines">Raw string lines from the file provided.</param>
        /// <returns> A list of weather days containing day numbers, max temp and min temp of valid lines. </returns>
        public static List<WeatherDay> ParseLines(IEnumerable<string> lines)
        {
            bool processLine = false;
            List<WeatherDay> weatherDays = new List<WeatherDay>();

            foreach (string line in lines)
            {
                // only process line if between the table tags <pre></pre> in this case
                if (line.Contains(tableTag))
                {
                    processLine = !processLine;
                    continue;
                }

                if (processLine)
                {
                    // first three columns are day, max, min
                    List<string> parts = FetchColumnValuesFromRowLine(line, new List<int>() { dayColumn, maxColumn, minColumn }, toFilterFromTable);

                    int day = 0, max = 0, min = 0;
                    if (parts != null && parts.Count == 3 && int.TryParse(parts[0], out day) && int.TryParse(parts[1], out max) && int.TryParse(parts[2], out min))
                    {
                        weatherDays.Add(new WeatherDay(day, max, min));
                    }
                }
            }

            return weatherDays;
        }

        /// <summary>
        /// Gets the weather day with the lowest temperature variance
        /// </summary>
        /// <param name="weatherDays">Parsed information about valid weather days provided with day number, max temp and min temp.</param>
        /// <returns> The weather day with the lowest temperature variance </returns>
        public static WeatherDay GetWeatherDayWithLowestVariance(List<WeatherDay> weatherDays)
        {
            decimal lowestVariance = int.MaxValue;
            WeatherDay lowestVarianceDay = null;

            foreach(WeatherDay day in weatherDays)
            { 
                decimal variance = (day.Max - day.Min) / (day.Max * 1.0m);
                if (lowestVariance > variance)
                {
                    lowestVariance = variance;
                    lowestVarianceDay = day;
                }
            }

            return lowestVarianceDay;
        }

        /// <summary>
        /// This fetches a list of strings, where each element contains a single cell value from the row input.
        /// </summary>
        /// <param name="tableRowLine">String form of a row of a table. Columns split by some form of spaces.</param>
        /// <param name="columnsToFetch">List of integers representing what columns to extract and return.</param>
        /// <param name="otherCharsToFilter">String containing characters to ignore/filter out e.g. "*%" will ignore those two characters.</param>
        /// <returns>A List of strings containing the cell values of the row. Null if there are not enough columns in the string to pull the full result.</returns>
        private static List<string> FetchColumnValuesFromRowLine(string tableRowLine, List<int> columnsToFetch, string otherCharsToFilter)
        {
            var spaceFilter = new Regex("[\\s]+");
            var otherFilter = new Regex("[" + otherCharsToFilter + "]");

            string filteredLine = otherFilter.Replace(tableRowLine, string.Empty);
            List<string> parts = spaceFilter.Replace(filteredLine, " ").Trim().Split().ToList();

            if (parts.Count < columnsToFetch.Max())
            {
                return null;
            }

            return parts.Where(x => columnsToFetch.Contains(parts.IndexOf(x) + 1)).ToList();
        }
    }
}
