namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Content;
    using Android.Util;
    using Android.Widget;
    using Android.Views;

    public class ItemGridView : GridView
    {
        #region Events

        public new event EventHandler<ItemListSelectEventArgs> ItemSelected;
        public event EventHandler<ItemListLongPressEventArgs> ItemLongPress;
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