namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Widget;
    using Android.Graphics;
    using Android.OS;
    using Android.Views.InputMethods;
    using Android.Content;

    public static class EditTextExtension
    {
        public static void RequestUserInput(this EditText text, float delay = 0f)
        {
            text.RequestFocus();
            (new Handler()).PostDelayed(
                () =>
                {
                    InputMethodManager imm = (InputMethodManager)Android.App.Application.Context.GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(text, ShowFlags.Implicit);

                }, 50 + (long)delay);
        }
    }
}