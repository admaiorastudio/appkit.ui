namespace AdMaiora.AppKit.UI
{
    using System;

    using Android;
    using Android.Support.V4.App;
    using Android.Support.V7.App;
    using Android.Support.V7.Widget;

    public static class SupportFragmentExtension
    {
        public static ActionBar GetActionBar(this Fragment fragment)
        {
            FragmentActivity activity = fragment.Activity;
            if (activity == null)
                return null;

            return ((AppCompatActivity)activity).SupportActionBar;
        }

        public static void SetTitle(this Fragment fragment, string title)
        {
            if (fragment.GetActionBar() != null)
                fragment.GetActionBar().Title = title;
        }

        public static void DismissKeyboard(this Fragment fragment)
        {
            FragmentActivity activity = fragment.Activity;
            if (activity == null)
                return;

            activity.DismissKeyboard();
        }
    }
}