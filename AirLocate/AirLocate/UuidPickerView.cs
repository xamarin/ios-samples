using System;
using Foundation;
using UIKit;

namespace AirLocate
{
	public class UuidPickerView : UIPickerView
	{

		public UuidPickerView (UITextField uuidTextField)
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			ShowSelectionIndicator = true;
			Model = new UuidPickerViewModel (uuidTextField);
		}

		class UuidPickerViewModel : UIPickerViewModel
		{

			UITextField field;

			public UuidPickerViewModel (UITextField uuidTextField)
			{
				field = uuidTextField;
			}

			public override void Selected (UIPickerView picker, nint row, nint component)
			{
				field.Text = Defaults.SupportedProximityUuids [(int)row].AsString ();
			}

			public override nint GetRowsInComponent (UIPickerView picker, nint component)
			{
				return Defaults.SupportedProximityUuids.Count;
			}

			public override nint GetComponentCount (UIPickerView picker)
			{
				return 1;
			}

			public override string GetTitle (UIPickerView picker, nint row, nint component)
			{
				return Defaults.SupportedProximityUuids [(int)row].AsString ();
			}
		}
	}
}
