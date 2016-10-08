namespace AdMaiora.AppKit.UI.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Foundation;
    using UIKit;
    
    public interface IBackButton
    {
        bool ViewWillPop();
    }

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
                return _bundle;
            }
            set
            {
                _bundle = value;
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

        public virtual bool ShouldPopItem()
        {
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
                bundle.PutAll(bundle);

            UIApplication.SharedApplication.Windows[0].RootViewController = c;
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