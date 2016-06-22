namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;

    using Android.App;
    using Android.Content.Res;
    using Android.Graphics;
    using Android.Views;
    using Android.Widget;

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

        #region Properties

        public static ColorStateList ClickableColorSateList
        {
            get
            {
                return new ColorStateList
                    (
                        new int[][]
                    {
                        new int[] { Android.Resource.Attribute.StatePressed },
                        new int[] { Android.Resource.Attribute.StateFocused },
                        new int[] { 0 }
                    },
                        new int[]
                    {
                        Color.Gray,
                        Color.Gray,
                        Color.White
                    }
                    );
            }
        }

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

        public static int AsPixels(float dp)
        {
            return
                (int)Math.Round(Android.App.Application.Context.Resources.DisplayMetrics.Density * dp);
        }

        public static LayoutConfigurator<TLayoutParameters> ConfigureLayout<TLayoutParameters>()
        {
            Type requestedType = typeof(TLayoutParameters);
            object lp = null;

            if (requestedType == typeof(LinearLayout.LayoutParams))
            {
                lp = new LinearLayout
                    .LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            }
            else if (requestedType == typeof(RelativeLayout.LayoutParams))
            {
                lp = new RelativeLayout
                    .LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            }
            else if (requestedType == typeof(AbsListView.LayoutParams))
            {
                lp = new AbsListView
                    .LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            }
            else if (requestedType == typeof(FrameLayout.LayoutParams))
            {
                lp = new FrameLayout
                    .LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            }
            else if (requestedType == typeof(ImageSwitcher.LayoutParams))
            {
                lp = new ImageSwitcher
                    .LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            }
            else
            {
                // Unsopported layout.
                throw new InvalidOperationException(
                    String.Format("Unsopported layout type '{0}'. Unable to configure.", requestedType.Name));
            }

            LayoutConfigurator<TLayoutParameters> configurator = new LayoutConfigurator<TLayoutParameters>((TLayoutParameters)lp);

            return configurator;
        }

        public static View[] GetWidgets(object context, Func<int, View> getter)
        {
            Type type = context.GetType();
            var fields = type.GetFields(System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.DeclaredOnly);

            if (fields == null || fields.Length == 0)
                return null;

            List<View> views = new List<View>();

            bool isWidgetAttributeUsed = false;
            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(WidgetAttribute), false);
                if (attributes == null || attributes.Length == 0)
                    continue;

                isWidgetAttributeUsed = true;

                Type fieldType = field.FieldType;
                int id = Application.Context.Resources.GetIdentifier(field.Name, "id", Application.Context.PackageName);
                View view = getter(id);
                field.SetValue(context, view);

                views.Add(view);
            }

            if (!isWidgetAttributeUsed)
                throw new InvalidOperationException("Did you miss the 'Widget' attribute in your fields?");

            return views.ToArray();
        }

        #endregion
    }
}