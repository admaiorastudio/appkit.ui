namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Foundation;
    using UIKit;
    using CoreText;

    public static class UIButtonExtension
    {
        public static void SetAttributedTitle(this UIButton button, string title, UIControlState state)
        {
            NSMutableAttributedString ms = new NSMutableAttributedString(button.GetAttributedTitle(UIControlState.Normal));
            ms.Replace(new NSRange(0, ms.Length), title);
            button.SetAttributedTitle(ms, state);
        }

        public static void SetTextUnderline(this UIButton button, bool underline)
        {
            string fontName = button.Font.Name;
            string text = button.CurrentTitle ?? String.Empty;

            var astring = new NSMutableAttributedString(text ?? String.Empty,
                new CoreText.CTStringAttributes()
                {
                    UnderlineStyle = CTUnderlineStyle.Single,
                    Font = new CTFont(fontName, 13f)
                });

            astring.AddAttribute(UIStringAttributeKey.ForegroundColor,
                button.TitleLabel.TextColor, new NSRange(0, text.Length));

            button.TitleLabel.AttributedText = astring;
        }
    }
}