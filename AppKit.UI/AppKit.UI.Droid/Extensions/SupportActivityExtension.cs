namespace AdMaiora.AppKit.UI
{
    using System;

    using Android;
    using Android.Support.V4.App;
    using Android.Support.V7.App;
    using Android.Support.V7.Widget;

    public static class SupportActivityExtension
    {
        internal static ActionBar GetSupportActionBar(this FragmentActivity activity)
        {            
            return ((AppCompatActivity)activity).SupportActionBar;            
        }

        public static void SetTitle(this FragmentActivity activity, string title)
        {
            if (activity.GetSupportActionBar() != null)
                activity.GetSupportActionBar().Title = title;
        }
    }
}