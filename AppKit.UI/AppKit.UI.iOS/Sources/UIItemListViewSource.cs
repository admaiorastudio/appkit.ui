namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;

    using Foundation;
    using UIKit;
    using CoreGraphics;

    public abstract class ItemViewHolder
    {
        public ItemViewHolder(UIView itemView)
        {
            ViewBuilder.GetWidgets(this, itemView);
        }
    }

    public abstract class UIItemListViewSource<THolder, TItem> : UITableViewSource
        where THolder : ItemViewHolder
    {
        #region Constants and Fields

        private UIViewController _viewController;
       
        private string _cellLayout;

        private List<TItem> _sourceItems;

        private nint _associatedIndex;

        // This dictionary maps each UITableViewCell created to it's related TItem
        // Will be used as hash table for the GetItemFromView() method         
        private Dictionary<nint, TItem> _associatedItems;

        // This dictionary maps each UITableViewCell created to it's related TOutlets
        // Will be used as hash table for the GetView() method         
        private Dictionary<nint, ItemViewHolder> _associatedHolders;

        #endregion

        #region Constructors and Destructors

        public UIItemListViewSource(UIViewController viewController, string cellLayout, IEnumerable<TItem> source)
            : base()
        {
            _viewController = viewController;

            _cellLayout = cellLayout;

            _sourceItems = new List<TItem>(source);

            _associatedIndex = 0;

            _associatedItems = new Dictionary<nint, TItem>();
            _associatedHolders = new Dictionary<nint, ItemViewHolder>();
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

        public int ItemCount
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

        protected UIViewController ViewController
        {
            get
            {
                return _viewController;
            }
        }

        #endregion

        #region Public Methods

        public int GetItemIndex(TItem item)
        {
            if(_sourceItems == null
                || _sourceItems.Count == 0)
            {
                return -1;
            }

            return _sourceItems.IndexOf(item);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _sourceItems.Count;
        }
                                    
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
			TItem item = _sourceItems[GetSourceIndexFromIndexPath(tableView, indexPath)];

            UITableViewCell view = tableView.DequeueReusableCell(_cellLayout);
            if (view == null)
            {
                NSBundle bundle = NSBundle.MainBundle;
                view = bundle.LoadNib(_cellLayout, this, null).GetItem<UITableViewCell>(0);
				view.Tag = ++_associatedIndex;

                var lpgr = new UILongPressGestureRecognizer(
                    (r) =>
                    {
                        if (r.State == UIGestureRecognizerState.Began)
                        {
                            //CGPoint p = r.LocationInView(tableView);
                            NSIndexPath ip = tableView.IndexPathForCell(view);
                            if (ip == null)
                                return;

                            LongPress(tableView, ip);
                        }
                    });

                lpgr.MinimumPressDuration = 1.0;
                view.AddGestureRecognizer(lpgr);

                var vh = Activator.CreateInstance(typeof(THolder), view) as ItemViewHolder;
                _associatedHolders.Add(view.Tag, vh);

                _associatedItems.Add(view.Tag, item);
                GetViewCreated((THolder)vh, view, tableView);
            }
            else
            {
				_associatedItems[view.Tag] = item;
            }
            
            return GetCell(tableView, indexPath, (THolder)_associatedHolders[view.Tag], view, item);
        }

        public virtual UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath, THolder holder, UITableViewCell cellView, TItem item)
        {
            GetCell(indexPath, holder, cellView, item);
            return cellView;
        }

        public virtual void GetCell(NSIndexPath indexPath, THolder holder, UITableViewCell view, TItem item)
        {
            /* Do Nothing */
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            UIItemListView listView = tableView as UIItemListView;
            if (listView != null)
				listView.SelectItem(GetSourceIndexFromIndexPath(tableView, indexPath), _sourceItems[GetSourceIndexFromIndexPath(tableView, indexPath)]);
        }
        
        public void LongPress(UITableView tableView, NSIndexPath indexPath)
        {
            UIItemListView listView = tableView as UIItemListView;
            if (listView != null)
                listView.LongPressItem(GetSourceIndexFromIndexPath(tableView, indexPath), _sourceItems[GetSourceIndexFromIndexPath(tableView, indexPath)]);
        }

        public override void Scrolled(UIScrollView scrollView)
		{
			UIItemListView listView = scrollView as UIItemListView;
			if (listView != null)
				listView.WhenScrolled();
		}

		public override void ScrollAnimationEnded(UIScrollView scrollView)
		{
			UIItemListView listView = scrollView as UIItemListView;
			if (listView != null)
				listView.WhenScrollAnimationEnded();
		}

		public override void ScrolledToTop(UIScrollView scrollView)
		{
			UIItemListView listView = scrollView as UIItemListView;
			if (listView != null)
				listView.WhenScrolledToTop();
		}

        public virtual void AddItem(TItem item)
        {
            _sourceItems.Add(item);
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

        #region Methods

        protected void ExecuteCommand(UITableView tableView, string command, object userData)
        {
            UIItemListView listView = tableView as UIItemListView;
            if (listView != null)
                listView.ExecuteCommand(command, userData);
        }

        protected TItem GetItemFromView(UIView view)
        {
            while (view != null)
            {
                nint associatedIndex = view.Tag;
				if(_associatedItems.ContainsKey(associatedIndex))    
                {
					object instance = _associatedItems[associatedIndex];

                    if (instance is TItem)
                        return (TItem)instance;
                }

                view = view.Superview as UIView;
            }

            return default(TItem);
        }
           
        protected virtual void GetViewCreated(THolder holder, UITableViewCell view, UITableView parent)
        {
            /* Do Nothing */
        }

        private int GetSourceIndexFromIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            int index = 0;
            for (int i = 0; i < indexPath.Section; index += (int)this.RowsInSection(tableView, i++)) ;
            return index + indexPath.Row;
        }

        public object GetSourceItemFromIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            int index = GetSourceIndexFromIndexPath(tableView, indexPath);
            return this.SourceItems[index];
        }

        #endregion
    }
}