using System;

using Foundation;
using UIKit;
using AddressBookUI;
using AddressBook;

namespace PeoplePicker
{
	[Register ("CompatiblePersonPickerViewController")]
	public class CompatiblePersonPickerViewController : UIViewController
	{
		[Outlet]
		private UILabel ResultLabel { get ; set; }

		public CompatiblePersonPickerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export("showPicker:")]
		private void ShowPicker(NSObject sender)
		{
			ABPeoplePickerNavigationController picker = new ABPeoplePickerNavigationController ();
			#pragma warning disable 618
			picker.SelectPerson += HandleSelectPerson;
			picker.SelectPerson2 += HandleSelectPerson2;
			#pragma warning restore 618
			picker.Cancelled += HandleCancelled;

			PresentViewController (picker, true, null);
		}

		// On iOS 8.0, a selected person is returned with this method.
		void HandleSelectPerson2 (object sender, ABPeoplePickerSelectPerson2EventArgs e)
		{
			ResultLabel.Text = PersonFormatter.GetPickedName (e.Person);
		}

		// On iOS 7.x or earlier, a selected person is returned with this method.
		private void HandleSelectPerson (object sender, ABPeoplePickerSelectPersonEventArgs e)
		{
			var peoplePicker = (ABPeoplePickerNavigationController)sender;
			ResultLabel.Text = PersonFormatter.GetPickedName (e.Person);

			peoplePicker.DismissViewController (true, null);
			e.Continue = false;
		}

		// On iOS 7.x or earlier, this method is required and it must dismiss the picker.
		private void HandleCancelled (object sender, EventArgs e)
		{
			var peoplePicker = (ABPeoplePickerNavigationController)sender;
			peoplePicker.DismissViewController (true, null);
		}
	}
}
