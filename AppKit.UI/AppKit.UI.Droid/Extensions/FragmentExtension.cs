namespace AdMaiora.AppKit.UI
{
    using System;

    using Android;
    using Android.App;
    using Android.Views.InputMethods;
    
    public static class FragmentExtension
    {
        public static void SetActionBarTitle(this Fragment fragment, string title)
        {
            if (fragment.Activity == null)
                return;

            Activity activity = fragment.Activity;
            if (activity.ActionBar != null)
                activity.ActionBar.Title = title;
        }

        public static void DismissKeyboard(this Fragment fragment)
        {
            Activity activity = fragment.Activity;
            if (activity == null)
                return;

            activity.DismissKeyboard();
        }
    }
}