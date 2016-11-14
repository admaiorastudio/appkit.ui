namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Content;
    using Android.Util;
    using Android.Widget;
    using Android.Views;

    public class ItemListSelectEventArgs : EventArgs
    {
        public ItemListSelectEventArgs(int index, object item)
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

    public class ItemListLongPressEventArgs : EventArgs
    {
        public ItemListLongPressEventArgs(int index, object item)
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

    public class ItemListCommandEventArgs : EventArgs
    {
        public ItemListCommandEventArgs(string command, object userData)
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

    public class ItemListView : ListView
    {
        #region Event Handlers

        public new event EventHandler<ItemListSelectEventArgs> ItemSelected;
        public event EventHandler<ItemListLongPressEventArgs> ItemLongPress;
        public event EventHandler<ItemListCommandEventArgs> ItemCommand;

        #endregion

        #region Properties

        public int VerticalContentOffset
        {
            get
            {
                return this.ComputeVerticalScrollOffset();
            }
        }

        #endregion

        #region Constructors and Destructors

        public ItemListView(Context context)
            : base(context)
        {
            Initialize();
        }

        public ItemListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
        }

        public ItemListView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize();
        }

        #endregion

        #region Public Methods

        public void ReloadData()
        {
            var adapter = this.Adapter as BaseAdapter;
            if (adapter != null)
                adapter.NotifyDataSetChanged();
        }

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

        #endregion

        #region Event Raising Methods

        protected void OnItemSelected(int index, object item)
        {
            if (ItemSelected != null)
                ItemSelected(this, new ItemListSelectEventArgs(index, item));
        }

        protected void OnItemLongPress(int index, object item)
        {
            if (ItemLongPress != null)
                ItemLongPress(this, new ItemListLongPressEventArgs(index, item));
        }

        protected void OnItemCommand(string command, object userData)
        {
            if (ItemCommand != null)
                ItemCommand(this, new ItemListCommandEventArgs(command, userData));
        }
        
        #endregion

        #region Methods

        private void Initialize()
        {      
        }

        #endregion

        #region Event Handlers
        #endregion
    }
}