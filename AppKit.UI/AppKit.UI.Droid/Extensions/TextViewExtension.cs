namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Widget;
    using Android.Graphics;
    
    public static class TextViewExtension
    {
        public static void SetTextBold(this TextView textView, bool bold)
        {
            textView.SetTypeface(textView.Typeface, bold ? TypefaceStyle.Bold : TypefaceStyle.Normal);
        }

        public static void SetTextUnderline(this TextView textView, bool underline)
        {
            if(underline)
                textView.PaintFlags |= PaintFlags.UnderlineText;
            else
                textView.PaintFlags &= ~PaintFlags.UnderlineText;
        }
    }
}