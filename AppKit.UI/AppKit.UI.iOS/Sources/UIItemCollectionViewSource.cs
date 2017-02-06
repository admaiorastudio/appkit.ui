namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;

    using Foundation;
    using UIKit;

    public abstract class UIItemCollectionViewSource<THolder, TItem> : UICollectionViewSource
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

        public UIItemCollectionViewSource(UIViewController viewController, string cellLayout, IEnumerable<TItem> source)
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
            
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _sourceItems.Count;
        }
                    
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            TItem item = _sourceItems[indexPath.Row];

            UICollectionViewCell view = (UICollectionViewCell)collectionView.DequeueReusableCell(_cellLayout, indexPath);
            if (view == null)
            {
                NSBundle bundle = NSBundle.MainBundle;
                view = bundle.LoadNib(_cellLayout, this, null).GetItem<UICollectionViewCell>(0);
				view.Tag = ++_associatedIndex;

                var lpgr = new UILongPressGestureRecognizer(
                    (r) =>
                    {
                        if (r.State == UIGestureRecognizerState.Began)
                        {
                            //CGPoint p = r.LocationInView(tableView);
                            NSIndexPath ip = collectionView.IndexPathForCell(view);
                            if (ip == null)
                                return;

                            LongPress(collectionView, ip);
                        }
                    });

                lpgr.MinimumPressDuration = 1.0;
                view.AddGestureRecognizer(lpgr);

                var vh = Activator.CreateInstance(typeof(THolder), view) as ItemViewHolder;
                _associatedHolders.Add(view.Tag, vh);

                _associatedItems.Add(view.Tag, item);
                GetViewCreated((THolder)vh, view, collectionView);
            }
            else
            {
				_associatedItems[view.Tag] = item;
            }

            return GetCell(collectionView, indexPath, (THolder)_associatedHolders[view.Tag], view, item);
        }
                               
        public virtual UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath, THolder holder, UICollectionViewCell cellView, TItem item)
        {
            GetCell(indexPath, holder, cellView, item);
            return cellView;
        }

        public virtual void GetCell(NSIndexPath indexPath, THolder holder, UICollectionViewCell view, TItem item)
        {
            /* Do Nothing */
        }

        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UIItemCollectionView listView = collectionView as UIItemCollectionView;
            if (listView != null)
                listView.SelectItem(indexPath.Row, _sourceItems[indexPath.Row]);

            return true;
        }

        public void LongPress(UICollectionView tableView, NSIndexPath indexPath)
        {
            UIItemCollectionView listView = tableView as UIItemCollectionView;
            if (listView != null)
                listView.LongPressItem(indexPath.Row, _sourceItems[indexPath.Row]);
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            UIItemCollectionView collectionView = scrollView as UIItemCollectionView;
            if (collectionView != null)
                collectionView.WhenScrolled();
        }

        public override void ScrollAnimationEnded(UIScrollView scrollView)
        {
            UIItemCollectionView collectionView = scrollView as UIItemCollectionView;
            if (collectionView != null)
                collectionView.WhenScrollAnimationEnded();
        }

        public override void ScrolledToTop(UIScrollView scrollView)
        {
            UIItemCollectionView collectionView = scrollView as UIItemCollectionView;
            if (collectionView != null)
                collectionView.WhenScrolledToTop();
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

        protected void ExecuteCommand(UICollectionView collectionView, string command, object userData)
        {
            UIItemCollectionView listView = collectionView as UIItemCollectionView;
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

        protected virtual void GetViewCreated(THolder holder, UICollectionViewCell view, UICollectionView parent)
        {
            /* Do Nothing */
        }  
            
        #endregion
    }
}

