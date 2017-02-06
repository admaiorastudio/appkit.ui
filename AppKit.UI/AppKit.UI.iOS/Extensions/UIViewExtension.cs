namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;
    using Foundation;
    using CoreGraphics;

    public static class UIViewExtension   
    {
        public static T FindViewById<T>(this UIView view, string id) where T : UIView
        {
            if (view == null)
                throw new NullReferenceException();

            return (T)FindView(view, id);
        }

        private static UIView FindView(UIView container, string id)
        {
            if (container.Subviews.Length == 0)
                return null;

            UIView view = null;

            foreach (var v in container.Subviews)
            {
                if (v.AccessibilityIdentifier == id)
                {
                    view = v;
                    break;
                }
                else if (v.Subviews.Length != 0)
                {
                    view = FindView(v, id);
                }
            }

            return view;
        }

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

