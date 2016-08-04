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

    public class GestureRecogniser : GestureDetector.SimpleOnGestureListener
    {
        #region Constants and Fields

        private const int SWIPE_THRESHOLD = 100;
        private const int SWIPE_VELOCITY_THRESHOLD = 100;

        private EventHandler<View.TouchEventArgs> _eventHandler;
        private  GestureDetector _gestureDetector;

        #endregion

        #region Events

        public event EventHandler<MotionEventArgs> Down;
        public event EventHandler<MotionEventArgs> SingleTapUp;
        public event EventHandler<PressEventArgs> LongPress;
        public event EventHandler<PressEventArgs> ShowPress;
        public event EventHandler<FlingEventArgs> Fling;
        public event EventHandler<FlingEventArgs> Scroll;

        #endregion

        #region Constructors and Destructors

        public GestureRecogniser(Context context)
        {
            _gestureDetector = new GestureDetector(context, this);
            _eventHandler = (sender, e) =>
            {
                _gestureDetector.OnTouchEvent(e.Event);
            };
        }

        public static GestureRecogniser ForDown(Context context, EventHandler<MotionEventArgs> down) { GestureRecogniser g = new GestureRecogniser(context); g.Down += down; return g; }
        public static GestureRecogniser ForSingleTapUp(Context context, EventHandler<MotionEventArgs> tap) { GestureRecogniser g = new GestureRecogniser(context); g.SingleTapUp += tap; return g; }
        public static GestureRecogniser ForLongPress(Context context, EventHandler<PressEventArgs> longTap) { GestureRecogniser g = new GestureRecogniser(context); g.LongPress += longTap; return g; }
        public static GestureRecogniser ForShowPress(Context context, EventHandler<PressEventArgs> shwoPress) { GestureRecogniser g = new GestureRecogniser(context); g.ShowPress += shwoPress; return g; }
        public static GestureRecogniser ForFling(Context context, EventHandler<FlingEventArgs> fling) { GestureRecogniser g = new GestureRecogniser(context); g.Fling += fling; return g; }
        public static GestureRecogniser ForScroll(Context context, EventHandler<FlingEventArgs> scroll) { GestureRecogniser g = new GestureRecogniser(context); g.Scroll += scroll; return g; }

        #endregion

        #region Methods

        public static implicit operator EventHandler<View.TouchEventArgs>(GestureRecogniser g)
        {
            return g._eventHandler;
        }

        #endregion

        #region Event Raising Methods

        public override bool OnDown(MotionEvent e)
        {
            if (Down != null)
            {
                var args = new MotionEventArgs(false, e);
                Down(this, args);
                return args.Handled;
            }
            return false;
        }

        public override void OnLongPress(MotionEvent e)
        {
            if (LongPress != null)
            {
                var args = new PressEventArgs(e);
                LongPress(this, args);
            }
        }

        public override void OnShowPress(MotionEvent e)
        {
            if (ShowPress != null)
            {
                var args = new PressEventArgs(e);
                ShowPress(this, args);
            }
        }

        public override bool OnSingleTapUp(MotionEvent e)
        {
            if (SingleTapUp != null)
            {
                var args = new MotionEventArgs(false, e);
                SingleTapUp(this, args);
                return args.Handled;
            }
            return false;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (Fling != null)
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
                Fling(this, args);
                return args.Handled;
            }

            return false;
        }

        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            if (Scroll != null)
            {
                var args = new FlingEventArgs(false, e1, e2, distanceX, distanceY, FlingDirection.Unknown);
                Scroll(this, args);
                return args.Handled;
            }
            return false;
        }

        #endregion
    }
}