namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;

    public static class UIBarButtonItemExtension
    {   
        public static void SetTitleWithEllipsis(this UIBarButtonItem button, string text, int maxLenght = 10)
        {       
            if (text.Length > maxLenght)
            {
                if (text.Contains(" "))
                    text = text.Split(' ')[0];

                text = String.Concat(text.Length > maxLenght ? text.Substring(0, maxLenght) : text, Char.ConvertFromUtf32(8230));
            }

            button.Title = text;               
        }
    }
}

