namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Linq;
    
    using Foundation;

    public static class NSBundleExtension
    {
        public static void PutString(this NSBundle bundle, string key, string value)
        {
            bundle.InfoDictionary[key] = new NSString(value);
        }

        public static string GetString(this NSBundle bundle, string key)
        {
            if (!bundle.InfoDictionary.ContainsKey(new NSString(key)))
                return default(string);

            return (bundle.InfoDictionary[key] as NSString).ToString();
        }

        public static void PutStringArray(this NSBundle bundle, string key, string[] values)
        {
            bundle.InfoDictionary[key] = new NSString(String.Join(new String((char)1, 1), values.Select(x => x.ToString())));
        }

        public static string[] GetStringArray(this NSBundle bundle, string key)
        {
            if (!bundle.InfoDictionary.ContainsKey(new NSString(key)))
                return default(string[]);

            string s = (bundle.InfoDictionary[key] as NSString).ToString();
            if (String.IsNullOrWhiteSpace(s))
                return default(string[]);

            string[] array = s.Split(new[] { new String((char)1, 1) }, StringSplitOptions.None);
            return array;
        }


        public static void PutInt(this NSBundle bundle, string key, int value)
        {
            bundle.InfoDictionary[key] = new NSNumber(value);
        }

        public static int GetInt(this NSBundle bundle, string key)
        {
            if (!bundle.InfoDictionary.ContainsKey(new NSString(key)))
                return default(int);

            return (bundle.InfoDictionary[key] as NSNumber).Int32Value;
        }

        public static void PutIntArray(this NSBundle bundle, string key, int[] values)
        {
            bundle.InfoDictionary[key] = new NSString(String.Join(",", values));
        }

        public static int[] GetIntArray(this NSBundle bundle, string key)
        {
            if (!bundle.InfoDictionary.ContainsKey(new NSString(key)))
                return default(int[]);

            string s = (bundle.InfoDictionary[key] as NSString).ToString();
            if (String.IsNullOrWhiteSpace(s))
                return default(int[]);

            string[] array = s.Split(new string[] { "," }, StringSplitOptions.None);
            int[] intArray = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                intArray[i] = Int32.Parse(array[i]);

            return intArray;
        }

        public static void PutBoolean(this NSBundle bundle, string key, bool value)
        {
            bundle.InfoDictionary[key] = new NSNumber(value);
        }

        public static bool GetBoolean(this NSBundle bundle, string key)
        {
            if (!bundle.InfoDictionary.ContainsKey(new NSString(key)))
                return default(bool);

            return (bundle.InfoDictionary[key] as NSNumber).BoolValue;
        }
    }
}