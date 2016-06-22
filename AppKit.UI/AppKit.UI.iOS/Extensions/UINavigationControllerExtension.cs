namespace AdMaiora.AppKit.UI
{
    using UIKit;

    public static class UINavigationControllerExtension
    {
        public static void ReplaceViewController(this UINavigationController navigationController, UIViewController viewController, bool animated)
        {
            navigationController.PopViewController(false);
            navigationController.PushViewController(viewController, animated);
        }           
    }
}

