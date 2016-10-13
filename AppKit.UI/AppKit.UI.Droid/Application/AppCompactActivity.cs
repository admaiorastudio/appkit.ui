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
    using Android.Views.InputMethods;

    public interface IBackButton
    {
        bool OnBackButton();
    }

    public class AppCompactActivity : Android.Support.V7.App.AppCompatActivity
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        private Bundle _extras;

        #endregion

        #region Widgets

        private Android.Support.V7.Widget.Toolbar Toolbar;

        #endregion

        #region Constructors

        public AppCompactActivity()
        {
        }

        #endregion

        #region Properties

        public Bundle Arguments
        {
            get
            {
                if (_extras == null)
                    _extras = new Bundle();

                Bundle extras = new Bundle();
                if (_extras != null)
                    extras.PutAll(_extras);

                if (this.Intent.Extras != null)
                    extras.PutAll(this.Intent.Extras);

                return extras;
            }
            set
            {
                if (value != null)
                    _extras = value;
            }
        }

        #endregion

        #region Activity Methods
        #endregion

        #region Public Methods
        #endregion

        #region Methods
        
        protected void SetContentView(int layoutResID, int toolBarResId = 0)
        {
            base.SetContentView(layoutResID);
            ViewBuilder.GetWidgets(this);

            if (toolBarResId != 0)
            {
                this.Toolbar = FindViewById< Android.Support.V7.Widget.Toolbar>(toolBarResId);
                SetSupportActionBar(this.Toolbar);
            }
        }

        protected void DismissKeyboard()
        {
            var view = this.CurrentFocus;
            if (view == null)
                return;

            InputMethodManager imm = (InputMethodManager)GetSystemService(Activity.InputMethodService);
            imm.HideSoftInputFromWindow(view.WindowToken, 0);

            if (this.Window.DecorView != null)
                this.Window.DecorView.ClearFocus();
        }                       

        protected void MakeRoot(Type activity, Bundle extras = null)
        {
            Intent i = new Intent(this, activity);

            if(extras != null)
                i.Extras.PutAll(extras);

            StartActivity(i);
            Finish();
        }

        #endregion

        #region Event Handlers
        #endregion
    }
}