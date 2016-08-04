namespace AdMaiora.AppKit.UI
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Linq;

    using Android.Content.Res;
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Util;
    using Android.Widget;

    using Java.Lang;

    internal class RoundedDrawable : Drawable
    {
        private const string TAG = "RoundedDrawable";
        public static readonly Color DEFAULT_BORDER_COLOR = Color.Black;

        private readonly RectF _bounds = new RectF();
        private readonly RectF _drawableRect = new RectF();
        private readonly RectF _bitMapRect = new RectF();
        private readonly Bitmap _bitmap;
        private readonly Paint _bitmapPaint;
        private readonly int _bitmapWidth;
        private readonly int _bitmapHeight;
        private readonly RectF _borderRect = new RectF();
        private readonly Paint _borderPaint;
        private readonly Matrix _shaderMatrix = new Matrix();
        private readonly RectF _squareCornersRect = new RectF();

        private Shader.TileMode _tileModeX = Shader.TileMode.Clamp;
        private Shader.TileMode _tileModeY = Shader.TileMode.Clamp;
        private bool _rebuildShader = true;

        // [ topLeft, topRight, bottomLeft, bottomRight ]
        private float _cornerRadius = 0f;
        private readonly bool[] _cornersRounded = new bool[] { true, true, true, true };

        private bool _oval = false;
        private float _borderWidth = 0;
        private ColorStateList _borderColor = ColorStateList.ValueOf(DEFAULT_BORDER_COLOR);
        private ImageView.ScaleType _scaleType = ImageView.ScaleType.FitCenter;

        public RoundedDrawable(Bitmap bitmap)
        {
            _bitmap = bitmap;

            _bitmapWidth = bitmap.Width;
            _bitmapHeight = bitmap.Height;
            _bitMapRect.Set(0, 0, _bitmapWidth, _bitmapHeight);

            _bitmapPaint = new Paint();
            _bitmapPaint.SetStyle(Paint.Style.Fill);
            _bitmapPaint.AntiAlias = true;

            _borderPaint = new Paint();
            _borderPaint.SetStyle(Paint.Style.Stroke);
            _borderPaint.AntiAlias = true;
            _borderPaint.Color = new Color(_borderColor.GetColorForState(GetState(), DEFAULT_BORDER_COLOR));
            _borderPaint.StrokeWidth = _borderWidth;
        }

        public static RoundedDrawable fromBitmap(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                return new RoundedDrawable(bitmap);
            }
            else
            {
                return null;
            }
        }

        public static Drawable fromDrawable(Drawable drawable)
        {
            if (drawable != null)
            {
                if (drawable is RoundedDrawable)
                {
                    // just return if it's already a RoundedDrawable
                    return drawable;
                }
                else if (drawable is LayerDrawable)
                {
                    var ld = (LayerDrawable)drawable;
                    int num = ld.NumberOfLayers;

                    // loop through layers to and change to RoundedDrawables if possible
                    for (int i = 0; i < num; i++)
                    {
                        var d = ld.GetDrawable(i);
                        ld.SetDrawableByLayerId(ld.GetId(i), fromDrawable(d));
                    }
                    return ld;
                }

                // try to get a bitmap from the drawable and
                Bitmap bm = drawableToBitmap(drawable);
                if (bm != null)
                {
                    return new RoundedDrawable(bm);
                }
            }
            return drawable;
        }

        public static Bitmap drawableToBitmap(Drawable drawable)
        {
            if (drawable is BitmapDrawable)
            {
                return (drawable as BitmapDrawable).Bitmap;
            }

            Bitmap bitmap;
            int width = Math.Max(drawable.IntrinsicWidth, 2);
            int height = Math.Max(drawable.IntrinsicHeight, 2);
            try
            {
                bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                drawable.Draw(canvas);
            }
            catch (Java.Lang.Throwable e)
            {
                e.PrintStackTrace();
                Log.Warn(TAG, "Failed to create bitmap from drawable!");
                bitmap = null;
            }

            return bitmap;
        }

        public Bitmap getSourceBitmap()
        {
            return _bitmap;
        }

        public override bool IsStateful
        {
            get
            {
                return _borderColor.IsStateful;
            }
        }

        protected override bool OnStateChange(int[] state)
        {
            var newColor = _borderColor.GetColorForState(state, Color.Black);
            if (_borderPaint.Color != newColor)
            {
                _borderPaint.Color = new Color(newColor);
                return true;
            }
            else
            {
                return base.OnStateChange(state);
            }
        }

        private void updateShaderMatrix()
        {
            float scale;
            float dx;
            float dy;

            if (_scaleType == ImageView.ScaleType.Center)
            {
                _borderRect.Set(_bounds);
                _borderRect.Inset(_borderWidth / 2, _borderWidth / 2);

                _shaderMatrix.Reset();
                _shaderMatrix.SetTranslate((int)((_borderRect.Width() - _bitmapWidth) * 0.5f + 0.5f),
                    (int)((_borderRect.Height() - _bitmapHeight) * 0.5f + 0.5f));

            }
            else if (_scaleType == ImageView.ScaleType.CenterCrop)
            {
                _borderRect.Set(_bounds);
                _borderRect.Inset(_borderWidth / 2, _borderWidth / 2);

                _shaderMatrix.Reset();

                dx = 0;
                dy = 0;

                if (_bitmapWidth * _borderRect.Height() > _borderRect.Width() * _bitmapHeight)
                {
                    scale = _borderRect.Height() / (float)_bitmapHeight;
                    dx = (_borderRect.Width() - _bitmapWidth * scale) * 0.5f;
                }
                else
                {
                    scale = _borderRect.Width() / (float)_bitmapWidth;
                    dy = (_borderRect.Height() - _bitmapHeight * scale) * 0.5f;
                }

                _shaderMatrix.SetScale(scale, scale);
                _shaderMatrix.PostTranslate((int)(dx + 0.5f) + _borderWidth / 2,
                    (int)(dy + 0.5f) + _borderWidth / 2);
            }
            else if (_scaleType == ImageView.ScaleType.CenterInside)
            {

                _shaderMatrix.Reset();

                if (_bitmapWidth <= _bounds.Width() && _bitmapHeight <= _bounds.Height())
                {
                    scale = 1.0f;
                }
                else
                {
                    scale = Math.Min(_bounds.Width() / (float)_bitmapWidth, _bounds.Height() / (float)_bitmapHeight);
                }

                dx = (int)((_bounds.Width() - _bitmapWidth * scale) * 0.5f + 0.5f);
                dy = (int)((_bounds.Height() - _bitmapHeight * scale) * 0.5f + 0.5f);

                _shaderMatrix.SetScale(scale, scale);
                _shaderMatrix.PostTranslate(dx, dy);

                _borderRect.Set(_bitMapRect);
                _shaderMatrix.MapRect(_borderRect);
                _borderRect.Inset(_borderWidth / 2, _borderWidth / 2);
                _shaderMatrix.SetRectToRect(_bitMapRect, _borderRect, Matrix.ScaleToFit.Fill);
            }
            else if (_scaleType == ImageView.ScaleType.FitCenter)
            {

                _borderRect.Set(_bitMapRect);
                _shaderMatrix.SetRectToRect(_bitMapRect, _bounds, Matrix.ScaleToFit.Center);
                _shaderMatrix.MapRect(_borderRect);
                _borderRect.Inset(_borderWidth / 2, _borderWidth / 2);
                _shaderMatrix.SetRectToRect(_bitMapRect, _borderRect, Matrix.ScaleToFit.Fill);
            }
            else if (_scaleType == ImageView.ScaleType.FitEnd)
            {
                _borderRect.Set(_bitMapRect);
                _shaderMatrix.SetRectToRect(_bitMapRect, _bounds, Matrix.ScaleToFit.End);
                _shaderMatrix.MapRect(_borderRect);
                _borderRect.Inset(_borderWidth / 2, _borderWidth / 2);
                _shaderMatrix.SetRectToRect(_bitMapRect, _borderRect, Matrix.ScaleToFit.Fill);
            }
            else if (_scaleType == ImageView.ScaleType.FitStart)
            {
                _borderRect.Set(_bitMapRect);
                _shaderMatrix.SetRectToRect(_bitMapRect, _bounds, Matrix.ScaleToFit.Start);
                _shaderMatrix.MapRect(_borderRect);
                _borderRect.Inset(_borderWidth / 2, _borderWidth / 2);
                _shaderMatrix.SetRectToRect(_bitMapRect, _borderRect, Matrix.ScaleToFit.Fill);
            }
            else if (_scaleType == ImageView.ScaleType.FitXy)
            {

                _borderRect.Set(_bounds);
                _borderRect.Inset(_borderWidth / 2, _borderWidth / 2);
                _shaderMatrix.Reset();
                _shaderMatrix.SetRectToRect(_bitMapRect, _borderRect, Matrix.ScaleToFit.Fill);
            }

            _drawableRect.Set(_borderRect);
        }

        protected override void OnBoundsChange(Rect bounds)
        {
            base.OnBoundsChange(bounds);

            _bounds.Set(bounds);

            updateShaderMatrix();
        }

        public override void Draw(Canvas canvas)
        {
            if (_rebuildShader)
            {
                var bitmapShader = new BitmapShader(_bitmap, _tileModeX, _tileModeY);
                if (_tileModeX == Shader.TileMode.Clamp && _tileModeY == Shader.TileMode.Clamp)
                {
                    bitmapShader.SetLocalMatrix(_shaderMatrix);
                }
                _bitmapPaint.SetShader(bitmapShader);
                _rebuildShader = false;
            }

            if (_oval)
            {
                if (_borderWidth > 0)
                {
                    canvas.DrawOval(_drawableRect, _bitmapPaint);
                    canvas.DrawOval(_borderRect, _borderPaint);
                }
                else
                {
                    canvas.DrawOval(_drawableRect, _bitmapPaint);
                }
            }
            else
            {
                if (any(_cornersRounded))
                {
                    float radius = _cornerRadius;
                    if (_borderWidth > 0)
                    {
                        canvas.DrawRoundRect(_drawableRect, radius, radius, _bitmapPaint);
                        canvas.DrawRoundRect(_borderRect, radius, radius, _borderPaint);
                        redrawBitmapForSquareCorners(canvas);
                        redrawBorderForSquareCorners(canvas);
                    }
                    else
                    {
                        canvas.DrawRoundRect(_drawableRect, radius, radius, _bitmapPaint);
                        redrawBitmapForSquareCorners(canvas);
                    }
                }
                else
                {
                    canvas.DrawRect(_drawableRect, _bitmapPaint);
                    if (_borderWidth > 0)
                    {
                        canvas.DrawRect(_borderRect, _borderPaint);
                    }
                }
            }
        }

        private void redrawBitmapForSquareCorners(Canvas canvas)
        {
            if (all(_cornersRounded))
            {
                // no square corners
                return;
            }

            if (_cornerRadius == 0)
            {
                return; // no round corners
            }

            float left = _drawableRect.Left;
            float top = _drawableRect.Top;
            float right = left + _drawableRect.Width();
            float bottom = top + _drawableRect.Height();
            float radius = _cornerRadius;

            if (!_cornersRounded[Corner.TopLeft])
            {
                _squareCornersRect.Set(left, top, left + radius, top + radius);
                canvas.DrawRect(_squareCornersRect, _bitmapPaint);
            }

            if (!_cornersRounded[Corner.TopRight])
            {
                _squareCornersRect.Set(right - radius, top, right, radius);
                canvas.DrawRect(_squareCornersRect, _bitmapPaint);
            }

            if (!_cornersRounded[Corner.BottomRight])
            {
                _squareCornersRect.Set(right - radius, bottom - radius, right, bottom);
                canvas.DrawRect(_squareCornersRect, _bitmapPaint);
            }

            if (!_cornersRounded[Corner.BottomLeft])
            {
                _squareCornersRect.Set(left, bottom - radius, left + radius, bottom);
                canvas.DrawRect(_squareCornersRect, _bitmapPaint);
            }
        }

        private void redrawBorderForSquareCorners(Canvas canvas)
        {
            if (all(_cornersRounded))
            {
                // no square corners
                return;
            }

            if (_cornerRadius == 0)
            {
                return; // no round corners
            }

            float left = _drawableRect.Left;
            float top = _drawableRect.Top;
            float right = left + _drawableRect.Width();
            float bottom = top + _drawableRect.Height();
            float radius = _cornerRadius;
            float offset = _borderWidth / 2;

            if (!_cornersRounded[Corner.TopLeft])
            {
                canvas.DrawLine(left - offset, top, left + radius, top, _borderPaint);
                canvas.DrawLine(left, top - offset, left, top + radius, _borderPaint);
            }

            if (!_cornersRounded[Corner.TopRight])
            {
                canvas.DrawLine(right - radius - offset, top, right, top, _borderPaint);
                canvas.DrawLine(right, top - offset, right, top + radius, _borderPaint);
            }

            if (!_cornersRounded[Corner.BottomRight])
            {
                canvas.DrawLine(right - radius - offset, bottom, right + offset, bottom, _borderPaint);
                canvas.DrawLine(right, bottom - radius, right, bottom, _borderPaint);
            }

            if (!_cornersRounded[Corner.BottomLeft])
            {
                canvas.DrawLine(left - offset, bottom, left + radius, bottom, _borderPaint);
                canvas.DrawLine(left, bottom - radius, left, bottom, _borderPaint);
            }
        }

        public override int Opacity
        {
            get
            {
                return 0;
            }
        }

        public override int Alpha
        {
            get
            {
                return _bitmapPaint.Alpha;
            }

            set
            {
                SetAlpha(value);
            }
        }

        public override void SetAlpha(int alpha)
        {
            _bitmapPaint.Alpha = alpha;
            InvalidateSelf();
        }

        public override ColorFilter ColorFilter
        {
            get
            {
                return _bitmapPaint.ColorFilter;
            }
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
            _bitmapPaint.SetColorFilter(colorFilter);
        }

        [System.Obsolete]
        public override void SetDither(bool dither)
        {
            _bitmapPaint.Dither = dither;
            InvalidateSelf();
        }

        public override void SetFilterBitmap(bool filter)
        {
            _bitmapPaint.FilterBitmap = filter;
            InvalidateSelf();
        }

        public override int IntrinsicWidth
        {
            get
            {
                return _bitmapWidth;
            }
        }

        public override int IntrinsicHeight
        {
            get
            {
                return _bitmapHeight;
            }
        }

        /**
         * @return the corner radius.
         */
        public float CornerRadius
        {
            get
            {
                return _cornerRadius;
            }
        }

        /**
         * @param corner the specific corner to get radius of.
         * @return the corner radius of the specified corner.
         */
        internal float getCornerRadius(int corner)
        {
            return _cornersRounded[corner] ? _cornerRadius : 0f;
        }

        /**
         * Sets all corners to the specified radius.
         *
         * @param radius the radius.
         * @return the {@link RoundedDrawable} for chaining.
         */
        public RoundedDrawable setCornerRadius(float radius)
        {
            setCornerRadius(radius, radius, radius, radius);
            return this;
        }

        /**
         * Sets the corner radius of one specific corner.
         *
         * @param corner the corner.
         * @param radius the radius.
         * @return the {@link RoundedDrawable} for chaining.
         */
        internal RoundedDrawable setCornerRadius(int corner, float radius)
        {
            if (radius != 0 && _cornerRadius != 0 && _cornerRadius != radius)
            {
                throw new IllegalArgumentException("Multiple nonzero corner radii not yet supported.");
            }

            if (radius == 0)
            {
                if (only(corner, _cornersRounded))
                {
                    _cornerRadius = 0;
                }
                _cornersRounded[corner] = false;
            }
            else
            {
                if (_cornerRadius == 0)
                {
                    _cornerRadius = radius;
                }
                _cornersRounded[corner] = true;
            }

            return this;
        }

        /**
         * Sets the corner radii of all the corners.
         *
         * @param topLeft top left corner radius.
         * @param topRight top right corner radius
         * @param bottomRight bototm right corner radius.
         * @param bottomLeft bottom left corner radius.
         * @return the {@link RoundedDrawable} for chaining.
         */
        public RoundedDrawable setCornerRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            var radiusSet = new HashSet<float>();
            radiusSet.Add(topLeft);
            radiusSet.Add(topRight);
            radiusSet.Add(bottomRight);
            radiusSet.Add(bottomLeft);

            radiusSet.Remove(0f);

            if (radiusSet.Count > 1)
            {
                throw new IllegalArgumentException("Multiple nonzero corner radii not yet supported.");
            }

            if (radiusSet.Any())
            {
                var radius = radiusSet.First();
                if (Float.InvokeIsInfinite(radius) || Float.InvokeIsNaN(radius) || radius < 0)
                {
                    throw new IllegalArgumentException("Invalid radius value: " + radius);
                }

                _cornerRadius = radius;
            }
            else
            {
                _cornerRadius = 0f;
            }

            _cornersRounded[Corner.TopLeft] = topLeft > 0;
            _cornersRounded[Corner.TopRight] = topRight > 0;
            _cornersRounded[Corner.BottomLeft] = bottomLeft > 0;
            _cornersRounded[Corner.BottomRight] = bottomRight > 0;
            return this;
        }

        public float BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
                _borderPaint.StrokeWidth = _borderWidth;
            }
        }
        
        public int BorderColor
        {
            get
            {
                return _borderColor.DefaultColor;
            }
            set
            {
                BorderColors = ColorStateList.ValueOf(new Color(value));
            }
        }

        public ColorStateList BorderColors
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value != null ? value : ColorStateList.ValueOf(Color.Black);
                _borderPaint.Color = new Color(_borderColor.GetColorForState(GetState(), DEFAULT_BORDER_COLOR));
            }
        }

        public bool IsOval
        {
            get
            {
                return _oval;
            }
            set
            {
                _oval = value;
            }
        }

        public ImageView.ScaleType ScaleType
        {
            get
            {
                return _scaleType;
            }
            set
            {
                var scaleType = value ?? ImageView.ScaleType.FitCenter;
                
                if (_scaleType != scaleType)
                {
                    _scaleType = scaleType;
                    updateShaderMatrix();
                }
            }
        }

        public Shader.TileMode TileModeX
        {
            get
            {
                return _tileModeX;
            }
            set
            {
                if (_tileModeX != value)
                {
                    _tileModeX = value;
                    _rebuildShader = true;
                    InvalidateSelf();
                }
            }
        }

        public Shader.TileMode TileModeY
        {
            get
            {
                return _tileModeY;
            }
            set
            {
                if (_tileModeY != value)
                {
                    _tileModeY = value;
                    _rebuildShader = true;
                    InvalidateSelf();
                }
            }
        }

        private static bool only(int index, bool[] booleans)
        {
            for (int i = 0, len = booleans.Length; i < len; i++)
            {
                if (booleans[i] != (i == index))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool any(bool[] booleans)
        {
            foreach (var b in booleans)
            {
                if (b) { return true; }
            }
            return false;
        }

        private static bool all(bool[] booleans)
        {
            foreach (var b in booleans)
            {
                if (b) { return false; }
            }
            return true;
        }

        public Bitmap toBitmap()
        {
            return drawableToBitmap(this);
        }
    }
}
