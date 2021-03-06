namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;

    public abstract class ExpandableItemListAdapter<THolder, TGroup, TItem> : BaseExpandableListAdapter, View.IOnClickListener, View.IOnLongClickListener
        where THolder : ItemViewHolder
    {
        #region Constants and Fields

        private Activity _context;

        private int _parentLayoutID;
        private int _childLayoutID;

        private List<KeyValuePair<TGroup, IList<TItem>>> _sourceItems;

        private int _associationIndex;

        // This dictionary maps each UITableViewCell created to it's related TItem
        // Will be used as hash table for the GetItemFromView() method           
        private Dictionary<int, TItem> _associatedItems;

        // This dictionary maps each UITableViewCell created to it's related TOutlets
        // Will be used as hash table for the GetView() method         
        private Dictionary<int, ItemViewHolder> _associatedHolders;
        
        #endregion

        #region Constructor and Destructor

        public ExpandableItemListAdapter(Activity context, int parentLayoutID, int childLayoutID, IEnumerable<IGrouping<TGroup, TItem>> groups)
        {
            _context = context;

            _parentLayoutID = parentLayoutID;
            _childLayoutID = childLayoutID;

            _sourceItems = new List<KeyValuePair<TGroup, IList<TItem>>>();
            foreach (IGrouping<TGroup, TItem> grouping in groups)
            {
                _sourceItems.Add(new KeyValuePair<TGroup, IList<TItem>>(grouping.Key, new List<TItem>(grouping)));
            }

            _associationIndex = 0;

            _associatedItems = new Dictionary<int, TItem>();
            _associatedHolders = new Dictionary<int, ItemViewHolder>();
        }

        public ExpandableItemListAdapter(Android.Support.V4.App.Fragment context, int parentLayoutID, int childLayoutID, IEnumerable<IGrouping<TGroup, TItem>> groups)
            : this(context.Activity, parentLayoutID, childLayoutID, groups)
        {
        }

        #endregion

        #region Indexers

        public KeyValuePair<TGroup, IList<TItem>> this[int position]
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

        protected List<KeyValuePair<TGroup, IList<TItem>>> Groups
        {
            get
            {
                return _sourceItems;
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
                LayoutInflater inflater = LayoutInflater.From(_context);
                view = inflater.Inflate(_parentLayoutID, parent, false);

                GetGroupViewCreated(view, parent);
            }

            view.Tag = new JavaHolder(item);

            return GetGroupView(groupPosition, isExpanded, item, view, parent);
        }

        public virtual View GetGroupView(int position, bool isExpanded, TGroup item, View view, ViewGroup parent)
        {
            return view;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View view, ViewGroup parent)
        {
            TItem item = _sourceItems[groupPosition].Value[childPosition];

            if (view == null && _childLayoutID != -1)
            {
                LayoutInflater inflater = LayoutInflater.From(_context);
                view = inflater.Inflate(_childLayoutID, null);
                view.Tag = ++_associationIndex;
                view.Clickable = true;
                view.SetOnClickListener(this);
                view.SetOnLongClickListener(this);

                var vh = Activator.CreateInstance(typeof(THolder), view) as ItemViewHolder;
                _associatedHolders.Add((int)view.Tag, vh);

                _associatedItems[(int)view.Tag] = item;
                GetChildViewCreated((THolder)vh, view, parent);
            }
            else
            {
                _associatedItems[(int)view.Tag] = item;
            }

            return GetChildView(parent, groupPosition, childPosition, isLastChild, (THolder)_associatedHolders[(int)view.Tag], view, item);
        }

        public virtual View GetChildView(ViewGroup parent, int groupPosition, int childPosition, bool isLastChild, THolder holder, View view, TItem item)
        {
            GetChildView(groupPosition, childPosition, holder, view, item);
            return view;
        }

        public virtual void GetChildView(int groupPosition, int childPosition, THolder holder, View view, TItem item)
        {
            /* Do Nothing */
        }

        public virtual void AddItem(int groupPosition, TItem item)
        {
            _sourceItems[groupPosition].Value.Add(item);
        }

        public virtual void InsertItem(int groupPosition, int position, TItem item)
        {
            _sourceItems[groupPosition].Value.Insert(position, item);
        }

        public virtual void RemoveItem(int groupPosition, int position, TItem item)
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
            ExpandableItemListView listView = sender as ExpandableItemListView;
            if (listView != null)
                listView.ExecuteCommand(command, userData);
        }

        protected virtual void GetGroupViewCreated(View view, ViewGroup parent)
        {
            /* Do Nothing */
        }

        protected virtual void GetChildViewCreated(THolder holder, View view, ViewGroup parent)
        {
            /* Do Nothing */
        }

        private int GetItemIndex(TItem item)
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