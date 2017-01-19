using System;
using System.Collections.Generic;
using UIKit;

namespace FingerPaint
{
    class PickerDataModel<T> : UIPickerViewModel where T : class
    {
        public event EventHandler<EventArgs> ValueChanged;

        int selectedIndex = 0;

        public PickerDataModel()
        {
            Items = new List<T>();
        }

        public IList<T> Items { private set; get; }

        public T SelectedItem
        {
            get
            {
                return Items != null &&
                       selectedIndex >= 0 &&
                       selectedIndex < Items.Count ? Items[selectedIndex] : null;
            }
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return Items != null ? Items.Count : 0;
        }

        public override string GetTitle(UIPickerView picker, nint row, nint component)
        {
            return Items != null && Items.Count > row ? Items[(int)row].ToString() : null;
        }

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override void Selected(UIPickerView picker, nint row, nint component)
        {
            selectedIndex = (int)row;
            ValueChanged?.Invoke(this, new EventArgs());
        }
    }
}
