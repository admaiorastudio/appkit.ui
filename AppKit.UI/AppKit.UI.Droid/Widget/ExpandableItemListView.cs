namespace AdMaiora.AppKit.UI
{
    using System;

    using Android.Content;
    using Android.Util;
    using Android.Widget;

    public class ExpandableItemListView : ExpandableListView
    {
        #region Event Handlers

        public event EventHandler<ItemListSelectEventArgs> ItemSelected;
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

        public ExpandableItemListView(Context context)
            : base(context)
        {
            Initialize();
        }

        public ExpandableItemListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize();
        }

        public ExpandableItemListView(Context context, IAttributeSet attrs, int defStyle)
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
            this.ChildClick += ExpandableItemListView_ChildClick;
        }

        #endregion

        #region Event Handlers

        private void ExpandableItemListView_ChildClick(object sender, ExpandableListView.ChildClickEventArgs e)
        {
            var o = this.ExpandableListAdapter.GetChild(e.GroupPosition, e.ChildPosition);
            var h = o as JavaHolder;
            if (h == null)
                return;

            var item = h.Instance;

            SelectItem(e.ChildPosition, item);
        }

        #endregion
    }
}