using System;
using System.Collections.Generic;
using UIKit;

namespace FingerPaint
{
    class NamedValue<T>
    {
        public NamedValue(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public string Name { private set; get; }

        public T Value { private set; get; }
    }

    class PickerDataModel<T> : UIPickerViewModel 
    {
        public event EventHandler<EventArgs> ValueChanged;

        int selectedIndex = 0;

        public PickerDataModel()
        {
            Items = new List<NamedValue<T>>();
        }

        public IList<NamedValue<T>> Items { private set; get; }

        public NamedValue<T> SelectedItem
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
            return Items != null && Items.Count > row ? Items[(int)row].Name : null;
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
