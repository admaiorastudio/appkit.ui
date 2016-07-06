namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;
    using Foundation;
    using CoreGraphics;

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
    }
}

