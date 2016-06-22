namespace AdMaiora.AppKit.UI
{
    using System;
    using Foundation;

    public static class NSDateExtension
    {
        public static DateTime ToDateTime(this NSDate date)
        {
            DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0);
            DateTime currentDate = reference.AddSeconds(date.SecondsSinceReferenceDate);
            DateTime localDate = currentDate.ToLocalTime();
            return localDate;
        }
    }
}