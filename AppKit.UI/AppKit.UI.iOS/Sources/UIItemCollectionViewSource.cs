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
        private Dictionary<int, T> _associatedItems;

        #endregion

        #region Constructors and Destructors

        public UIItemCollectionViewSource(UIViewController viewController, string cellLayout, IEnumerable<T> source)
        {
            _viewController = viewController;

            _cellLayout = cellLayout;

            _sourceItems = new List<T>(source);

            _associatedItems = new Dictionary<int, T>();
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
                       
            collectionView.RegisterNibForCell(UINib.FromName(_cellLayout, NSBundle.MainBundle), new NSString(_cellLayout));
            UICollectionViewCell view = (UICollectionViewCell)collectionView.DequeueReusableCell(new NSString(_cellLayout), indexPath);
            if (String.IsNullOrEmpty(view.RestorationIdentifier))
            {
                view.RestorationIdentifier = indexPath.ToString();

                _associatedItems.Add(view.GetHashCode(), item);
                GetViewCreated(collectionView, view);
            }
            else
            {               
                _associatedItems[view.GetHashCode()] = item;
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
                if(_associatedItems.ContainsKey(view.GetHashCode()))    
                {
                    object instance = _associatedItems[view.GetHashCode()];

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

