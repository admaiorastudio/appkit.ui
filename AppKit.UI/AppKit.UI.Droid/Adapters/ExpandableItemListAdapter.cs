namespace AdMaiora.AppKit.UI
{
    using System.Collections.Generic;
    using System.Linq;

    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;

    public abstract class ExpandableItemListAdapter<TGroup, TChild> : BaseExpandableListAdapter, View.IOnClickListener, View.IOnLongClickListener
    {
        #region Constants and Fields

        private Android.Support.V4.App.Fragment _context;

        private int _parentLayoutID;
        private int _childLayoutID;

        private List<KeyValuePair<TGroup, IList<TChild>>> _sourceItems;

        // This dictionary preserve a map to View hash code and 
        // an associate item, to be used later when the method GetItemForView needs it
        private int _associationIndex;
        private Dictionary<int, TChild> _associatedItems;

        #endregion

        #region Constructor and Destructor

        public ExpandableItemListAdapter(Android.Support.V4.App.Fragment context, int parentLayoutID, int childLayoutID, IEnumerable<IGrouping<TGroup, TChild>> groups)
        {
            _context = context;

            _parentLayoutID = parentLayoutID;
            _childLayoutID = childLayoutID;

            _sourceItems = new List<KeyValuePair<TGroup, IList<TChild>>>();
            foreach (IGrouping<TGroup, TChild> grouping in groups)
            {
                _sourceItems.Add(new KeyValuePair<TGroup, IList<TChild>>(grouping.Key, new List<TChild>(grouping)));
            }

            _associationIndex = 0;
            _associatedItems = new Dictionary<int, TChild>();
        }

        #endregion

        #region Indexers

        public KeyValuePair<TGroup, IList<TChild>> this[int position]
        {
            get
            {
                return _sourceItems[position];
            }
        }

        #endregion

        #region Properties

        public override int GroupCount
        {
            get
            {
                return _sourceItems.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        protected List<KeyValuePair<TGroup, IList<TChild>>> Groups
        {
            get
            {
                return _sourceItems;
            }
        }

        #endregion

        #region Public Methods

        public override int GetChildrenCount(int groupPosition)
        {
            return _sourceItems[groupPosition].Value.Count;
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return new JavaHolder(_sourceItems[groupPosition].Value[childPosition]);
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return new JavaHolder(_sourceItems[groupPosition].Key);
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            long gp = groupPosition + 1;
            gp <<= 32;
            return gp + childPosition;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        public override bool AreAllItemsEnabled()
        {
            return true;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View view, ViewGroup parent)
        {
            TGroup item = _sourceItems[groupPosition].Key;

            if (view == null && _parentLayoutID != -1)
            {
                LayoutInflater inflater = LayoutInflater.From(_context.Context);
                view = inflater.Inflate(_parentLayoutID, parent, false);

                GetGroupViewCreated(view, parent);
            }

            view.Tag = new JavaHolder(item);

            return GetGroupView(groupPosition, isExpanded, item, view, parent);
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View view, ViewGroup parent)
        {
            TChild item = _sourceItems[groupPosition].Value[childPosition];

            if (view == null && _childLayoutID != -1)
            {
                LayoutInflater inflater = LayoutInflater.From(_context.Context);
                view = inflater.Inflate(_childLayoutID, null);
                view.Tag = ++_associationIndex;
                view.Clickable = true;
                view.SetOnClickListener(this);
                view.SetOnLongClickListener(this);

                _associatedItems[(int)view.Tag] = item;

                GetChildViewCreated(view, parent);
            }
            else
            {
                _associatedItems[(int)view.Tag] = item;
            }

            return GetChildView(groupPosition, childPosition, isLastChild, item, view, parent);
        }

        public virtual View GetGroupView(int position, bool isExpanded, TGroup item, View view, ViewGroup parent)
        {
            return view;
        }

        public virtual View GetChildView(int groupPosition, int childPosition, bool isLastChild, TChild item, View view, ViewGroup parent)
        {
            return view;
        }

        public virtual void AddItem(int groupPosition, TChild item)
        {
            _sourceItems[groupPosition].Value.Add(item);
        }

        public virtual void InsertItem(int groupPosition, int position, TChild item)
        {
            _sourceItems[groupPosition].Value.Insert(position, item);
        }

        public virtual void RemoveItem(int groupPosition, int position, TChild item)
        {
            _sourceItems[groupPosition].Value.Remove(item);
        }

        public virtual void RemoveItem(int groupPosition, int position)
        {
            _sourceItems[groupPosition].Value.RemoveAt(position);
        }

        #endregion

        #region View Methods

        public void OnClick(View v)
        {
            ExpandableItemListView listView = v.Parent as ExpandableItemListView;
            if (listView == null)
                return;

            var item = GetItemFromView(v);
            listView.SelectItem(GetItemIndex(item), item);
        }

        public bool OnLongClick(View v)
        {
            ExpandableItemListView listView = v.Parent as ExpandableItemListView;
            if (listView == null)
                return false;

            var item = GetItemFromView(v);
            listView.LongPressItem(GetItemIndex(item), item);

            return true;
        }

        #endregion

        #region Methods

        protected TChild GetItemFromView(View view)
        {
            while (view != null)
            {
                int associatedIndex = (int)view.Tag;
                if (_associatedItems.ContainsKey(associatedIndex))
                {
                    object instance = _associatedItems[associatedIndex];

                    if (instance is TChild)
                        return (TChild)instance;
                }

                view = view.Parent as View;
            }

            return default(TChild);
        }

        protected void ExecuteCommand(View sender, string command, object userData)
        {
            ExpandableItemListView listView = sender as ExpandableItemListView;
            if (listView != null)
                listView.ExecuteCommand(command, userData);
        }

        protected virtual void GetGroupViewCreated(View convertView, ViewGroup parent)
        {
            /* Do Nothing */
        }

        protected virtual void GetChildViewCreated(View convertView, ViewGroup parent)
        {
            /* Do Nothing */
        }

        private int GetItemIndex(TChild item)
        {
            int index = -1;
            foreach(var kvp in _sourceItems)
            {
                index++;
                foreach(var i in kvp.Value)
                {
                    index++;
                    if (i.GetHashCode() == item.GetHashCode())
                        return index;
                }               
            }

            return -1;
        }

        #endregion
    }
}