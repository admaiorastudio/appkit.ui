namespace AdMaiora.AppKit.UI
{   
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Foundation;

    public class UIBundle
    {
        #region Constants and Fields

        private Dictionary<string, object> _values;

        #endregion

        #region Constructors

        public UIBundle()
        {
            _values = new Dictionary<string, object>();
        }

        #endregion

        #region Public Methods

        public void PutAll(UIBundle bundle)
        {
            foreach (var kvp in bundle._values)
                _values.Add(kvp.Key, kvp.Value);
        }

        #endregion

        #region Public String Methods

        public void PutString(string key, string value)
        {
            _values[key] = value;
        }
        public string GetString(string key, string defaultValue)
        {
            if (!_values.ContainsKey(key))
                return defaultValue;

            return _values[key]?.ToString();
        }
        public string GetString(string key)
        {
            return GetString(key, default(string));
        }

        public void PutStringArray(string key, string[] values)
        {
            _values[key] =
                String.Join(new String((char)1, 1), values.Select(x => x.ToString()));
        }
        public string[] GetStringArray(string key)
        {
            if (!_values.ContainsKey(key))
                return default(string[]);

            string s = _values[key].ToString();
            if (String.IsNullOrWhiteSpace(s))
                return default(string[]);

            string[] array = s.Split(new[] { new String((char)1, 1) }, StringSplitOptions.None);
            return array;
        }

        #endregion

        #region Public Numeric Methods

        public void PutByte(string key, byte value)
        {
            _values[key] = value;
        }
        public byte GetByte(string key, byte defaultValue)
        {
            if (!_values.ContainsKey(key))
                return defaultValue;

            return (byte)_values[key];
        }
        public byte GetByte(string key)
        {
            return GetByte(key, default(byte));
        }

        public void PutByteArray(string key, byte[] values)
        {
            _values[key] = String.Join(",", values);
        }
        public byte[] GetByteArray(string key)
        {
            if (!_values.ContainsKey(new NSString(key)))
                return default(byte[]);

            string s = _values[key].ToString();
            if (String.IsNullOrWhiteSpace(s))
                return default(byte[]);

            string[] array = s.Split(new string[] { "," }, StringSplitOptions.None);
            byte[] byteArray = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
                byteArray[i] = Byte.Parse(array[i]);

            return byteArray;
        }

        public void PutInt(string key, int value)
        {
            _values[key] = value;
        }
        public int GetInt(string key, int defaultValue)
        {
            if (!_values.ContainsKey(key))
                return defaultValue;

            return (int)_values[key];
        }
        public int GetInt(string key)
        {
            return GetInt(key, default(int));
        }

        public void PutIntArray(string key, int[] values)
        {
            _values[key] = String.Join(",", values);
        }
        public int[] GetIntArray(string key)
        {
            if (!_values.ContainsKey(new NSString(key)))
                return default(int[]);

            string s = _values[key].ToString();
            if (String.IsNullOrWhiteSpace(s))
                return default(int[]);

            string[] array = s.Split(new string[] { "," }, StringSplitOptions.None);
            int[] intArray = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                intArray[i] = Int32.Parse(array[i]);

            return intArray;
        }

        public void PutLong(string key, long value)
        {
            _values[key] = value;
        }
        public long GetLong(string key, long defaultValue)
        {
            if (!_values.ContainsKey(key))
                return defaultValue;

            return (long)_values[key];
        }
        public long GetLong(string key)
        {
            return GetLong(key, default(long));
        }

        public void PutLongArray(string key, long[] values)
        {
            _values[key] = String.Join(",", values);
        }
        public long[] GetLongArray(string key)
        {
            if (!_values.ContainsKey(new NSString(key)))
                return default(long[]);

            string s = _values[key].ToString();
            if (String.IsNullOrWhiteSpace(s))
                return default(long[]);

            string[] array = s.Split(new string[] { "," }, StringSplitOptions.None);
            long[] longArray = new long[array.Length];
            for (int i = 0; i < array.Length; i++)
                longArray[i] = Int64.Parse(array[i]);

            return longArray;
        }

        public void PutFloat(string key, float value)
        {
            _values[key] = value;
        }
        public float GetFloat(string key, float defaultValue)
        {
            if (!_values.ContainsKey(key))
                return defaultValue;

            return (long)_values[key];
        }
        public float GetFloat(string key)
        {
            return GetFloat(key, default(float));
        }

        public void PutFloatArray(string key, float[] values)
        {
            _values[key] = String.Join("|", values);
        }
        public float[] GetFloatArray(string key)
        {
            if (!_values.ContainsKey(new NSString(key)))
                return default(float[]);

            string s = _values[key].ToString();
            if (String.IsNullOrWhiteSpace(s))
                return default(float[]);

            string[] array = s.Split(new string[] { "|" }, StringSplitOptions.None);
            float[] floatArray = new float[array.Length];
            for (int i = 0; i < array.Length; i++)
                floatArray[i] = Single.Parse(array[i]);

            return floatArray;
        }

        public void PutDouble(string key, double value)
        {
            _values[key] = value;
        }
        public double GetDouble(string key, double defaultValue)
        {
            if (!_values.ContainsKey(key))
                return defaultValue;

            return (double)_values[key];
        }
        public double GetDouble(string key)
        {
            return GetDouble(key, default(double));
        }

        public void PutDoubleArray(string key, double[] values)
        {
            _values[key] = String.Join("|", values);
        }
        public double[] GetDoubleArray(string key)
        {
            if (!_values.ContainsKey(new NSString(key)))
                return default(double[]);

            string s = _values[key].ToString();
            if (String.IsNullOrWhiteSpace(s))
                return default(double[]);

            string[] array = s.Split(new string[] { "|" }, StringSplitOptions.None);
            double[] doubleArray = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
                doubleArray[i] = Double.Parse(array[i]);

            return doubleArray;
        }

        #endregion

        #region Public Boolean Methods

        public void PutBoolean(string key, bool value)
        {
            _values[key] = value;
        }
        public bool GetBoolean(string key, bool defaultValue)
        {
            if (!_values.ContainsKey(key))
                return defaultValue;

            return (bool)_values[key];
        }
        public bool GetBoolean(string key)
        {
            return GetBoolean(key, default(bool));
        }


        #endregion

        #region Public DateTime Mehtods

        public void PutDateTime(string key, DateTime value)
        {
            _values[key] = value;
        }
        public DateTime GetDateTime(string key, DateTime defaultValue)
        {
            if (!_values.ContainsKey(key))
                return defaultValue;

            return (DateTime)_values[key];
        }
        public DateTime GetDateTime(string key)
        {
            return GetDateTime(key, default(DateTime));
        }

        #endregion

        #region Public Object Methods

        public void PutObject<T>(string key, T obj)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            PutString(key, json);
        }
        public T GetObject<T>(string key, T defaultValue)
        {
            string json = GetString(key, null);
            if (json == null)
                return defaultValue;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
        public T GetObject<T>(string key)
        {
            return GetObject<T>(key, default(T));
        }

        #endregion
    }

}