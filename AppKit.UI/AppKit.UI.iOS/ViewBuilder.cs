namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;

    using Foundation;
    using UIKit;
    using CoreGraphics;

    public class WidgetAttribute : Attribute
    {
        public WidgetAttribute()
        {

        }
    }

    public static class ViewBuilder
    {
        #region Constants and Fields

        private static Dictionary<string, UIFont> _fonts;

        #endregion

        #region Public Methods

        public static UIColor ColorFromARGB(byte a, byte r, byte g, byte b)
        {
            return UIColor.FromRGBA(r, g, b, a);
        }

        public static UIColor ColorFromARGB(string aarrggbb)
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

        public static UIFont FontFromAsset(string fontName, nfloat size)
        {
            if (_fonts == null)
                _fonts = new Dictionary<string, UIFont>();

            string key = String.Concat(fontName, "-", size);
            if (_fonts.ContainsKey(key))
                return _fonts[key];

            UIFont font = UIFont.FromName(fontName, size);
            _fonts.Add(key, font);

            return font;
        }

        public static void FontForViews(string fontName, UIView[] views)
        {
            if (views == null || views.Length == 0)
                return;

            foreach (UIView view in views)
            {
                if (view is UILabel)
                {
                    UILabel label = view as UILabel;
                    label.Font = ViewBuilder.FontFromAsset(fontName, label.Font.PointSize);
                }
                else if (view is UITextField)
                {
                    UITextField text = view as UITextField;
                    text.Font = ViewBuilder.FontFromAsset(fontName, text.Font.PointSize);
                }
                else if (view is UIButton)
                {
                    UIButton button = view as UIButton;
                    button.Font = ViewBuilder.FontFromAsset(fontName, button.Font.PointSize);
                }
            }
        }

        public static UIView[] GetWidgets(object context, UIView container = null)
        {
            Type type = context.GetType();
            var fields = type.GetFields(System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.DeclaredOnly);

            if (fields == null || fields.Length == 0)
                return null;

            if ((context as UIViewController) == null && container == null)
                throw new InvalidOperationException("You can't get widgets out of a UIViewControllore or a UIView");            

            List<UIView> views = new List<UIView>();

            //bool isWidgetAttributeUsed = false;
            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(WidgetAttribute), false);
                if (attributes == null || attributes.Length == 0)
                    continue;

                //isWidgetAttributeUsed = true;

                string id = field.Name;
                UIView view = (container != null) ? container.FindViewById<UIView>(id) : ((UIViewController)context).View.FindViewById<UIView>(id);
                field.SetValue(context, view);

                views.Add(view);
            }

            //if (!isWidgetAttributeUsed)
            //    throw new InvalidOperationException("Did you miss the 'Outlet' attribute in your fields?");

            return views.ToArray();
        }

        #endregion

        #region Platform Specific Methods

        public static UIImage MonochromatizeImage(UIImage source, UIColor color)
        {
            UIGraphics.BeginImageContextWithOptions(source.Size, false, 0);
            CGContext context = UIGraphics.GetCurrentContext();
            context.SetFillColor(color.CGColor);
            context.TranslateCTM(0, source.Size.Height);
            context.ScaleCTM(1, -1);
            context.ClipToMask(new CGRect(0, 0, source.Size.Width, source.Size.Height), source.CGImage);
            context.FillRect(new CGRect(0, 0, source.Size.Width, source.Size.Height));
            UIImage result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return result;
        }

        public static UIImage CreateBackgroundFromColor(UIColor color)
        {
            CGRect rect = new CGRect(0.0f, 0.0f, 1.0f, 1.0f);
            UIGraphics.BeginImageContext(rect.Size);
            CGContext context = UIGraphics.GetCurrentContext();

            context.SetFillColor(color.CGColor);
            context.FillRect(rect);

            UIImage image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
        }

        public static UIImage CreateBackgroundFromColor(string aarrggbb)
        {
            return CreateBackgroundFromColor(ViewBuilder.ColorFromARGB(aarrggbb));
        }

        #endregion
    }
}