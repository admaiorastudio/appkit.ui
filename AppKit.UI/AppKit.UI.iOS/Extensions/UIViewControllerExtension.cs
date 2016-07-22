namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;

    using UIKit;
    using Foundation;
    using CoreGraphics;

    public static class UIViewControllerExtension
    {
        private static List<WeakReference<UIViewController>> _keyboardWillChangeFrameNotificatedControllers;

        static UIViewControllerExtension()
        {
            _keyboardWillChangeFrameNotificatedControllers = new List<WeakReference<UIViewController>>();
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIKeyboardWillChangeFrameNotification"),
                (notification) =>
                {
                    if (_keyboardWillChangeFrameNotificatedControllers == null)
                        return;

                    if (_keyboardWillChangeFrameNotificatedControllers.Count == 0)
                        return;

                    var controllers = _keyboardWillChangeFrameNotificatedControllers.ToArray();
                    foreach (var w in controllers)
                    {
                        UIViewController c = null;
                        if (!w.TryGetTarget(out c))
                        {
                            _keyboardWillChangeFrameNotificatedControllers.Remove(w);
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

                        CGRect keyboardEndFrame = ((NSValue)notification.UserInfo[UIKeyboard.FrameEndUserInfoKey]).CGRectValue;
                        CGRect keyboardBeginFrame = ((NSValue)notification.UserInfo[UIKeyboard.FrameBeginUserInfoKey]).CGRectValue;
                        UIViewAnimationCurve animationCurve = (UIViewAnimationCurve)((NSNumber)notification.UserInfo[UIKeyboard.AnimationCurveUserInfoKey]).Int32Value;
                        int animationDuration = ((NSNumber)notification.UserInfo[UIKeyboard.AnimationDurationUserInfoKey]).Int32Value;

                        // Check if the final keyboard frame is outside the windows
                        // This means that the keyboard is going to be hidden
                        bool isShowing = keyboardEndFrame.Y < windowRect.Bottom;
                        // Get direction as -1 is up and 1 is down
                        int direction = isShowing ? -1 : 1; //keyboardEndFrame.Y <= keyboardBeginFrame.Y ? -1 : 1;

                        // Calculate navbar offset
                        nfloat navbarOffset = (hasNavigationBar ? 64f : 0f);

                        // Get Available height
                        nfloat availableHeight = windowRect.Height - (navbarOffset + keyboardEndFrame.Height);

                        // Calculate responder offset
                        //nfloat responderOffset =
                        //    isShowing && ((responderFrame.Y - offset) < navbarOffset + 4f) ? (navbarOffset - (offset + responder.Frame.Y)) + 4f : 0;

                        // Calculate view offset                    
                        nfloat offset = 0; // (isShowing ? (keyboardEndFrame.Height * direction) : 0);
                        if (isShowing)
                        { 
                            if((responderFrame.Y + responderFrame.Height) <= availableHeight)
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
                        newFrame.Y = offset; //offset + navbarOffset + responderOffset;
                        view.Frame = newFrame;
                        UIView.CommitAnimations();
                    }

                });
        }

        public static UIViewController GetRootViewController(this UIViewController viewController)
        {
            return UIApplication.SharedApplication.Windows[0].RootViewController;
        }

        public static void SlideUpToShowKeyboard(this UIViewController viewController)
        {
            UIViewController c = null;
            foreach (var w in _keyboardWillChangeFrameNotificatedControllers)
            {
                if (!w.TryGetTarget(out c))
                    continue;

                if (c == viewController)
                    return;
            }

            _keyboardWillChangeFrameNotificatedControllers.Add(new WeakReference<UIViewController>(viewController));
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
    }
}

