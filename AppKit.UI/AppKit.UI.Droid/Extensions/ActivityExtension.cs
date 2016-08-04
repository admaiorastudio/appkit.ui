namespace AdMaiora.AppKit.UI
{
    using System;

    using Android;
    using Android.App;
    using Android.Views.InputMethods;

    public interface IBackButton
    {
        bool OnBackButton();
    }

    public static class ActivityExtension
    {
        public static void SetActionBarTitle(this Activity activity, string title)
        {
            if (activity.ActionBar != null)
                activity.ActionBar.Title = title;
        }       

        public static void DismissKeyboard(this Activity activity)
        {
            var view = activity.CurrentFocus;
            if (view == null)
                return;

            InputMethodManager imm = (InputMethodManager)activity.GetSystemService(Activity.InputMethodService);
            imm.HideSoftInputFromWindow(view.WindowToken, 0);

            if (activity.Window.DecorView != null)
                activity.Window.DecorView.ClearFocus();
        }
    }
}