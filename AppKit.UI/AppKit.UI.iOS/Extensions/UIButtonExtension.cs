namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Foundation;
    using UIKit;

    public static class UIButtonExtension
    {
        public static void SetAttributedTitle(this UIButton button, string title, UIControlState state)
        {
            NSMutableAttributedString ms = new NSMutableAttributedString(button.GetAttributedTitle(UIControlState.Normal));
            ms.Replace(new NSRange(0, ms.Length), title);
            button.SetAttributedTitle(ms, state);
        }
    }
}