namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Linq;

    using UIKit;
    using CoreGraphics;

    public enum UIToastLength
    {
        Short,
        Long
    }

    public class UIToast
    {
        #region Inner Classes

        class UIStatusBarViewController : UIViewController
        {
            public override bool PrefersStatusBarHidden()
            {
                return true;
            }
        }

        #endregion

        #region Constants and Fields

        private UIView _toast;        
        private UILabel _textLabel;

        private UIToastLength _duration;

        #endregion

        #region Constructors

        public UIToast(string text, UIToastLength duration)
        {
            UIWindow window = UIApplication.SharedApplication.Windows[0];
            
            nfloat maxWidth = window.Bounds.Width - 32f;
            var frame = new CGRect(0, 0, 0, 21f);

            // Setting lable
			_textLabel = new UILabel(frame);
			_textLabel.Text = text;
            _textLabel.Font = UIFont.SystemFontOfSize(15f);
			_textLabel.TextColor = UIColor.White;
			_textLabel.TextAlignment = UITextAlignment.Left;
			_textLabel.AutoresizingMask = UIViewAutoresizing.All;
            _textLabel.Lines = 0;
            _textLabel.LineBreakMode = UILineBreakMode.WordWrap;

            _textLabel.SizeToFit();
            frame = _textLabel.Frame;    

            // Calculate label frame container
            if(frame.Width < maxWidth)
            {
                nfloat width = frame.Width + 12f;
                nfloat x = (window.Bounds.Width - width) * .5f;
                frame = new CGRect(x, window.Bounds.Height - (frame.Height + 80f), width, frame.Height + 8f);
            }
            else
            {                                
                // Update label frame to fit
                frame.Width = maxWidth;
                _textLabel.Frame = frame;

                _textLabel.SizeToFitHeight();
                frame = _textLabel.Frame;    

                frame = new CGRect(4f, window.Bounds.Height - (frame.Height + 80f), window.Bounds.Width - 8f, frame.Height + 8f);
            }

            // Creating label container

			_toast = new UIView(frame);
            _toast.Layer.BackgroundColor = UIColor.DarkGray.CGColor;
            _toast.Layer.Opacity = .75f;
            _toast.Layer.BorderWidth = 0f;
            _toast.Layer.CornerRadius = _toast.Layer.Frame.Height / 4f;
			_toast.AutosizesSubviews = true;
			_toast.AddSubview(_textLabel);

            // Center label into container
            _textLabel.Center = new CGPoint(frame.Width * .5f, frame.Height * .5f);             

            // Layout!
			_toast.LayoutSubviews();

            // Set duration
            _duration = duration;
		}

        #endregion

        #region Public Methods

        public static UIToast MakeText(string text, UIToastLength duration)
        {
            UIToast toast = new UIToast(text, duration);
            return toast;
        }

        public void Show()
        {
            UIWindow window = new UIWindow(UIScreen.MainScreen.Bounds);
            window.BackgroundColor = UIColor.Clear;
            window.WindowLevel = UIApplication.SharedApplication.Windows.Last().WindowLevel + 1;
            window.RootViewController = UIApplication.SharedApplication.Windows[0].RootViewController;
            window.UserInteractionEnabled = false;

            _toast.Alpha = 0f;

            if (_toast.Superview == null)
            {
                window.Add(_toast);
                window.MakeKeyAndVisible();
            }
        
            float delay = _duration == UIToastLength.Short ? 1f : 1.5f;

            UIView.Animate(.4f * delay,
            () =>
            {
                _toast.Alpha = 1f;
            },
            () =>
            {
                UIView.Animate(2f * delay,
                () =>
                {
                    UIView.SetAnimationDelay(.8f * delay);
                    _toast.Alpha = 0f;
                },
                () =>
                {
                    if (_toast.Layer.AnimationKeys == null
                        || _toast.Layer.AnimationKeys.Length == 0)
                    {
                        _toast.RemoveFromSuperview();

                        window.Hidden = true;
                        window.RemoveFromSuperview();
                    }
                });
            });
        }

        #endregion
    }
}