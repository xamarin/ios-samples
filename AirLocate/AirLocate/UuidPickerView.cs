using System;
using Foundation;
using UIKit;

namespace AirLocate {
	
	public class UuidPickerView : UIPickerView {

		public UuidPickerView (UITextField uuidTextField)
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			ShowSelectionIndicator = true;
			Model = new UuidPickerViewModel (uuidTextField);
		}

		class UuidPickerViewModel : UIPickerViewModel {

			UITextField field;

			public UuidPickerViewModel (UITextField uuidTextField)
			{
				field = uuidTextField;
			}

			public override void Selected (UIPickerView picker, int row, int component)
			{
				field.Text = Defaults.SupportedProximityUuids [row].AsString ();
			}

			public override int GetRowsInComponent (UIPickerView picker, int component)
			{
				return Defaults.SupportedProximityUuids.Count;
			}

			public override int GetComponentCount (UIPickerView picker)
			{
				return 1;
			}

			public override string GetTitle (UIPickerView picker, int row, int component)
			{
				return Defaults.SupportedProximityUuids [row].AsString ();
			}
		}
	}
}