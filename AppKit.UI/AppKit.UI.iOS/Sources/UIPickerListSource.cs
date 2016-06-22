namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;

    using UIKit;

    public class UIPickerListSource<T> : UIPickerViewModel
    {
        #region Constants and Fields

        private List<T> _sourceItems;

        #endregion

        #region Constructors

        public UIPickerListSource(IEnumerable<T> items)
        {
            _sourceItems = new List<T>(items);
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

        #endregion

        #region Public Methods

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return _sourceItems.Count;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {             
            return _sourceItems[(int)row].ToString();
        }

        public T GetItemAtIndex(int index)
        {
            return _sourceItems[index];
        }
            
        #endregion
    }
}

