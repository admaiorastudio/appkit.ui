namespace AdMaiora.AppKit.UI
{
    using System;

    using Android;
    using Android.App;
    using Android.Content;
    using Android.Content.Res;
    using Android.Widget;  
    
    public static class ImageViewExtension
    {
        public static void SetImageResource(this ImageView imageView, string resource)
        {
            int id = imageView.Context.Resources.GetIdentifier(resource, "drawable", imageView.Context.PackageName);
            imageView.SetImageResource(id);
        }
    }
}