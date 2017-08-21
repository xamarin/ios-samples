using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace PickerControl
{
    public partial class PickerViewController : UIViewController
    {
        public PickerViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            var pickerModel = new PeopleModel(personLabel);

            personPicker.Model = pickerModel;
            personPicker.ShowSelectionIndicator = true;
        }
    }

    public class PeopleModel : UIPickerViewModel
    {
        public string[] names = new string[] {
                "Amy Burns",
                "Kevin Mullins",
                "Craig Dunn",
                "Joel Martinez",
                "Charles Petzold",
                "David Britch",
                "Mark McLemore",
                "Tom Opegenorth",
                "Joseph Hill",
                "Miguel De Icaza"
            };

        private UILabel personLabel;

        public PeopleModel(UILabel personLabel)
        {
            this.personLabel = personLabel;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
		{
			return 2;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return names.Length;
		}

		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			if (component == 0)
				return names[row];
			else
				return row.ToString();
		}

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
            
            personLabel.Text = $"This person is: {names[pickerView.SelectedRowInComponent(0)]},\n they are number {pickerView.SelectedRowInComponent(1)}";
		}

		public override nfloat GetComponentWidth(UIPickerView picker, nint component)
		{
			if (component == 0)
				return 240f;
			else
				return 40f;
		}

		public override nfloat GetRowHeight(UIPickerView picker, nint component)
		{
			return 40f;
		}
    }
}