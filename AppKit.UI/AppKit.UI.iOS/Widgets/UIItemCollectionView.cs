namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Drawing;

    using UIKit;
    using Foundation;

    [Register("UIItemCollectionView")]
    public class UIItemCollectionView : UICollectionView
    {
        public event EventHandler<UIItemListSelectEventArgs> ItemSelected;
        public event EventHandler<UIItemListCommandEventArgs> ItemCommand;

        public new event EventHandler<EventArgs> Scrolled;
        public new event EventHandler<EventArgs> ScrollAnimationEnded;
        public new event EventHandler<EventArgs> ScrolledToTop;

        public UIItemCollectionView(RectangleF frame, UICollectionViewLayout layout)
            : base(frame, layout)
        {
        }

        public UIItemCollectionView(IntPtr handle)
            : base(handle)
        {
        }
            
        public void SelectItem(int index, object item)
        {
            OnItemSelected(index, item);
        }

        public void ExecuteCommand(string command, object userData)
        {
            OnItemCommand(command, userData);
        }

        internal void WhenScrolled()
        {
            OnScrolled();
        }

        internal void WhenScrollAnimationEnded()
        {
            OnScrollAnimationEnded();
        }

        internal void WhenScrolledToTop()
        {
            OnScrolledToTop();
        }

        protected void OnItemSelected(int index, object item)
        {
            if(ItemSelected != null)
                ItemSelected(this, new UIItemListSelectEventArgs(index, item));
        }

        protected void OnItemCommand(string command, object userData)
        {
            if(ItemCommand != null)
                ItemCommand(this, new UIItemListCommandEventArgs(command, userData));
        }

        protected void OnScrolled()
        {
            if (Scrolled != null)
                Scrolled(this, EventArgs.Empty);
        }

        protected void OnScrollAnimationEnded()
        {
            if (ScrollAnimationEnded != null)
                ScrollAnimationEnded(this, EventArgs.Empty);
        }

        protected void OnScrolledToTop()
        {
            if (ScrolledToTop != null)
                ScrolledToTop(this, EventArgs.Empty);
        }
    }
}