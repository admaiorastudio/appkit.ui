namespace AdMaiora.AppKit.UI.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;

    using Newtonsoft;

    public static class BundleExtension
    {
        #region Public DateTime Methods

        public static void PutDateTime(this Bundle bundle, string key, DateTime value)
        {
            bundle.PutString(key, String.Concat(value.Ticks, ",", value.Kind));
        }
        public static DateTime GetDateTime(this Bundle bundle, string key, DateTime defaultValue)
        {
            string dateDescription = bundle.GetString(key, null);
            if (String.IsNullOrWhiteSpace(dateDescription))
                return defaultValue;

            long ticks = Int64.Parse(dateDescription.Split(',')[0]);
            DateTimeKind kind = (DateTimeKind)Int32.Parse(dateDescription.Split(',')[1]);
            return new DateTime(ticks, kind);               
        }
        public static DateTime GetDateTime(this Bundle bundle, string key)
        {
            return GetDateTime(bundle, key, default(DateTime));
        }

        #endregion

        #region Public Object Methods

        public static void PutObject<T>(this Bundle bundle, string key, T obj)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            bundle.PutString(key, json);
        }
        public static T GetObject<T>(this Bundle bundle, string key, T defaultValue)
        {
            string json = bundle.GetString(key, null);
            if (json == null)
                return defaultValue;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);            
        }
        public static T GetObject<T>(this Bundle bundle, string key)
        {
            return GetObject<T>(bundle, key, default(T));
        }

        #endregion
    }
}