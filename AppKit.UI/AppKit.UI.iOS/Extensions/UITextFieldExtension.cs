namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;
    using Foundation;

    public static class UITextFieldExtension
    {
        public static void SetMaxLength(this UITextField text, int maxLength)
        {
            // Some reference
            // http://stackoverflow.com/questions/433337/set-the-maximum-character-length-of-a-uitextfield
            //
            text.ShouldChangeCharacters =
                (textField, range, replacement) =>
                {
                    if (range.Length + range.Location > textField.Text.Length)
                        return false;

                    int newLength = textField.Text.Length + replacement.Length - (int)range.Length;
                    return newLength <= maxLength;
                };
        }    

        public static void SetPlaceholderColor(this UITextField text, UIColor color)
        {
            string t = text.Placeholder ?? String.Empty;
            var astring = new NSMutableAttributedString(t,
                new CoreText.CTStringAttributes()
                {                    
                    Font = new CoreText.CTFont(text.Font.Name, text.Font.PointSize),
                    ForegroundColor = color.CGColor
                });

            astring.AddAttribute(UIStringAttributeKey.ForegroundColor, color, new NSRange(0, t.Length));

            text.AttributedPlaceholder = astring;
        }
    }
}