namespace AdMaiora.AppKit.UI.App
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Foundation;
    using UIKit;
    using CoreGraphics;

    public class UISubViewController : UIViewController
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        private UIBundle _bundle;

        private bool _isNotifyngKeyboardStatus;
        private bool _isViewResizingByKeyboard;
        private bool _isViewSlidingByKeyboard;

        private bool _isKeyboardVisible;
        private NSObject _keyboardObserver;

        #endregion

        #region Constructors

        public UISubViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        #endregion

        #region Properties

        public UIViewController MainViewController
        {
            get
            {                                
                return UIApplication.SharedApplication.Windows[0].RootViewController;
            }
        }        

        public UINavigationBar NavigationBar
        {
            get
            {
                if (this.NavigationController == null)
                    return null;

                return this.NavigationController.NavigationBar;
            }
        }

        public UIBundle Arguments
        {
            get
            {
                if (_bundle == null)
                    _bundle = new UIBundle();

                return _bundle;
            }
            set
            {
                var arguments = new UIBundle();

                if (_bundle != null)
                    arguments.PutAll(_bundle);

                if (value != null)
                    arguments.PutAll(value);

                _bundle = arguments;
            }
        }

        #endregion

        #region ViewController Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff
            #endregion

            _keyboardObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIKeyboardWillChangeFrameNotification"),
                (notification) =>
                {
                    CGRect keyboardEndFrame = ((NSValue)notification.UserInfo[UIKeyboard.FrameEndUserInfoKey]).CGRectValue;
                    CGRect keyboardBeginFrame = ((NSValue)notification.UserInfo[UIKeyboard.FrameBeginUserInfoKey]).CGRectValue;
                    UIViewAnimationCurve animationCurve = (UIViewAnimationCurve)((NSNumber)notification.UserInfo[UIKeyboard.AnimationCurveUserInfoKey]).Int32Value;
                    int animationDuration = ((NSNumber)notification.UserInfo[UIKeyboard.AnimationDurationUserInfoKey]).Int32Value;

                    #region Notify Logic

                    if (_isNotifyngKeyboardStatus)
                    {
                        CGRect windowRect = this.View.Window.Frame;

                        // Check if the final keyboard frame is outside the windows
                        // This means that the keyboard is going to be hidden
                        bool isShowing = keyboardEndFrame.Y < windowRect.Bottom;

                        if (isShowing)
                        {
                            if (!_isKeyboardVisible)
                            {
                                _isKeyboardVisible = true;

                                KeyboardWillShow();
                            }
                        }
                        else
                        {
                            if (_isKeyboardVisible)
                            {
                                _isKeyboardVisible = false;

                                KeyboardWillHide();
                            }
                        }
                    }

                    #endregion

                    #region Resize logic

                    if (_isViewResizingByKeyboard)
                    {
                        // Get View Controller view
                        UIView view = this.View;
                        if (view != null)
                        {
                            bool hasNavigationBar = this.NavigationController != null
                                && this.NavigationController.NavigationBarHidden == false;

                            // Calculate navbar offset
                            nfloat navbarOffset = (hasNavigationBar ? 64f : 0f);

                            CGRect windowRect = view.Window.Frame;

                            nfloat availableHeight = windowRect.Height - keyboardEndFrame.Height - navbarOffset;

                            UIView.BeginAnimations(null);
                            UIView.SetAnimationBeginsFromCurrentState(true);
                            UIView.SetAnimationDuration(animationDuration * 0.95);
                            UIView.SetAnimationCurve(animationCurve);
                            CGRect newFrame = view.Frame;
                            newFrame.Height = availableHeight;
                            view.Frame = newFrame;
                            UIView.CommitAnimations();
                        }
                    }

                    #endregion

                    #region Slide up logic

                    if (_isViewSlidingByKeyboard)
                    {
                        // Get View Controller view
                        UIView view = this.View;
                        if (view != null)
                        {
                            // Get first responder
                            UIView responder = view.GetFirstResponder();
                            if (responder != null)
                            {
                                bool hasNavigationBar = this.NavigationController != null
                                    && this.NavigationController.NavigationBarHidden == false;

                                CGRect responderFrame = view.ConvertRectFromView(responder.Frame, responder.Superview);

                                // Animate in case

                                CGRect windowRect = view.Window.Frame;

                                // Check if the final keyboard frame is outside the windows
                                // This means that the keyboard is going to be hidden
                                bool isShowing = keyboardEndFrame.Y < windowRect.Bottom;
                                // Get direction as -1 is up and 1 is down
                                int direction = isShowing ? -1 : 1;

                                // Calculate navbar offset
                                nfloat navbarOffset = (hasNavigationBar ? 64f : 0f);

                                // Get Available height
                                nfloat availableHeight = windowRect.Height - keyboardEndFrame.Height - navbarOffset;

                                // Calculate view offset                    
                                nfloat offset = 0; // (isShowing ? (keyboardEndFrame.Height * direction) : 0);
                                if (isShowing)
                                {
                                    if ((responderFrame.Y + responderFrame.Height) <= availableHeight)
                                    {
                                        offset = navbarOffset;                                        
                                        if (view.Frame.Y == navbarOffset)
                                            offset = nfloat.MinValue;
                                    }
                                    else
                                    {
                                        // k is the distance between the top of the keyboard frame and the top of the widget
                                        nfloat k = responderFrame.Top - keyboardEndFrame.Top;
                                        // offset is calculated to bring the widget over the keyboard with a small padding (p)
                                        nfloat p = 4;
                                        offset = -(k + responderFrame.Height + p);
                                    }
                                }
                                else
                                {
                                    offset = navbarOffset;
                                }

                                if (offset != nfloat.MinValue)
                                {
                                    UIView.BeginAnimations(null);
                                    UIView.SetAnimationBeginsFromCurrentState(true);
                                    UIView.SetAnimationDuration(animationDuration * 0.95);
                                    UIView.SetAnimationCurve(animationCurve);
                                    CGRect newFrame = view.Frame;
                                    newFrame.Y = offset;
                                    view.Frame = newFrame;
                                    UIView.CommitAnimations();
                                }
                            }
                        }
                    }

                    #endregion
                });
        }

        public virtual void KeyboardWillShow()
        {

        }

        public virtual void KeyboardWillHide()
        {

        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            
            NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardObserver);
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        public void StartNotifyKeyboardStatus()
        {
            _isNotifyngKeyboardStatus = true;
        }

        public void StopNotifyKeyboardStatus()
        {
            _isNotifyngKeyboardStatus = false;
        }

        public void ResizeToShowKeyboard()
        {
            _isViewResizingByKeyboard = true;
            _isViewSlidingByKeyboard = false;
        }

        public void SlideUpToShowKeyboard()
        {
            _isViewResizingByKeyboard = false;
            _isViewSlidingByKeyboard = true;
        }

        public void AutoShouldReturnTextFields(UITextField[] texts)
        {
            foreach (var text in texts)
            {
                text.ShouldReturn = (UITextField t) =>
                {
                    t.ResignFirstResponder();
                    if (t.ReturnKeyType != UIReturnKeyType.Next)
                        return true;

                    int index = Array.IndexOf(texts, t);
                    if (++index == texts.Length)
                        return true;

                    return texts[index].BecomeFirstResponder();

                };
            }
        }

        public void AutoDismissTextFields(UITextField[] texts)
        {
            UIView view = this.View;
            if (view == null)
                return;

            view.AddGestureRecognizer(new UITapGestureRecognizer(
                () =>
                {
                    foreach (var text in texts)
                    {
                        if (text.IsFirstResponder)
                            text.ResignFirstResponder();
                    }
                }));
        }

        public void DismissKeyboard()
        {
            UIView firstResponder = this.View.GetFirstResponder();
            if (firstResponder != null)
                firstResponder.ResignFirstResponder();
        }

        #endregion

        #region Event Handlers
        #endregion
    }
}
