namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;
    using Foundation;
    using CoreGraphics;

    public static class CGRectExtension
    {
        public static CGRect OffsetIf(this CGRect r, bool condition, CGRect whenTrue, CGRect whenFalse)
        {
            CGRect offset = condition ? whenTrue : whenFalse;
            r.X += offset.X;
            r.Y += offset.Y;
            r.Width += offset.Width;
            r.Height += offset.Height;

            return r;
        }
    }

    public static class UIViewExtension   
    {               
        public static UIView GetFirstResponder(this UIView view)
        {            
            UIView[] subviews = view.Subviews;
            if(subviews == null || subviews.Length == 0)
                return null;

            UIView responder = null;
            foreach(UIView v in subviews)
            {
                if(v.IsFirstResponder)
                    return v;

                responder = v.GetFirstResponder();
                if(responder != null)
                    return responder;
            }

            return null;
        }

        public static void AnimateVertically(this UIView view, int direction, float distance, float duration = .3f)
        {
            float movement = (direction < 0 ? -distance : distance);
            UIView.BeginAnimations("AnimateTextField");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(duration);

            var frame = view.Frame;
            frame.Y += movement;
            view.Frame = frame;

            UIView.CommitAnimations();
        }

        public static void AnimateVerticallyUp(this UIView view, float distance, float duration = .3f)
        {
            AnimateVertically(view, -1, distance, duration);
        }

        public static void AnimateVerticallyDown(this UIView view, float distance, float duration = .3f)
        {
            AnimateVertically(view, 1, distance, duration);
        }

        public static void SlideUpToShowKeyboard(this UIView view, bool hasNavigationBar = true)
        {                    
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIKeyboardWillChangeFrameNotification"),
                (notification) =>
                {
                    // Get first responder
                    UIView responder = view.GetFirstResponder();
                    if(responder == null)
                        return;

                    CGRect responderFrame = view.ConvertRectFromView(responder.Frame, responder);

                    // Animate in case

                    CGRect windowRect = view.Window.Frame;

                    CGRect keyboardEndFrame =  ((NSValue)notification.UserInfo[UIKeyboard.FrameEndUserInfoKey]).CGRectValue;
                    CGRect keyboardBeginFrame = ((NSValue)notification.UserInfo[UIKeyboard.FrameBeginUserInfoKey]).CGRectValue;
                    UIViewAnimationCurve animationCurve = (UIViewAnimationCurve)((NSNumber)notification.UserInfo[UIKeyboard.AnimationCurveUserInfoKey]).Int32Value;
                    int animationDuration = ((NSNumber)notification.UserInfo[UIKeyboard.AnimationDurationUserInfoKey]).Int32Value;

                    // Get Available height
                    nfloat availableHeight = windowRect.Height - keyboardEndFrame.Height;

                    // Check if the final keyboard frame is outside the windows
                    // This means that the keyboard is going to be hidden
                    bool isShowing = keyboardEndFrame.Y < windowRect.Bottom;
                    // Get direction as -1 is up and 1 is down
                    int direction = isShowing ? -1 : 1; //keyboardEndFrame.Y <= keyboardBeginFrame.Y ? -1 : 1;
                    
                    // Do not slide up if not necessary
                    if(isShowing
                        && (responderFrame.Y + responderFrame.Height) <= availableHeight)
                    {
                        return;
                    }

                    // Calculate view offset
                    nfloat navbarOffset = (hasNavigationBar ? 64f : 0f); 
                    nfloat offset = (isShowing ? (keyboardEndFrame.Height * direction) : 0);                    
                    
                    UIView.BeginAnimations(null);
                    UIView.SetAnimationBeginsFromCurrentState(true);
                    UIView.SetAnimationDuration(animationDuration * 0.95);
                    UIView.SetAnimationCurve(animationCurve);
                    CGRect newFrame = view.Frame;                   
                    newFrame.Y = navbarOffset + offset;
                    view.Frame = newFrame;
                    UIView.CommitAnimations();

                });            
        }

        public static void AutoShouldReturnTextFields(this UIView view, UITextField[] texts)
        {
            foreach(var text in texts)
            {
                text.ShouldReturn = (UITextField t) =>
                    {    
                        t.ResignFirstResponder();
                        if(t.ReturnKeyType != UIReturnKeyType.Next)
                            return true;

                        int index = Array.IndexOf(texts, t);
                        if(++index == texts.Length)
                            return true;

                        return texts[index].BecomeFirstResponder();

                    };
            }
        }

        public static void AutoDismissTextFields(this UIView view, UITextField[] texts)
        {
            view.AddGestureRecognizer(new UITapGestureRecognizer(
                () =>
                {
                    foreach(var text in texts)
                    {
                        if(text.IsFirstResponder)
                            text.ResignFirstResponder();
                    }
                }));
        }
    }
}

