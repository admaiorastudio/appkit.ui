namespace AdMaiora.AppKit.UI
{
    using System.Collections.Generic;
    using System.Linq;

    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;

    public abstract class ExpandableItemListAdapter<TGroup, TChild> : BaseExpandableListAdapter
    {
        #region Constants and Fields

        private Activity _context;

        private int _parentLayoutID;
        private int _childLayoutID;

        private List<KeyValuePair<TGroup, IList<TChild>>> _sourceItems;

        // This dictionary preserve a map to View hash code and 
        // an associate item, to be used later when the method GetItemForView needs it
        private Dictionary<int, TChild> _associatedItems;

        #endregion

        #region Constructor and Destructor

        public ExpandableItemListAdapter(Activity context, int parentLayoutID, int childLayoutID, IEnumerable<IGrouping<TGroup, TChild>> groups)
        {
            _context = context;

            _parentLayoutID = parentLayoutID;
            _childLayoutID = childLayoutID;

            _sourceItems = new List<KeyValuePair<TGroup, IList<TChild>>>();
            foreach (IGrouping<TGroup, TChild> grouping in groups)
            {
                _sourceItems.Add(new KeyValuePair<TGroup, IList<TChild>>(grouping.Key, new List<TChild>(grouping)));
            }

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

        protected Activity Activity
        {
            get
            {
                return _context;
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
                LayoutInflater inflater = (LayoutInflater)this.Activity.GetSystemService(Context.LayoutInflaterService);
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
                LayoutInflater inflater = (LayoutInflater)this.Activity.GetSystemService(Activity.LayoutInflaterService);
                view = inflater.Inflate(_childLayoutID, null);

                _associatedItems[view.GetHashCode()] = item;

                GetChildViewCreated(view, parent);
            }
            else
            {
                _associatedItems[view.GetHashCode()] = item;
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
            NotifyDataSetChanged();
        }

        public virtual void RemoveItem(int groupPosition, int position, TChild item)
        {
            _sourceItems[groupPosition].Value.Remove(item);
            NotifyDataSetChanged();
        }

        #endregion

        #region Methods

        protected TChild GetItemFromView(View view)
        {
            while (view != null)
            {
                if (_associatedItems.ContainsKey(view.GetHashCode()))
                {
                    object instance = _associatedItems[view.GetHashCode()];

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

        #endregion
    }
}