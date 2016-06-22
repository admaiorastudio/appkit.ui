namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Drawing;

    using CoreGraphics;
    using Foundation;
    using UIKit;

    public class UIRatingEventArgs : EventArgs 
    {
        public UIRatingEventArgs(int rating)
        {
            this.Rating = rating;
        }

        public int Rating
        {
            get;
            set;
        }
    }

    [Register("UIRatingBar")]
    public class UIRatingBar : UIControl
    {    
        #region Constants and Fields

        protected const int DefaultMaxRating = 5;

        protected const float DefaultStarWidthAndHeight = 16f;
        protected const float DefaultStarSpacing = 4f;

        protected int _maxRating;
        protected int _rating;

        protected float _starWidthAndHeight;
        protected float _starSpacing;

        protected bool _respondsToTranslatesAutoresizingMaskIntoConstraints;

        protected UIImage _emptyImage;
        protected UIImage _solidImage;

        protected bool _isIndicator;

        #endregion

        #region Events

        public event EventHandler<UIRatingEventArgs> EditingChangedBlock;
        public event EventHandler<UIRatingEventArgs> EditingDidEndBlock;

        #endregion

        #region Constructors and Destructors

        public UIRatingBar(PointF location, int maxRating)
            : base(new RectangleF(location.X, location.Y, (maxRating * UIRatingBar.DefaultStarWidthAndHeight), UIRatingBar.DefaultStarWidthAndHeight))
        {
            Initialize();

            _maxRating = maxRating;
        }
            
        public UIRatingBar(PointF location, int maxRating, UIImage emptyImage, UIImage solidImage)
            : this(location, maxRating)
        {
            _emptyImage = emptyImage;
            _solidImage = solidImage;
        }
          
        public UIRatingBar(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        #endregion

        #region Properties

        public int MaxRating
        {
            get
            {
                return _maxRating;
            }
            set
            {
                _maxRating = value;
                if (_rating > _maxRating)
                    _rating = _maxRating;

                AdjustFrame();
                SetNeedsDisplay();
            }
        }

        public int Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                _rating = (value < 0) ? 0 : value;
                _rating = (value > _maxRating) ? _maxRating : value;

                SetNeedsDisplay();
            }                
        }

        public float StarWidthAndHeight
        {
            get
            {
                return _starWidthAndHeight;
            }
            set
            {
                _starWidthAndHeight = value;
                if (_starWidthAndHeight < 0)
                    _starWidthAndHeight = 0;
                    
                AdjustFrame();
                SetNeedsDisplay();
            }
        }

        public float StarSpacing
        {
            get
            {
                return _starSpacing;
            }
            set
            {
                _starSpacing = value;
                if (_starSpacing < 0)
                    _starSpacing = 0;

                AdjustFrame();
                SetNeedsDisplay();
            }
        }

        public override CGSize IntrinsicContentSize
        {
            get
            {
                return new CGSize(
                    _maxRating * _starWidthAndHeight + (_maxRating - 1) * _starSpacing,
                    _starWidthAndHeight);
            }
        }
            
        public UIImage EmptyImage
        {
            get
            {
                return _emptyImage;
            }
            set
            {
                _emptyImage = value;

                AdjustFrame();
                SetNeedsDisplay();
            }
        }

        public UIImage SolidImage
        {
            get
            {
                return _solidImage;
            }
            set
            {
                _solidImage = value;

                AdjustFrame();
                SetNeedsDisplay();
            }
        }

        public bool IsIndicator
        {
            get
            {
                return _isIndicator;
            }
            set
            {
                _isIndicator = value;
                this.UserInteractionEnabled = !value;
            }
        }

        #endregion

        #region Public Methods

        public override void Draw(CGRect rect)
        {
            CGPoint currPoint = CGPoint.Empty;

            using (CGContext context = UIGraphics.GetCurrentContext())
            {            
                for (int i = 0; i < _rating; i++)
                {
                    if (_solidImage != null)
                        _solidImage.Draw(new CGRect(currPoint, new CGSize(_starWidthAndHeight, _starWidthAndHeight)));

                    currPoint.X += (_starWidthAndHeight + _starSpacing);
                }

                int remaining = _maxRating - _rating;
                for (int i = 0; i < remaining; i++)
                {
                    if (_emptyImage != null)
                        _emptyImage.Draw(new CGRect(currPoint, new CGSize(_starWidthAndHeight, _starWidthAndHeight)));

                    currPoint.X += (_starWidthAndHeight + _starSpacing);
                }
            }        
        }
            
        public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
        {
            if (this.IsIndicator)
                return base.BeginTracking(uitouch, uievent);

            HandleTouch(uitouch);
            return true;
        }

        public override bool ContinueTracking(UITouch uitouch, UIEvent uievent)
        {
            if (this.IsIndicator)
                return base.ContinueTracking(uitouch, uievent);
                
            HandleTouch(uitouch);
            return true;
        }

        public override void EndTracking(UITouch uitouch, UIEvent uievent)
        {
            if (this.IsIndicator)
                base.EndTracking(uitouch, uievent);

            OnEditingDidEndBlock(_rating);
        }

        #endregion

        #region Event Raising Methods

        protected virtual void OnEditingChangeBlock(int rating)
        {
            if(EditingChangedBlock != null)
                EditingChangedBlock(this, new UIRatingEventArgs(rating));
        }

        protected virtual void OnEditingDidEndBlock(int rating)
        {
            if(EditingDidEndBlock != null)
                EditingDidEndBlock(this, new UIRatingEventArgs(rating));
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            _maxRating = UIRatingBar.DefaultMaxRating;
            _rating = 0;

            _starWidthAndHeight = UIRatingBar.DefaultStarWidthAndHeight;
            _starSpacing = UIRatingBar.DefaultStarSpacing;

            this.BackgroundColor = UIColor.Clear;
            this.Opaque = false;
        }
            
        private void AdjustFrame()
        {
            if (_respondsToTranslatesAutoresizingMaskIntoConstraints 
                && !this.TranslatesAutoresizingMaskIntoConstraints)
            {
                InvalidateIntrinsicContentSize();
            }
            else
            {
                CGPoint center = this.Center;

                CGRect newFrame = new CGRect(
                    this.Frame.Location.X,
                    this.Frame.Location.Y,
                    _maxRating * _starWidthAndHeight + (_maxRating - 1) * _starSpacing,
                    _starWidthAndHeight);

                this.Frame = newFrame;
                this.Center = center;
            }
        }

        private void HandleTouch(UITouch touch)
        {
            nfloat width = this.Frame.Size.Width;
            CGRect section = new CGRect(0, 0, _starWidthAndHeight, this.Frame.Size.Height);

            CGPoint touchLocation = touch.LocationInView(this);

            if (touchLocation.X < 0)
            {
                if (_rating != 0)
                {
                    _rating = 0;
                    OnEditingChangeBlock(_rating);
                }
            }
            else if (touchLocation.X > width)
            {
                if (_rating != _maxRating)
                {
                    _rating = _maxRating;
                    OnEditingChangeBlock(_rating);
                }
            }
            else
            {
                for (int i = 0 ; i < _maxRating ; i++)
                {
                    if ((touchLocation.X > section.Location.X) && (touchLocation.X < (section.Location.X + _starWidthAndHeight)))
                    {
                        if (_rating != (i + 1))
                        {
                            _rating = i + 1;
                            OnEditingChangeBlock(_rating);
                        }
                        break;
                    }

                    var location = section.Location;
                    location.X += (_starWidthAndHeight + _starSpacing);
                    section.Location = location;
                }
            }

            SetNeedsDisplay();
        }

        #endregion
    }
}

