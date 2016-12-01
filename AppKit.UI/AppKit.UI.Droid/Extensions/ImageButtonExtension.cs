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
            if ((button.Background == null || button.Background.Current == null)
                && button.Drawable != null)
            {
                Drawable normalDrawable = button.Drawable;
                Drawable pressedDrawable = null;
                Drawable disableDrawable = null;

                ColorMatrix matrix = new ColorMatrix();
                matrix.SetSaturation(0);
                ColorMatrixColorFilter filter = new ColorMatrixColorFilter(matrix);

                if (normalDrawable is ColorDrawable)
                {
                    Color c = ((normalDrawable) as ColorDrawable).Color;

                    pressedDrawable = new ColorDrawable(c);
                    pressedDrawable.SetColorFilter(new Color(0, 0, 0, 30), PorterDuff.Mode.SrcAtop);

                    disableDrawable = new ColorDrawable(c);
                    disableDrawable.SetColorFilter(filter);
                }
                else if (normalDrawable is BitmapDrawable)
                {
                    BitmapDrawable bd = normalDrawable as BitmapDrawable;

                    pressedDrawable = new BitmapDrawable(bd.Bitmap);
                    pressedDrawable.SetColorFilter(new Color(0, 0, 0, 30), PorterDuff.Mode.SrcAtop);

                    disableDrawable = new BitmapDrawable(bd.Bitmap);
                    disableDrawable.SetColorFilter(filter);
                }

                StateListDrawable sld = new StateListDrawable();
                sld.AddState(new[] { Android.Resource.Attribute.StatePressed }, pressedDrawable);
                sld.AddState(new[] { -Android.Resource.Attribute.StateEnabled }, disableDrawable);
                sld.AddState(new[] { Android.Resource.Attribute.StateEnabled }, normalDrawable);

                button.SetImageDrawable(sld);
            }
        }
    }
}