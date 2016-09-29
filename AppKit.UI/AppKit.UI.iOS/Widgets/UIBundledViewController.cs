namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Foundation;
    using UIKit;

    public class Bundle
    {
        #region Constants and Fields

        private Dictionary<string, object> _values;

        #endregion

        #region Constructors

        public Bundle()
        {
            _values = new Dictionary<string, object>();
        }

        #endregion

        #region Public Methods

        public void PutAll(Bundle bundle)
        {            
            foreach (var kvp in bundle._values)
                _values.Add(kvp.Key, kvp.Value);
        }

        public void PutString(string key, string value)
        {
            _values[key] = value;
        }

        public string GetString(string key)
        {
            if (!_values.ContainsKey(key))
                return default(string);

            return _values[key].ToString();
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

        public void PutInt(string key, int value)
        {
            _values[key] = value;
        }

        public int GetInt(string key)
        {
            if (!_values.ContainsKey(key))
                return default(int);

            return (int)_values[key];
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

        public void PutBoolean(string key, bool value)
        {
            _values[key] = value;
        }

        public bool GetBoolean(string key)
        {
            if (!_values.ContainsKey(key))
                return default(bool);

            return (bool)_values[key];
        }

        #endregion
    }

    public class UIBundledViewController : UIViewController
    {
        #region Constants and Fields

        private Bundle _bundle;

        #endregion

        #region Constructors

        public UIBundledViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle) 
        {
        }

        #endregion

        #region Properties

        public Bundle Arguments
        {
            get
            {
                return _bundle;
            }
            set
            {
                _bundle = value;
            }
        }

        #endregion
    }
}