namespace AdMaiora.AppKit.UI.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Foundation;
    using UIKit;
    
    public class UIMainViewController : UIViewController
    {
        #region Inner Classes
        #endregion

        #region Constants and Fields

        private UIBundle _bundle;

        #endregion

        #region Widgets

        private UIView ContentLayout;       
        
        #endregion

        #region Constructors

        public UIMainViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle) 
        {
        }

        #endregion

        #region Properties

        public UINavigationController ContentController
        {
            get;
            private set;
        }

        public UINavigationBar NavigationBar
        {
            get
            {
                return this.ContentController.NavigationBar;
            }
        }

        public UIBundle Arguments
        {
            get
            {
                if (_bundle == null)
                    _bundle = new UIBundle();

                return _bundle;
            }
            set
            {
                var arguments = new UIBundle();

                if (_bundle != null)
                    arguments.PutAll(_bundle);

                if (value != null)
                    arguments.PutAll(value);

                _bundle = arguments;
            }
        }

        #endregion

        #region ViewController Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            #region Designer Stuff           
            #endregion
        }

        public virtual void RemoteNotification(UIBundle data)
        {

        }

        public virtual bool ShouldPopItem()
        {
            UIViewController controller = this.ContentController.TopViewController;
            if (controller is UISubViewController)
                return !((UISubViewController)controller).BarButtonItemSelected(UISubViewController.BarButtonBack);

            return true;
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
        }

        #endregion

        #region Public Methods
        #endregion

        #region Methods

        protected void SetContentView(UIView contentLayout)
        {
            this.ContentLayout = contentLayout;

            this.ContentController = new UIControlledNavigationController();
            this.ContentController.NavigationBar.Translucent = false;
            AddChildViewController(this.ContentController);
            this.ContentLayout.AddSubview(this.ContentController.View);
            this.ContentController.View.Frame = this.ContentLayout.Bounds;
        }

        protected void MakeRoot(Type viewController, UIBundle bundle = null)
        {
            UIMainViewController c = (UIMainViewController)Activator.CreateInstance(viewController);

            if (bundle != null)
            {
                c.Arguments = new UIBundle();
                c.Arguments.PutAll(bundle);
            }

            UIApplication.SharedApplication.Windows[0].RootViewController = c;
            UIApplication.SharedApplication.Windows[0].MakeKeyAndVisible();
        }

        public void DismissKeyboard()
        {
            UIView firstResponder = this.View.GetFirstResponder();
            if (firstResponder != null)
                firstResponder.ResignFirstResponder();
        }

        #endregion

        #region Event Handlers
        #endregion
    }
}