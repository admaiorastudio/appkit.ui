namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Content;
    using Android.Util;
    using Android.Widget;

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

        public void SelectItem(int index, object item)
        {
            OnItemSelected(index, item);
        }

        public void ExecuteCommand(string command, object userData)
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

        protected void OnItemCommand(string command, object userData)
        {
            if (ItemCommand != null)
                ItemCommand(this, new ItemListCommandEventArgs(command, userData));
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            this.ItemClick += ItemListView_ItemClick;
        }

        #endregion

        #region Event Handlers

        private void ItemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var o = this.Adapter.GetItem(e.Position);
            var h = o as JavaHolder;
            if (h == null)
                return;

            var item = h.Instance;

            SelectItem(e.Position, item);
        }

        #endregion
    }
}