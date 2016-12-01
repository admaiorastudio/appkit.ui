namespace AdMaiora.AppKit.UI
{
    using System.Collections.Generic;

    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;

    public abstract class ItemListAdapter<T> : BaseAdapter<T>, View.IOnClickListener, View.IOnLongClickListener
    {
        #region Constants and Fields

        private Activity _context;

        private int _cellLayoutID;

        private List<T> _sourceItems;

        // This dictionary preserve a map to View hash code and 
        // an associate item, to be used later when the method GetItemForView needs it
        private int _associationIndex;
        private Dictionary<int, T> _associatedItems;

        #endregion

        #region Constructor and Destructor

        public ItemListAdapter(Activity context, int cellLayoutID, IEnumerable<T> source)
        {
            _context = context;

            _cellLayoutID = cellLayoutID;

            _sourceItems = new List<T>(source);

            _associationIndex = 0;
            _associatedItems = new Dictionary<int, T>();
        }

        public ItemListAdapter(Android.Support.V4.App.Fragment context, int cellLayoutID, IEnumerable<T> source)
            : this(context.Activity, cellLayoutID, source)
        {
        }

        #endregion

        #region Indexers

        public override T this[int position]
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

        protected List<T> SourceItems
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

        public int GetItemIndex(T item)
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
            T item = _sourceItems[position];

            View view = convertView;
            if (view == null && _cellLayoutID != -1)
            {
                LayoutInflater inflater = LayoutInflater.From(_context);
                view = inflater.Inflate(_cellLayoutID, parent, false);
                view.Tag = ++_associationIndex;
                view.Clickable = true;
                view.SetOnClickListener(this);
                view.SetOnLongClickListener(this);

                _associatedItems[(int)view.Tag] = item;

                GetViewCreated(view, parent);
            }
            else
            {
                _associatedItems[(int)view.Tag] = item;
            }

            return GetView(position, item, view, parent);
        }

        public virtual View GetView(int position, T item, View view, ViewGroup parent)
        {
            return view;
        }

        public virtual void AddItem(T item)
        {
            _sourceItems.Add(item);
        }

        public virtual void InsertItem(int position, T item)
        {
            _sourceItems.Insert(position, item);
        }

        public virtual void RemoveItem(T item)
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

        protected T GetItemFromView(View view)
        {
            while (view != null)
            {
                int associatedIndex = (int)view.Tag;
                if (_associatedItems.ContainsKey(associatedIndex))
                {
                    object instance = _associatedItems[associatedIndex];

                    if (instance is T)
                        return (T)instance;
                }

                view = view.Parent as View;
            }

            return default(T);
        }

        protected void ExecuteCommand(View sender, string command, object userData)
        {
            ItemListView listView = sender as ItemListView;
            if (listView != null)
                listView.ExecuteCommand(command, userData);
        }

        protected virtual void GetViewCreated(View view, ViewGroup parent)
        {
            /* Do Nothing */
        }

        #endregion
    }
}