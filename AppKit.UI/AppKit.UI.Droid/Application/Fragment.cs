namespace AdMaiora.AppKit.UI.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using Android.Support.V4.App;
    using Android.Support.V7.App;
    using Android.Views.InputMethods;

    public class Fragment : Android.Support.V4.App.Fragment
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        private bool _notifyKeyboardStatus;

        private bool _isKeyboardVisible;
        
        #endregion

        #region Widgets
        #endregion

        #region Constructors

        public Fragment()
        {                        
        }

        #endregion

        #region Properties

        public Android.Support.V7.App.ActionBar ActionBar
        {
            get
            {
                FragmentActivity activity = this.Activity;
                if (activity == null)
                    return null;

                return ((AppCompatActivity)activity).SupportActionBar;
            }
        }

        public new Bundle Arguments
        {
            get
            {
                if (base.Arguments == null)
                    base.Arguments = new Bundle();

                return base.Arguments;
            }
            set
            {
                var arguments = new Bundle();

                if (base.Arguments != null)
                    arguments.PutAll(base.Arguments);

                if (value != null)
                    arguments.PutAll(value);

                base.Arguments = arguments;
            }
        }

        #endregion

        #region Fragment Methods

        public override void OnStart()
        {
            base.OnStart();

            if(_notifyKeyboardStatus)
                this.View.ViewTreeObserver.GlobalLayout += ViewTreeObserver_GlobalLayout;
        }
        
        public virtual void OnKeyboardShow()
        {

        }

        public virtual void OnKeyboardHide()
        {

        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        protected void StartNotifyKeyboardStatus()
        {
            _notifyKeyboardStatus = true;            
        }

        protected void StopNotifyKeyboardStatus()
        {
            this.View.ViewTreeObserver.GlobalLayout -= ViewTreeObserver_GlobalLayout;
        }

        protected void ResizeToShowKeyboard()
        {
            if (this.Activity == null)
                return;

            if (this.Activity.Window == null)
                return;

            this.Activity.Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);
        }

        protected void SlideUpToShowKeyboard()
        {
            if (this.Activity == null)
                return;

            if (this.Activity.Window == null)
                return;

            this.Activity.Window.SetSoftInputMode(Android.Views.SoftInput.AdjustPan);
        }

        protected void DismissKeyboard()
        {
            FragmentActivity activity = this.Activity;
            if (activity == null)
                return;

            var view = activity.CurrentFocus;
            if (view == null)
                return;

            InputMethodManager imm = (InputMethodManager)activity.GetSystemService(Android.App.Activity.InputMethodService);
            imm.HideSoftInputFromWindow(view.WindowToken, 0);

            if (activity.Window.DecorView != null)
                activity.Window.DecorView.ClearFocus();
        }

        #endregion

        #region Event Handlers

        private void ViewTreeObserver_GlobalLayout(object sender, EventArgs e)
        {
            if (this.View == null)
                return;

            var r = new Android.Graphics.Rect();
            this.View.GetWindowVisibleDisplayFrame(r);

            int height = this.View.RootView.Height;
            if (r.Bottom != height)
            {
                // Keyboard showing

                if (_isKeyboardVisible)
                    return;

                _isKeyboardVisible = true;

                OnKeyboardShow();
            }
            else
            {
                // Keyboard hiding

                if (!_isKeyboardVisible)
                    return;

                _isKeyboardVisible = false;

                OnKeyboardHide();
            }
        }

        #endregion
    }
}