namespace AdMaiora.AppKit.UI
{
    using System;

    using Android;
    using Android.App;
    using Android.Views;
    using Android.Content;

    public static class LayoutInflaterExtension
    {
        public static View InflateWithWidgets(this LayoutInflater inflater, int resource, object context, ViewGroup container, bool attachToRoot)
        {
            var view = inflater.Inflate(resource, container, attachToRoot);
            ViewBuilder.GetWidgets(context, view);

            return view;
        }
    }
}