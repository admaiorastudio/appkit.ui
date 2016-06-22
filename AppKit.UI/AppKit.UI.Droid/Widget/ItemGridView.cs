namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Content;
    using Android.Util;
    using Android.Widget;

    public class ItemGridView : GridView
    {
        #region Events

        public event EventHandler<ItemListSelectEventArgs> ItemSelected;
        public event EventHandler<ItemListCommandEventArgs> ItemCommand;

        #endregion

        #region Constructors and Destructors

        public ItemGridView(Context context)
            : base(context)
        {
            Initialize();
        }

        public ItemGridView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
        }

        public ItemGridView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize();
        }

        #endregion

        #region Public Methods

        public void SelectItem(int index, object item)
        {
            OnItemSelected(index, item);
        }

        public void ExecuteCommand(string command, object userData)
        {
            OnItemCommand(command, userData);
        }

        public void ReloadData()
        {
            var adapter = this.Adapter as BaseAdapter;
            if (adapter != null)
                adapter.NotifyDataSetChanged();
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

            var item = h.Instance;

            SelectItem(e.Position, item);
        }

        #endregion
    }
}