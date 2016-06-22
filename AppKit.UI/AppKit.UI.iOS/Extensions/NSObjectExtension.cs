namespace AdMaiora.AppKit.UI
{
    using System;
    
    using Foundation;

    public static class NSObjectExtension
    {
        public static void AddObserver(this NSObject fromObject, NSObject observer, string keyPath)
        {
            fromObject.AddObserver(observer, new NSString(keyPath), 0, IntPtr.Zero);
        }

        public static NSObject AddObserver(this NSObject fromObject, string aName, Action<NSNotification> notify)
        {
            return NSNotificationCenter.DefaultCenter.AddObserver(new NSString(aName), notify, fromObject);
        }

        public static void RemoveObserver(this NSObject fromObject, NSObject observer)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
        }
    }
}