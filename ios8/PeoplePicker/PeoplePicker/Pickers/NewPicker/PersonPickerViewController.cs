using System;

using Foundation;
using UIKit;
using AddressBookUI;
using AddressBook;

namespace PeoplePicker
{
	[Register ("PersonPickerViewController")]
	public class PersonPickerViewController : UIViewController
	{
		[Outlet]
		private UILabel ResultLabel { get ; set; }

		public PersonPickerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export("showPicker:")]
		private void ShowPicker(NSObject sender)
		{
			// This example is to be run on iOS 8.0.
			if (!DeviceHelper.IsRunningOn8())
				return;

			ABPeoplePickerNavigationController picker = new ABPeoplePickerNavigationController ();
			picker.SelectPerson2 += HandleSelectPerson2;
			picker.Cancelled += HandleCancelled;

			PresentViewController (picker, true, null);
		}

		// A selected person is returned with this method.
		void HandleSelectPerson2 (object sender, ABPeoplePickerSelectPerson2EventArgs e)
		{
			ResultLabel.Text = PersonFormatter.GetPickedName (e.Person);
		}

		void HandleCancelled (object sender, EventArgs e)
		{
			// Implement this if you want to do additional work when the picker is cancelled by the user.
		}
	}
}
