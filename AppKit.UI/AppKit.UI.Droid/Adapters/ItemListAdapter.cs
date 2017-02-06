namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;

    public abstract class ItemListAdapter<THolder, TItem> : BaseAdapter<TItem>, View.IOnClickListener, View.IOnLongClickListener
        where THolder : ItemViewHolder
    {
        #region Constants and Fields

        private Activity _context;

        private int _cellLayoutID;

        private List<TItem> _sourceItems;

        private int _associationIndex;

        // This dictionary maps each View created to it's related TItem
        // Will be used as hash table for the GetItemFromView() method             
        private Dictionary<int, TItem> _associatedItems;

        // This dictionary maps each UITableViewCell created to it's related TOutlets
        // Will be used as hash table for the GetView() method         
        private Dictionary<int, ItemViewHolder> _associatedHolders;
    
        #endregion

        #region Constructor and Destructor

        public ItemListAdapter(Activity context, int cellLayoutID, IEnumerable<TItem> source)
        {
            _context = context;

            _cellLayoutID = cellLayoutID;

            _sourceItems = new List<TItem>(source);

            _associationIndex = 0;

            _associatedItems = new Dictionary<int, TItem>();
            _associatedHolders = new Dictionary<int, ItemViewHolder>();
        }

        public ItemListAdapter(Android.Support.V4.App.Fragment context, int cellLayoutID, IEnumerable<TItem> source)
            : this(context.Activity, cellLayoutID, source)
        {
        }

        #endregion

        #region Indexers

        public override TItem this[int position]
        {
            get
            {
                return _sourceItems[position];
            }
        }

        #endregion

        #region Properties

        public override int Count
        {
            get
            {
                return _sourceItems.Count;
            }
        }

        protected List<TItem> SourceItems
        {
            get
            {
                return _sourceItems;
            }
            set
            {
                _sourceItems = value;
            }
        }

        protected Activity Context
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region Public Methods

        public override Java.Lang.Object GetItem(int position)
        {
            return new JavaHolder(_sourceItems[position]);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public int GetItemIndex(TItem item)
        {
            if (_sourceItems == null
                || _sourceItems.Count == 0)
            {
                return -1;
            }

            return _sourceItems.IndexOf(item);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            TItem item = _sourceItems[position];

            View view = convertView;
            if (view == null && _cellLayoutID != -1)
            {
                LayoutInflater inflater = LayoutInflater.From(_context);
                view = inflater.Inflate(_cellLayoutID, parent, false);
                view.Tag = ++_associationIndex;
                view.Clickable = true;
                view.SetOnClickListener(this);
                view.SetOnLongClickListener(this);

                var vh = Activator.CreateInstance(typeof(THolder), view) as ItemViewHolder;
                _associatedHolders.Add((int)view.Tag, vh);

                _associatedItems[(int)view.Tag] = item;
                GetViewCreated((THolder)vh, view, parent);
            }
            else
            {
                _associatedItems[(int)view.Tag] = item;
            }

            return GetView(parent, position, (THolder)_associatedHolders[(int)view.Tag], view, item);
        }

        public virtual View GetView(ViewGroup parent, int position, THolder holder, View view, TItem item)
        {
            GetView(position, holder, view, item);
            return view;
        }

        public virtual void GetView(int position, THolder holder, View view, TItem item)
        {
            /* Do Nothing */
        }

        public virtual void AddItem(TItem item)
        {
            _sourceItems.Add(item);
        }

        public virtual void InsertItem(int position, TItem item)
        {
            _sourceItems.Insert(position, item);
        }

        public virtual void RemoveItem(TItem item)
        {
            _sourceItems.Remove(item);
        }

        public virtual void RemoveItem(int position)
        {
            _sourceItems.RemoveAt(position);
        }

        #endregion

        #region View Methods

        public void OnClick(View v)
        {
            ItemListView listView = v.Parent as ItemListView;
            if (listView == null)
                return;

            var item = GetItemFromView(v);
            listView.SelectItem(_sourceItems.IndexOf(item), item);
        }

        public bool OnLongClick(View v)
        {
            ItemListView listView = v.Parent as ItemListView;
            if (listView == null)
                return false;

            var item = GetItemFromView(v);
            listView.LongPressItem(_sourceItems.IndexOf(item), item);

            return true;
        }

        #endregion


        #region Methods

        protected TItem GetItemFromView(View view)
        {
            while (view != null)
            {
                int associatedIndex = (int)view.Tag;
                if (_associatedItems.ContainsKey(associatedIndex))
                {
                    object instance = _associatedItems[associatedIndex];

                    if (instance is TItem)
                        return (TItem)instance;
                }

                view = view.Parent as View;
            }

            return default(TItem);
        }

        protected void ExecuteCommand(View sender, string command, object userData)
        {
            ItemListView listView = sender as ItemListView;
            if (listView != null)
                listView.ExecuteCommand(command, userData);
        }

        protected virtual void GetViewCreated(THolder holder, View view, ViewGroup parent)
        {
            /* Do Nothing */
        }

        #endregion
    }
}