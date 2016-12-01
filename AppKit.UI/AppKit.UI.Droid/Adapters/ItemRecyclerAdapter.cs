namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;

    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;
    using Android.Support.V7.Widget;

    public class ItemViewHolder : RecyclerView.ViewHolder
    {
        public ItemViewHolder(View itemView)
            : base(itemView)
        {
            ViewBuilder.GetWidgets(this, itemView);
        }
    }

    public class ItemRecyclerAdapter<THolder, TItem> : RecyclerView.Adapter, View.IOnClickListener, View.IOnLongClickListener
        where THolder : ItemViewHolder
    {
        #region Constants and Fields

        private Activity _context;

        private int _viewLayoutID;

        private List<TItem> _sourceItems;

        // This dictionary preserve a map to View hash code and 
        // an associate item, to be used later when the method GetItemForView needs it
        private int _associationIndex;
        private Dictionary<int, TItem> _associatedItems;

        #endregion

        #region Constructors

        public ItemRecyclerAdapter(Activity context, int viewLayoutID, IEnumerable<TItem> source)
        {
            _context = context;

            _viewLayoutID = viewLayoutID;

            _sourceItems = new List<TItem>(source);

            _associationIndex = 0;
            _associatedItems = new Dictionary<int, TItem>();
        }

        public ItemRecyclerAdapter(Android.Support.V4.App.Fragment context, int viewLayoutID, IEnumerable<TItem> source)
            : this(context.Activity, viewLayoutID, source)
        {
        }


        #endregion

        #region Indexers

        public TItem this[int position]
        {
            get
            {
                return _sourceItems[position];
            }
        }

        #endregion

        #region Properties

        public override int ItemCount
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

        #endregion

        #region Public Methods

        public int GetItemIndex(TItem item)
        {
            if (_sourceItems == null
                || _sourceItems.Count == 0)
            {
                return -1;
            }

            return _sourceItems.IndexOf(item);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) 
        {
            LayoutInflater inflater = LayoutInflater.From(_context);
            Android.Views.View view = inflater.Inflate(_viewLayoutID, parent, false);
            view.Tag = ++_associationIndex;
            view.Clickable = true;            
            view.SetOnClickListener(this);
            view.SetOnLongClickListener(this);                        
            
            var vh =
                Activator.CreateInstance(typeof(THolder), view) as RecyclerView.ViewHolder;

            GetViewCreated(vh, view, parent);

            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TItem item = _sourceItems[position];

            _associatedItems[(int)holder.ItemView.Tag] = item;

            GetView(position, (THolder)holder, holder.ItemView, item);
        }

        public virtual void GetView(int postion, THolder holder, View view, TItem item)
        {
            // Bind here
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
            ItemRecyclerView recyclerView = v.Parent as ItemRecyclerView;
            if (recyclerView == null)
                return;
            
            var item = GetItemFromView(v);
            recyclerView.SelectItem(_sourceItems.IndexOf(item), item);
        }

        public bool OnLongClick(View v)
        {
            ItemRecyclerView recyclerView = v.Parent as ItemRecyclerView;
            if (recyclerView == null)
                return false;

            var item = GetItemFromView(v);
            recyclerView.LongPressItem(_sourceItems.IndexOf(item), item);

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
            ItemRecyclerView recyclerView = sender as ItemRecyclerView;
            if (recyclerView != null)
                recyclerView.ExecuteCommand(command, userData);
        }

        protected virtual void GetViewCreated(RecyclerView.ViewHolder holder, View view, ViewGroup parent)
        {
            /* Do Nothing */
        }

        #endregion
    }
}