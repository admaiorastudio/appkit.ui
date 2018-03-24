namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Content;
    using Android.Views;

    public enum FlingDirection
    {
        Unknown,
        FlingUp,
        FlingRight,
        FlingDown,
        FlingLeft        
    }

    public class MotionEventArgs : EventArgs
    {
        public MotionEventArgs(bool handled, MotionEvent ev)
        {
            this.Handled = handled;
            this.Event = ev;
        }

        public MotionEvent Event
        {
            get;
            private set;
        }
        public bool Handled
        {
            get;
            set;
        }
    }

    public class PressEventArgs : EventArgs
    {
        public PressEventArgs(MotionEvent ev)
        {
            this.Event = ev;
        }

        public MotionEvent Event
        {
            get;
            private set;
        }
    }

    public class FlingEventArgs : EventArgs
    {
        public FlingEventArgs(bool handled, MotionEvent e1, MotionEvent e2, float x, float y, FlingDirection direction)
        {
            this.Event1 = e1;
            this.Event2 = e2;
            this.X = x;
            this.Y = y;
            this.Handled = handled;
            this.Direction = direction;
        }

        public MotionEvent Event1
        {
            get;
            private set;
        }

        public MotionEvent Event2
        {
            get;
            private set;
        }

        public float X
        {
            get;
            private set;
        }

        public float Y
        {
            get;
            private set;
        }

        public bool Handled
        {
            get;
            set;
        }

        public FlingDirection Direction
        {
            get;
            set;
        }

    }

    public class GestureListener : GestureDetector.SimpleOnGestureListener, View.IOnTouchListener
    {
        #region Constants and Fields

        private const int SWIPE_THRESHOLD = 100;
        private const int SWIPE_VELOCITY_THRESHOLD = 100;
        
        private  GestureDetector _gestureDetector;

        private Action<MotionEventArgs> WhenDown;
        private Action<MotionEventArgs> WhenSingleTapUp;
        private Action<PressEventArgs> WhenLongPress;
        private Action<PressEventArgs> WhenShowPress;
        private Action<FlingEventArgs> WhenFling;
        private Action<FlingEventArgs> WhenScroll;

        #endregion

        #region Events

        #endregion

        #region Constructors and Destructors

        public GestureListener(Context context)
        {
            _gestureDetector = new GestureDetector(context, this);
        }

        public static GestureListener ForDown(Context context, Action<MotionEventArgs> down) { GestureListener g = new GestureListener(context); g.WhenDown = down; return g; }
        public static GestureListener ForSingleTapUp(Context context, Action<MotionEventArgs> tap) { GestureListener g = new GestureListener(context); g.WhenSingleTapUp = tap; return g; }
        public static GestureListener ForLongPress(Context context, Action<PressEventArgs> longTap) { GestureListener g = new GestureListener(context); g.WhenLongPress = longTap; return g; }
        public static GestureListener ForShowPress(Context context, Action<PressEventArgs> shwoPress) { GestureListener g = new GestureListener(context); g.WhenShowPress = shwoPress; return g; }
        public static GestureListener ForFling(Context context, Action<FlingEventArgs> fling) { GestureListener g = new GestureListener(context); g.WhenFling = fling; return g; }
        public static GestureListener ForScroll(Context context, Action<FlingEventArgs> scroll) { GestureListener g = new GestureListener(context); g.WhenScroll = scroll; return g; }

        #endregion

        #region Methods

        #endregion

        #region View Methods

        public bool OnTouch(View v, MotionEvent e)
        {
            return _gestureDetector.OnTouchEvent(e);  
        }

        #endregion

        #region GestureDetector.SimpleOnGestureListener Methods

        public override bool OnDown(MotionEvent e)

        {
            if (WhenDown != null)
            {
                var args = new MotionEventArgs(false, e);
                WhenDown(args);
                return args.Handled;
            }

            return false || (WhenSingleTapUp != null);
        }

        public override void OnLongPress(MotionEvent e)
        {
            if (WhenLongPress != null)
            {
                var args = new PressEventArgs(e);
                WhenLongPress(args);
            }
        }

        public override void OnShowPress(MotionEvent e)
        {
            if (WhenShowPress != null)
            {
                var args = new PressEventArgs(e);
                WhenShowPress(args);
            }
        }

        public override bool OnSingleTapUp(MotionEvent e)
        {
            if (WhenSingleTapUp != null)
            {
                var args = new MotionEventArgs(false, e);
                WhenSingleTapUp(args);
                return args.Handled;
            }

            return false;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (WhenFling != null)
            {
                FlingDirection direction = FlingDirection.Unknown;
                try
                {
                    float diffY = e2.GetY() - e1.GetY();
                    float diffX = e2.GetX() - e1.GetX();
                    if (Math.Abs(diffX) > Math.Abs(diffY))
                    {
                        if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                        {
                            if (diffX > 0)
                            {
                                direction = FlingDirection.FlingRight;
                            }
                            else
                            {
                                direction = FlingDirection.FlingLeft;
                            }
                        }
                    }
                    else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
                    {
                        if (diffY > 0)
                        {
                            direction = FlingDirection.FlingDown;
                        }
                        else
                        {
                            direction = FlingDirection.FlingUp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Debug("APPKIT", ex.Message);
                    direction = FlingDirection.Unknown;
                }

                var args = new FlingEventArgs(false, e1, e2, velocityX, velocityY, direction);
                WhenFling(args);
                return args.Handled;
            }

            return false;
        }

        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            if (WhenScroll != null)
            {
                var args = new FlingEventArgs(false, e1, e2, distanceX, distanceY, FlingDirection.Unknown);
                WhenScroll(args);
                return args.Handled;
            }
            return false;
        }

        #endregion
    }
}