namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using Android.Graphics.Drawables;
    using Android.Graphics;

    public static class ImageButtonExtension
    {
        public static void SetAutomaticPressedState(this ImageButton button)
        {
            Drawable normalDrawable = button.Background;
            Drawable pressedDrawable = null;
            Drawable disableDrawable = null;

            if (normalDrawable is ColorDrawable)
            {
                Color c = ((normalDrawable) as ColorDrawable).Color;

                pressedDrawable = new ColorDrawable(c);
                pressedDrawable.SetColorFilter(Color.White, PorterDuff.Mode.SrcAtop);

                disableDrawable = new ColorDrawable(c);
                disableDrawable.SetColorFilter(Color.Gray, PorterDuff.Mode.SrcAtop);
            }
            else if (normalDrawable is BitmapDrawable)
            {
                BitmapDrawable bd = normalDrawable as BitmapDrawable;

                pressedDrawable = new BitmapDrawable(bd.Bitmap);
                pressedDrawable.SetColorFilter(new Color(255, 255, 255, 70), PorterDuff.Mode.SrcAtop);

                disableDrawable = new BitmapDrawable(bd.Bitmap);
                disableDrawable.SetColorFilter(Color.Gray, PorterDuff.Mode.SrcAtop);
            }

            StateListDrawable sld = new StateListDrawable();
            sld.AddState(new[] { Android.Resource.Attribute.StatePressed }, pressedDrawable);
            sld.AddState(new[] { -Android.Resource.Attribute.StateEnabled }, disableDrawable);
            sld.AddState(new[] { Android.Resource.Attribute.StateEnabled }, normalDrawable);

            button.Background = sld;

        }
    }
}