namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;
    using Foundation;
    using CoreFoundation;

    public static class UITextViewExtension
    {
        public static void RequestUserInput(this UITextView text, float delay = 0f)
        {
            var interval = new DispatchTime(CoreFoundation.DispatchTime.Now, (long)delay * 1000000);
            DispatchQueue.MainQueue.DispatchAfter(interval,() => text.BecomeFirstResponder());
        }
    }
}