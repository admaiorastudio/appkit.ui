namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UIKit;
    using Foundation;
    using CoreGraphics;

    public static class UIViewControllerExtension
    {
        #region Inner Classes

        class KeyboardWatcher
        {
            public UIViewController Controller
            {
                get;
                set;
            }

            public Action WhenShowing
            {
                get;
                set;
            }

            public Action WhenHiding
            {
                get;
                set;
            }
        }

        #endregion

        #region Constants and Fields

        private static List<KeyboardWatcher> _showKeyboardWatchers;

        private static List<WeakReference<UIViewController>> _resizeToShowKeyboardControllers;
        private static List<WeakReference<UIViewController>> _slideUpToShowKeyboardControllers;

        #endregion

        #region Constructors

        static UIViewControllerExtension()
        {
            _showKeyboardWatchers = new List<KeyboardWatcher>();

            _resizeToShowKeyboardControllers = new List<WeakReference<UIViewController>>();
            _slideUpToShowKeyboardControllers = new List<WeakReference<UIViewController>>();

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIKeyboardWillChangeFrameNotification"),
                (notification) =>
                {
                    CGRect keyboardEndFrame = ((NSValue)notification.UserInfo[UIKeyboard.FrameEndUserInfoKey]).CGRectValue;
                    CGRect keyboardBeginFrame = ((NSValue)notification.UserInfo[UIKeyboard.FrameBeginUserInfoKey]).CGRectValue;
                    UIViewAnimationCurve animationCurve = (UIViewAnimationCurve)((NSNumber)notification.UserInfo[UIKeyboard.AnimationCurveUserInfoKey]).Int32Value;
                    int animationDuration = ((NSNumber)notification.UserInfo[UIKeyboard.AnimationDurationUserInfoKey]).Int32Value;

                    if (_showKeyboardWatchers != null
                        && _showKeyboardWatchers.Count > 0)
                    {
                        #region Notify logic

                        foreach(var w in _showKeyboardWatchers)
                        {
                            UIView view = w.Controller.View;
                            CGRect windowRect = view.Window.Frame;

                            // Check if the final keyboard frame is outside the windows
                            // This means that the keyboard is going to be hidden
                            bool isShowing = keyboardEndFrame.Y < windowRect.Bottom;

                            if (isShowing)
                            {
                                w.WhenShowing?.Invoke();
                            }
                            else
                            {
                                w.WhenHiding?.Invoke();
                            }
                        }

                        #endregion
                    }

                    if (_resizeToShowKeyboardControllers != null
                        && _resizeToShowKeyboardControllers.Count > 0)
                    {
                        #region Resize logic

                        var controllers = _resizeToShowKeyboardControllers.ToArray();
                        foreach (var w in controllers)
                        {
                            UIViewController c = null;
                            if (!w.TryGetTarget(out c))
                            {
                                _resizeToShowKeyboardControllers.Remove(w);
                                continue;
                            }

                            // Get View Controller view
                            UIView view = c.View;
                            if (view == null)
                                continue;

                            bool hasNavigationBar = c.NavigationController != null
                                && c.NavigationController.NavigationBarHidden == false;

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

                        #endregion
                    }

                    if (_slideUpToShowKeyboardControllers != null
                    && _slideUpToShowKeyboardControllers.Count > 0)
                    {
                        #region Slide up logic

                        var controllers = _slideUpToShowKeyboardControllers.ToArray();
                        foreach (var w in controllers)
                        {
                            UIViewController c = null;
                            if (!w.TryGetTarget(out c))
                            {
                                _slideUpToShowKeyboardControllers.Remove(w);
                                continue;
                            }

                            // Get View Controller view
                            UIView view = c.View;
                            if (view == null)
                                continue;

                            // Get first responder
                            UIView responder = view.GetFirstResponder();
                            if (responder == null)
                                continue;

                            bool hasNavigationBar = c.NavigationController != null
                                && c.NavigationController.NavigationBarHidden == false;

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
                                    //responderOffset = 0;

                                    if (view.Frame.Y == navbarOffset)
                                        return;
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

                            UIView.BeginAnimations(null);
                            UIView.SetAnimationBeginsFromCurrentState(true);
                            UIView.SetAnimationDuration(animationDuration * 0.95);
                            UIView.SetAnimationCurve(animationCurve);
                            CGRect newFrame = view.Frame;
                            newFrame.Y = offset; 
                            view.Frame = newFrame;
                            UIView.CommitAnimations();
                        }

                        #endregion
                    }

                });
        }

        #endregion

        #region Extension Methods

        public static UIViewController GetRootViewController(this UIViewController viewController)
        {
            return UIApplication.SharedApplication.Windows[0].RootViewController;
        }

        public static void StartNotifyKeyboardStatus(this UIViewController controller, Action whenShowing, Action whenHiding)
        {
            foreach (var n in _showKeyboardWatchers)
            {
                if (n.Controller == controller)
                    return;
            }

            var notify = new KeyboardWatcher();
            notify.Controller = controller;
            notify.WhenShowing = whenShowing;
            notify.WhenHiding = whenHiding;

            _showKeyboardWatchers.Add(notify);
        }

        public static void StopNotifyKeyboardStatus(this UIViewController controller)
        {
            KeyboardWatcher notify = null;

            var notifiy = _showKeyboardWatchers.SingleOrDefault(n => n.Controller == controller);
            if (notify != null)
                _showKeyboardWatchers.Remove(notify);
        }

        public static void ResizeToShowKeyboard(this UIViewController viewController)
        {
            UIViewController c = null;
            foreach (var w in _resizeToShowKeyboardControllers)
            {
                if (!w.TryGetTarget(out c))
                    continue;

                if (c == viewController)
                    return;
            }

            _resizeToShowKeyboardControllers.Add(new WeakReference<UIViewController>(viewController));
        }

        public static void SlideUpToShowKeyboard(this UIViewController viewController)
        {
            UIViewController c = null;
            foreach (var w in _slideUpToShowKeyboardControllers)
            {
                if (!w.TryGetTarget(out c))
                    continue;

                if (c == viewController)
                    return;
            }

            _slideUpToShowKeyboardControllers.Add(new WeakReference<UIViewController>(viewController));
        }

        public static void AutoShouldReturnTextFields(this UIViewController viewController, UITextField[] texts)
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

        public static void AutoDismissTextFields(this UIViewController viewController, UITextField[] texts)
        {
            UIView view = viewController.View;
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

        public static void DismissKeyboard(this UIViewController viewController)
        {
            UIView firstResponder = viewController.View.GetFirstResponder();
            if (firstResponder != null)
                firstResponder.ResignFirstResponder();
        }

        #endregion
    }
}

