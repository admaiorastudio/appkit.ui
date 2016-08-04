namespace AdMaiora.AppKit.UI
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

    public class UIControlledNavigationController : UINavigationController, IUINavigationBarDelegate
    {
        #region Constants and Fields

        #endregion

        #region Constructors

        public UIControlledNavigationController()
            : base()
        {
        }

        #endregion

        #region Public Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        [Export("navigationBar:shouldPopItem:")]
        public bool ShouldPopItem(UINavigationBar navigationBar, UINavigationItem item)
        {
            if (this.ViewControllers.Length < this.NavigationBar.Items.Length)
                return true;

            bool shouldPop = true;
            UIViewController controller = this.TopViewController;
            if (controller is IBackButton)
                shouldPop = !((IBackButton)controller).ViewWillPop();

            if (shouldPop)
            {
                CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => PopViewController(true));
            }
            else
            {
                // Workaround for iOS7.1. Thanks to @boliva
                // http://stackoverflow.com/posts/comments/34452906
                foreach (UIView subview in this.NavigationBar.Subviews)
                {
                    if (subview.Alpha < 1f)
                        UIView.Animate(.25f, () => subview.Alpha = 1);
                }
            }

            return false;
        }

        #endregion
    }
}