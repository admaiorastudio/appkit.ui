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

        private bool _preseveViewWhenInBackStack;

        private View _view;

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

        public string Title
        {
            get
            {
                var actionBar = this.ActionBar;
                if (actionBar == null)
                    return null;

                return actionBar.Title;
            }
            set
            {
                var actionBar = this.ActionBar;
                if (actionBar == null)
                    return;

                actionBar.Title = value;
            }
        }

        #endregion

        #region Fragment Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _preseveViewWhenInBackStack = false;
            _view = null;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // WARNING SPERIMENTAL
            //
            // We're using this approach to keep View refrence alive when fragment is stored in back stack
            // http://stackoverflow.com/questions/11353075/how-can-i-maintain-fragment-state-when-added-to-the-back-stack
            // If you don't want this feature override this method!

            OnCreateView(inflater, container);

            if (_view == null)
                throw new InvalidOperationException("You need to set the current fragment layout using the 'SetContentView' method.");

            return _view;
        }

        public virtual void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            // Load your view using the SetContentView method
        }

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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Activity.OnBackPressed();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }            
        }

        public virtual bool OnBackButton()
        {
            return false;
        }

        public override void OnDestroyView()
        {
            if (_preseveViewWhenInBackStack)
            {
                if (_view.Parent != null)
                    ((ViewGroup)_view.Parent).RemoveView(_view);
            }
            
            base.OnDestroyView();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if(_preseveViewWhenInBackStack)
            {
                _preseveViewWhenInBackStack = false;
                _view = null;                
            }                
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        protected void SetContentView(int layoutResID, LayoutInflater inflater, ViewGroup container)
        {
            _preseveViewWhenInBackStack = true;

            if(_view == null)
                _view = inflater.InflateWithWidgets(layoutResID, this, container, false);
        }

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