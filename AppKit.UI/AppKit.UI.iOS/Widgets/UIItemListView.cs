namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;
    using Foundation;
	using CoreGraphics;

    public class UIItemListSelectEventArgs : EventArgs
    {
        public UIItemListSelectEventArgs(int index, object item)
        {
            this.Index = index;
            this.Item = item;
        }

        public int Index
        {
            get;
            private set;
        }

        public object Item
        {
            get;
            private set;
        }
    }

    public class UIItemListLongPressEventArgs : EventArgs
    {
        public UIItemListLongPressEventArgs(int index, object item)
        {
            this.Index = index;
            this.Item = item;
        }

        public int Index
        {
            get;
            private set;
        }

        public object Item
        {
            get;
            private set;
        }
    }

    public class UIItemListCommandEventArgs : EventArgs
    {
        public UIItemListCommandEventArgs(string command, object userData)
        {
            this.Command = command;
            this.UserData = userData;
        }

        public string Command
        {
            get;
            private set;
        }

        public object UserData
        {
            get;
            private set;
        }
    }

    [Register("UIItemListView")]
    public class UIItemListView : UITableView
    {
        #region Constants and Fields        
        #endregion

        #region Events

        public event EventHandler<UIItemListSelectEventArgs> ItemSelected;
        public event EventHandler<UIItemListLongPressEventArgs> ItemLongPress;
        public event EventHandler<UIItemListCommandEventArgs> ItemCommand;

        public new event EventHandler<EventArgs> Scrolled;
        public new event EventHandler<EventArgs> ScrollAnimationEnded;
        public new event EventHandler<EventArgs> ScrolledToTop;

        #endregion

        #region Constructors

        public UIItemListView(CGRect frame)
            : base(frame)
        {
            Initialize();
        }
            
        public UIItemListView(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        #endregion

        #region Public Methods

        internal void SelectItem(int index, object item)
        {
            OnItemSelected(index, item);
        }

        internal void LongPressItem(int index, object item)
        {
            OnItemLongPress(index, item);
        }

        internal void ExecuteCommand(string command, object userData)
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

        #endregion

        #region Event Raising Methods

        protected void OnItemSelected(int index, object item)
        {
            if(ItemSelected != null)
                ItemSelected(this, new UIItemListSelectEventArgs(index, item));
        }

        protected void OnItemLongPress(int index, object item)
        {
            if (ItemLongPress != null)
                ItemLongPress(this, new UIItemListLongPressEventArgs(index, item));
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

        #endregion

        #region Methods

        private void Initialize()
        {            
        }

        #endregion
    }
}