namespace AdMaiora.AppKit.UI
{
    using System;

    using CoreGraphics;
    using Foundation;
    using UIKit;

    [Register("UIDrawerPanel")]
    public class UIDrawerPanel : UIView
    {
        #region Constants and Fields

        protected nfloat _startX;

        protected bool _isOpened;

        private UIView _overlay;

        private bool _isAtLeft;
		protected bool _draggable;

        #endregion

        #region Events

        public event EventHandler DrawerOpened;
        public event EventHandler DrawerClosed;

        #endregion

        #region Constructors and Destructors

        public UIDrawerPanel(CGRect frame)
            : base(frame)
        {
            Initialize();
        }

        public UIDrawerPanel(IntPtr handle)
            : base(handle)
        {
            Initialize();
		}

        public UIDrawerPanel(CGRect frame, bool isAtLeft)
			: base(frame)
		{
			_isAtLeft = isAtLeft;

			Initialize();
		}

		public UIDrawerPanel(IntPtr handle, bool isAtLeft)
			: base(handle)
		{
			_isAtLeft = isAtLeft;

			Initialize();
		}

        #endregion

        #region Properties

        public bool IsOpened
        {
            get
            {
                return _isOpened;
            }
        }

		public bool Draggable
		{
			get{ return _draggable; }
			set{ _draggable = value; }
		}

        public bool IsAtLeft
        {
            get { return _isAtLeft; }
            set { _isAtLeft = value; }
        }

        #endregion

        #region Public Methods

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var frame = this.Frame;
			frame.X = _isAtLeft ? -frame.Width : 320;
            this.Frame = frame;

            this.Hidden = true;

            _overlay.Frame = this.Superview.Bounds;
            _overlay.AddGestureRecognizer(new UITapGestureRecognizer(() => CloseDrawer()));

            this.Superview.InsertSubviewBelow(_overlay, this);
            this.Superview.AddGestureRecognizer(new UIPanGestureRecognizer(DragContentView));
        }

        public void OpenDrawer()
        {
            _overlay.Hidden = false;
            _overlay.Alpha = _isOpened ? .8f : 0f;
            _overlay.UserInteractionEnabled = true;

            InvokeOnMainThread(() =>
            {
                this.Hidden = false;
                UIView.Animate(.2f, 
                () =>
                {
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseIn);

                    var frame = this.Frame;
					frame.X = _isAtLeft ? 0 : 320 - frame.Width;
                    this.Frame = frame;

                    _overlay.Alpha = .8f;
                },
                () =>
                {
                    _isOpened = true;

                    OnDrawerOpened();                   
                });
            });
        }

        public void CloseDrawer()
        {
            _overlay.UserInteractionEnabled = false;

            InvokeOnMainThread(() =>
            {
                this.Hidden = false;
                UIView.Animate(.2f, 
                () =>
                {
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseIn);

                    var frame = this.Frame;
					frame.X = _isAtLeft ? -frame.Width : 320;
                    this.Frame = frame;

                    _overlay.Alpha = 0f;
                },
                () =>
                {
                    _isOpened = false;

                    _overlay.Hidden = true;

                    this.Hidden = true;

                    OnDrawerClosed();
                });
            });
        }

        public void ToggleDrawer()
        {
            if (!_isOpened)
            {
                OpenDrawer();
            }
            else
            {
                CloseDrawer();
            }
        }

        #endregion

        #region Event Raising Methods

        protected void OnDrawerOpened()
        {
            if (DrawerOpened != null)
                DrawerOpened(this, EventArgs.Empty);
        }

        protected void OnDrawerClosed()
        {
            if (DrawerClosed != null)
                DrawerClosed(this, EventArgs.Empty);
        }

        #endregion

        #region Methods
            
        private void Initialize()
        {
            _isOpened = false;

            _overlay = new UIView();
            _overlay.BackgroundColor = UIColor.Black;
            _overlay.Alpha = 0f;
            _overlay.Hidden = true;
            _overlay.UserInteractionEnabled = false;

            this.ClipsToBounds = true;
        }

        private void DragContentView(UIPanGestureRecognizer panGesture)
        {
			if (!Draggable)
				return;
			
            CGRect frame = this.Frame;
            nfloat menuWidth = frame.Width;

            nfloat translation = panGesture.TranslationInView(this).X;

            if (panGesture.State == UIGestureRecognizerState.Began)
            {
                _startX = frame.X;

                this.Hidden = false;
            }
            else if (panGesture.State == UIGestureRecognizerState.Changed)
            {
                frame.X = _startX + translation;

				if (_isAtLeft)
				{
					if (frame.X < -frame.Width)
						frame.X = -frame.Width;
					if (frame.X > 0)
						frame.X = 0;
				}
				else
				{
					if (frame.X > 320)
						frame.X = 320;
					if (frame.X < 320 -frame.Width)
						frame.X = 320 -frame.Width;
				}

                this.Frame = frame;
            }
            else if (panGesture.State == UIGestureRecognizerState.Ended)
            {
                nfloat velocity = panGesture.VelocityInView(this).X;
                nfloat newX = translation + _startX;

				bool show;
				if (_isAtLeft)
					show = this.Frame.X > -(this.Frame.Width * .5f)
					|| Math.Abs(velocity) > 1000 ? velocity > 0 : newX > (menuWidth / 2);
				else
					show = !( this.Frame.X < 320
						|| Math.Abs(velocity) > 1000 ? velocity > 0 : newX < (320 - (menuWidth / 2)));

                if (show) 
                {
                    OpenDrawer();
                } 
                else 
                {
                    CloseDrawer();
                }
            }
        }

        #endregion
    }
}