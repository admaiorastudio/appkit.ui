namespace AdMaiora.AppKit.UI
{
    using System;

    using UIKit;

    public static class UIPickerViewExtension
    {
        public static T GetSelectedItem<T>(this UIPickerView picker)
        {
            int index = (int)picker.SelectedRowInComponent(0);
            if(index == -1)
                return default(T);

            var source = picker.Model as UIPickerListSource<T>;
            if(source == null)
                throw new InvalidOperationException("Unable to get selected item. Model should be a UIPickerListSource type");

            return source.GetItemAtIndex(index);

        }
    }
}