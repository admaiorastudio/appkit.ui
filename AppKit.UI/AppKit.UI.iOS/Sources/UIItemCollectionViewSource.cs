namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;

    using Foundation;
    using UIKit;

    public abstract class UIItemCollectionViewSource<T> : UICollectionViewSource
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

        public UIItemCollectionViewSource(UIViewController viewController, string cellLayout, IEnumerable<T> source)
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
            
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _sourceItems.Count;
        }
                    
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            T item = _sourceItems[indexPath.Row];

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

                _associatedItems.Add(view.Tag, item);
                GetViewCreated(collectionView, view);
            }
            else
            {
				_associatedItems[view.Tag] = item;
            }

            return GetCell(collectionView, indexPath, view, item);
        }
                               
        public virtual UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath, UICollectionViewCell cellView, T item)
        {
            return cellView;
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

        protected void ExecuteCommand(UICollectionView collectionView, string command, object userData)
        {
            UIItemCollectionView listView = collectionView as UIItemCollectionView;
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

        protected virtual void GetViewCreated(UICollectionView collectionView, UICollectionViewCell cellView)
        {
            /* Do Nothing */
        }  
            
        #endregion
    }
}

