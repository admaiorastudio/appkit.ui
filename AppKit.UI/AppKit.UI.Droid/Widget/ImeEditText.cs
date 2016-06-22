namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Content;
    using Android.Util;
    using Android.Views;
    using Android.Widget;

    public class ImeEditText : EditText
    {
        #region Events

        public event EventHandler BackPressed;

        #endregion

        #region Constructors and Destructors

        public ImeEditText(Context context)
            : base(context)
        {
            Initialize();
        }

        public ImeEditText(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
        }

        public ImeEditText(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize();
        }

        #endregion

        #region Public Methods

        public override bool OnKeyPreIme(Keycode keyCode, KeyEvent keyEvent)
        {
            if (keyCode == Keycode.Back
                && keyEvent.Action == KeyEventActions.Up)
            {
                OnBackPressed();
                return true;
            }

            return base.OnKeyPreIme(keyCode, keyEvent);
        }

        #endregion

        #region Event Raising Methods

        protected void OnBackPressed()
        {
            if (BackPressed != null)
                BackPressed(this, EventArgs.Empty);
        }

        #endregion

        #region Methods

        private void Initialize()
        {
        }

        #endregion
    }
}