namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Android.App;
    using Android.Content.Res;
    using Android.Graphics;
    using Android.OS;
    using Android.Views;
    using Android.Widget;

#pragma warning disable 0219, 0649

    class FalseActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.FindViewById(0);                    
        }
    }

    class FalseFragment : Android.Support.V4.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);

            this.View.FindViewById(0);            
        }
    }

#pragma warning restore 0219, 0649

    public class WidgetAttribute : Attribute
    {
        public WidgetAttribute()
        {

        }
    }

    public class LayoutConfigurator<TLayoutParameters>
    {
        #region Constants and Fields

        private TLayoutParameters _layoutParameters;

        #endregion

        #region Constructors and Destructors

        public LayoutConfigurator(TLayoutParameters lp)
        {
            _layoutParameters = lp;
        }

        #endregion

        #region Public Methods

        public LayoutConfigurator<TLayoutParameters> Set(Action<TLayoutParameters> setter)
        {
            setter(_layoutParameters);
            return this;
        }

        public TLayoutParameters GetParameters()
        {
            return _layoutParameters;
        }

        #endregion
    }

    public static class ViewBuilder
    {
        #region Constants and Fields

        private static Dictionary<string, Typeface> _fonts;

        #endregion

        #region Public Methods

        public static Color ColorFromARGB(byte a, byte r, byte g, byte b)
        {
            return new Color(r, g, b, a);
        }

        public static Color ColorFromARGB(string aarrggbb)
        {
            byte a = 255;
            int offset = 0;
            if (aarrggbb.Length == 8)
            {
                a = Byte.Parse(aarrggbb.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                offset = 2;
            }

            byte r = Byte.Parse(aarrggbb.Substring(0 + offset, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = Byte.Parse(aarrggbb.Substring(2 + offset, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = Byte.Parse(aarrggbb.Substring(4 + offset, 2), System.Globalization.NumberStyles.HexNumber);

            return ColorFromARGB(a, r, g, b);
        }

        public static Typeface FontFromAsset(string fontName, bool bold = false)
        {
            if (_fonts == null)
                _fonts = new Dictionary<string, Typeface>();

            fontName = String.Concat(fontName, bold ? "-Bold" : "-Regular");

            string key = fontName;
            if (_fonts.ContainsKey(key))
                return _fonts[key];

            string path = String.Format("fonts/{0}.ttf", fontName);
            Typeface font = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, path);
            _fonts.Add(key, font);

            return font;
        }

        public static void FontForViews(string fontName, View[] views)
        {
            if (views == null || views.Length == 0)
                return;

            foreach (View view in views)
            {
                if (view is TextView)
                {
                    TextView label = view as TextView;
                    label.SetTypeface(ViewBuilder.FontFromAsset(fontName, label.Typeface.IsBold), label.Typeface.Style);
                }
                else if (view is EditText)
                {
                    EditText text = view as EditText;
                    text.SetTypeface(ViewBuilder.FontFromAsset(fontName, text.Typeface.IsBold), text.Typeface.Style);
                }
                else if (view is Button)
                {
                    Button button = view as Button;
                    button.SetTypeface(ViewBuilder.FontFromAsset(fontName, button.Typeface.IsBold), button.Typeface.Style);
                }
            }
        }

        public static View[] GetWidgets(object context, View container = null)
        {
            Type type = context.GetType();
            var fields = type.GetFields(System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.DeclaredOnly);

            if (fields == null || fields.Length == 0)
                return null;
                      
            var getter = type.GetMethods().Where(m => m.Name == "FindViewById" && !m.IsGenericMethod).FirstOrDefault();
            if(getter == null && container == null)
                throw new InvalidOperationException("You can't get widgets out of an Activity a Fragment or a View.");

            List<View> views = new List<View>();

            //bool isWidgetAttributeUsed = false;
            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(WidgetAttribute), false);
                if (attributes == null || attributes.Length == 0)
                    continue;

                //isWidgetAttributeUsed = true;
                
                int id = Android.App.Application.Context.Resources.GetIdentifier(field.Name, "id", Android.App.Application.Context.PackageName);
                View view = container != null ? container.FindViewById(id) : (View)getter.Invoke(context, new object[] { id });
                field.SetValue(context, view);

                views.Add(view);
            }

            //if (!isWidgetAttributeUsed)
            //    throw new InvalidOperationException("Did you miss the 'Widget' attribute in your fields?");

            return views.ToArray();
        }

        #endregion

        #region Platform Specific Methods

        public static int AsPixels(float dp)
        {
            return
                (int)Math.Round(Android.App.Application.Context.Resources.DisplayMetrics.Density * dp);
        }

        #endregion
    }
}