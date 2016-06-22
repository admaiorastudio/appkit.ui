namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;
    using Foundation;

    public static class UINavigationItemExtension
    {   
        public static void SetTitleWithEllipsis(this UINavigationItem item, string text, int maxLenght = 10)
        {       
            if (text.Length > maxLenght)
            {
                if (text.Contains(" "))
                    text = text.Split(' ')[0];

                text = String.Concat(text.Length > maxLenght ? text.Substring(0, maxLenght) : text, Char.ConvertFromUtf32(8230));
            }

            item.Title = text;               
        }
    }
}

