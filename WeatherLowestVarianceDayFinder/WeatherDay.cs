namespace WeatherLowestVarianceDayFinder
{
    class WeatherDay
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
}
