namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Android.Views;
    using Android.Support.V4.App;
    using Android.Support.V7.App;

    public static class SupportFragmentExtension
    {
        #region Inner Classes

        class KeyboardWatcher
        {
            public Fragment Fragment
            {
                get;
                set;
            }

            public EventHandler GlobalLayout
            {
                get;
                set;
            }

            public bool IsKeyboardVisible
            {
                get;
                set;
            }
        }

        #endregion

        #region Constants and Fields

        private static List<KeyboardWatcher> _showKeyboardWatchers;

        #endregion

        #region Constructors

        static SupportFragmentExtension()
        {
            _showKeyboardWatchers = new List<KeyboardWatcher>();
        }

        #endregion

        #region Extension Methods

        public static ActionBar GetActionBar(this Fragment fragment)
        {
            FragmentActivity activity = fragment.Activity;
            if (activity == null)
                return null;

            return ((AppCompatActivity)activity).SupportActionBar;
        }

        public static void SetTitle(this Fragment fragment, string title)
        {
            if (fragment.GetActionBar() != null)
                fragment.GetActionBar().Title = title;
        }

        public static void StartNotifyKeyboardStatus(this Fragment fragment, View view, Action whenShowing, Action whenHiding)
        {            
            foreach (var n in _showKeyboardWatchers)
            {
                if (n.Fragment == fragment)
                    return;
            }

            var notify = new KeyboardWatcher();
            notify.Fragment = fragment;
            notify.GlobalLayout =
                (s, e) =>
                {
                    if (view == null)
                        return;

                    var r = new Android.Graphics.Rect();
                    view.GetWindowVisibleDisplayFrame(r);

                    int height = view.RootView.Height;
                    if (r.Bottom != height)
                    {
                        if (notify.IsKeyboardVisible)
                            return;

                        notify.IsKeyboardVisible = true;

                        whenShowing?.Invoke();
                    }
                    else
                    {
                        // Keyboard hiding

                        if (!notify.IsKeyboardVisible)
                            return;

                        notify.IsKeyboardVisible = false;

                        whenHiding?.Invoke();
                    }
                };
            
            view.ViewTreeObserver.GlobalLayout += notify.GlobalLayout;           
            _showKeyboardWatchers.Add(notify);
        }

        public static void StopNotifyKeyboardStatus(this Fragment fragment)
        {            
            KeyboardWatcher notify = null;
            foreach (var n in _showKeyboardWatchers)
            {
                if (n.Fragment == fragment)
                {                    
                    n.Fragment.View.ViewTreeObserver.GlobalLayout -= n.GlobalLayout;

                    notify = n;
                    break;                    
                }
            }

            if (notify != null)
                _showKeyboardWatchers.Remove(notify);
        }

        public static void ResizeToShowKeyboard(this Fragment fragment)
        {
            if (fragment.Activity == null)
                return;

            if (fragment.Activity.Window == null)
                return;

            fragment.Activity.Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);
        }

        public static void SlideUpToShowKeyboard(this Fragment fragment)
        {
            if (fragment.Activity == null)
                return;

            if (fragment.Activity.Window == null)
                return;

            fragment.Activity.Window.SetSoftInputMode(Android.Views.SoftInput.AdjustPan);
        }

        public static void DismissKeyboard(this Fragment fragment)
        {
            FragmentActivity activity = fragment.Activity;
            if (activity == null)
                return;

            activity.DismissKeyboard();
        }

        #endregion
    }
}