namespace ForexMiner.Heimdallr.Instruments.Storage.Model
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTime GetInclusiveCandleTime(this DateTime time, Granularity granularity)
        {
            return granularity switch
            {
                Granularity.M1 => new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0),
                Granularity.M5 => new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute - (time.Minute % 5), 0),
                Granularity.M10 => new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute - (time.Minute % 10), 0),
                Granularity.M15 => new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute - (time.Minute % 15), 0),
                Granularity.M30 => new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute - (time.Minute % 30), 0),
                Granularity.H1 => new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0),
                Granularity.H2 => new DateTime(time.Year, time.Month, time.Day, time.Hour - (time.Hour % 2), 0, 0),
                Granularity.H3 => new DateTime(time.Year, time.Month, time.Day, time.Hour - (time.Hour % 3), 0, 0),
                Granularity.H4 => new DateTime(time.Year, time.Month, time.Day, time.Hour - (time.Hour % 4), 0, 0),
                Granularity.H6 => new DateTime(time.Year, time.Month, time.Day, time.Hour - (time.Hour % 6), 0, 0),
                Granularity.H8 => new DateTime(time.Year, time.Month, time.Day, time.Hour - (time.Hour % 8), 0, 0),
                Granularity.H12 => new DateTime(time.Year, time.Month, time.Day, time.Hour - (time.Hour % 12), 0, 0),
                Granularity.D => new DateTime(time.Year, time.Month, time.Day),
                _ => new DateTime(time.Year, time.Month, 1),
            };
        }
    }
}
