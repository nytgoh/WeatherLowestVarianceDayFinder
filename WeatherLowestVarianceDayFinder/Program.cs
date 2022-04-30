using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace WeatherLowestVarianceDayFinder
{
    class WeatherLowestVarianceDayFinder
    {
        private static string filePath = "C:\\Users\\nadia.goh\\Downloads\\weather.txt";

        private static string tableTag = "pre";

        private static string toFilterFromTable = "*";

        static void Main(string[] args)
        {
            Console.WriteLine("The day with the lowest temperature variance is: " +
                              GetLowestVarianceDayFromFile(filePath));
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

        /// <summary>
        /// Gets the day number for the day that has the lowest variance between the max and min temperatures in a weather data set provided.
        /// </summary>
        /// <param name="filePath">Path for pointing towards the weather data set txt file.</param>
        /// <returns>Integer day number that has the lowest variance.</returns>
        private static int GetLowestVarianceDayFromFile(string filePath)
        {
            IEnumerable<string> lines = File.ReadLines(filePath);

            bool processLine = false;
            decimal lowestVariance = int.MaxValue;
            int lowestVarianceDay = -1;

            foreach (var line in lines)
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
                    List<string> parts = FetchColumnValuesFromRowLine(line, new List<int>() {1, 2, 3}, toFilterFromTable);

                    if (parts == null)
                    {
                        continue;
                    }

                    int day, max, min;
                    if (int.TryParse(parts[0], out day) && int.TryParse(parts[1], out max) && int.TryParse(parts[2], out min))
                    {
                        decimal variance = (max - min) / (max * 1.0m);
                        if (lowestVariance > variance)
                        {
                            lowestVariance = variance;
                            lowestVarianceDay = day;
                        }
                    }
                }
            }

            return lowestVarianceDay;
        }
    }
}
