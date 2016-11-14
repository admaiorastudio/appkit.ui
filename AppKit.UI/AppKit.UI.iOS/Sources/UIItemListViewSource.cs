namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;

    using Foundation;
    using UIKit;
    using CoreGraphics;

    public abstract class UIItemListViewSource<T> : UITableViewSource
    {
        #region Constants and Fields

        private UIViewController _viewController;
       
        private string _cellLayout;

        private List<T> _sourceItems;

        // This dictionary preserve a map to UITableViewCell hash code and 
        // an associate item, to be used later when the method GetItemForView needs it
        private nint _associatedIndex;
        private Dictionary<nint, T> _associatedItems;		

        #endregion

        #region Constructors and Destructors

        public UIItemListViewSource(UIViewController viewController, string cellLayout, IEnumerable<T> source)
            : base()
        {
            _viewController = viewController;

            _cellLayout = cellLayout;

            _sourceItems = new List<T>(source);

            _associatedIndex = 0;
            _associatedItems = new Dictionary<nint, T>();			
        }

        #endregion

        #region Indexers

        public T this[int position]
        {
            get 
            { 
                return _sourceItems[position]; 
            }
        }

        #endregion

        #region Properties

        public int Count
        {
            get 
            { 
                return _sourceItems.Count; 
            }
        }
            
        protected UIViewController ViewController
        {
            get
            {
                return _viewController;
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

        public int GetItemIndex(T item)
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
			T item = _sourceItems[GetSourceIndexFromIndexPath(tableView, indexPath)];

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

                _associatedItems.Add(view.Tag, item);
                GetViewCreated(tableView, view);
            }
            else
            {
				_associatedItems[view.Tag] = item;
            }

            return GetCell(tableView, indexPath, view, item);
        }

        public virtual UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath, UITableViewCell cellView, T item)
        {
            return cellView;
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

        public virtual void AddItem(T item)
        {
            _sourceItems.Add(item);
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

        #region Methods

        protected void ExecuteCommand(UITableView tableView, string command, object userData)
        {
            UIItemListView listView = tableView as UIItemListView;
            if (listView != null)
                listView.ExecuteCommand(command, userData);
        }

        protected T GetItemFromView(UIView view)
        {
            while (view != null)
            {
                nint associatedIndex = view.Tag;
				if(_associatedItems.ContainsKey(associatedIndex))    
                {
					object instance = _associatedItems[associatedIndex];

                    if (instance is T)
                        return (T)instance;
                }

                view = view.Superview as UIView;
            }

            return default(T);
        }
           
        protected virtual void GetViewCreated(UITableView tableView, UITableViewCell cellView)
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