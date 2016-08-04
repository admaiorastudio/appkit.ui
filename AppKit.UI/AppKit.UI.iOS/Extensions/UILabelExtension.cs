namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;
    using CoreGraphics;

    public static class UILabelExtension
    {
        public static void SizeToFitHeight(this UILabel label, float maxHeight = Single.MaxValue)
        {
            var size = UIStringDrawing.StringSize(
                label.Text, label.Font, new CGSize(label.Frame.Size.Width, maxHeight), UILineBreakMode.WordWrap);

            var frame = label.Frame;
            frame.Height = size.Height;
            label.Frame = frame;
        }

        public static void SizeToFit(this UILabel label, float horizontalPadding = 0, float verticalPadding = 0)
        {
            label.SizeToFit();

            var frame = label.Frame;
            frame.Width += 2 * horizontalPadding;
            frame.Height += 2 * verticalPadding;
            label.Frame = frame;
        }

        public static void SizeToFit(this UILabel label, float padding = 0)
        {
            label.SizeToFit(padding, padding);
        }

        public static void SetText(this UILabel label, string text, float horizontalPadding = 0, float verticalPadding = 0)
        {
            label.Text = text;
            label.SizeToFit(horizontalPadding, verticalPadding);
        }

        public static void SetText(this UILabel label, string text, float horizontalPadding = 0, float verticalPadding = 0, bool centerHorizontal = false, bool centerVertical = false)
        {
            label.Text = text;
            label.SizeToFit(horizontalPadding, verticalPadding);

            UIView super = label.Superview;
            if (super == null)
                return;

            CGRect frame = label.Frame;
            if (centerHorizontal)
                frame.X = super.Frame.Width / 2 - frame.Width / 2;
            if (centerVertical)
                frame.Y = super.Frame.Height / 2 - frame.Height / 2;

            label.Frame = frame;
        }

        public static void SetText(this UILabel label, string text, float padding = 0)
        {
            label.Text = text;
            label.SizeToFit(padding);
        }

        public static void SetText(this UILabel label, string text, float padding = 0, bool center = false)
        {
            label.SetText(text, padding, padding, center, center);
        }

        public static void SetTextBold(this UILabel label, bool bold)
        {
            UIFont system = UIFont.SystemFontOfSize(label.Font.PointSize);
            UIFont systemBold = UIFont.BoldSystemFontOfSize(label.Font.PointSize);            

            // Are we using system font?
            if(label.Font.FamilyName == system.FamilyName)
            {
                label.Font = bold ? systemBold : system;
            }
            else
            {
                label.Font = ViewBuilder.FontFromAsset(label.Font.Name, label.Font.PointSize, bold);
            }
        }

    }
}