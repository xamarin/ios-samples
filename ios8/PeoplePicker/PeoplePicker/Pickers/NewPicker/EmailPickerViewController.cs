using System;

using Foundation;
using UIKit;
using AddressBookUI;
using AddressBook;

namespace PeoplePicker
{
	[Register ("EmailPickerViewController")]
	public class EmailPickerViewController : UIViewController
	{
		[Outlet]
		private UILabel ResultLabel { get ; set; }

		public EmailPickerViewController (IntPtr handle)
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
			picker.PerformAction2 += HandlePerformAction2;
			picker.Cancelled += HandleCancelled;

			// The people picker will only display the person's name, image and email properties in ABPersonViewController.
			picker.DisplayedProperties.Add (ABPersonProperty.Email);

			// The people picker will enable selection of persons that have at least one email address.
			picker.PredicateForEnablingPerson = NSPredicate.FromFormat ("emailAddresses.@count > 0");

			// The people picker will select a person that has exactly one email address and call HandleSelectPerson2,
			// otherwise the people picker will present an ABPersonViewController for the user to pick one of the email addresses.
			picker.PredicateForSelectionOfPerson = NSPredicate.FromFormat ("emailAddresses.@count = 1");

			PresentViewController (picker, true, null);
		}

		void HandleSelectPerson2 (object sender, ABPeoplePickerSelectPerson2EventArgs e)
		{
			ResultLabel.Text = PersonFormatter.GetPickedEmail (e.Person);
		}

		// A selected person and property is returned with this method.
		void HandlePerformAction2 (object sender, ABPeoplePickerPerformAction2EventArgs e)
		{
			ResultLabel.Text = PersonFormatter.GetPickedEmail (e.Person, e.Identifier);
		}

		void HandleCancelled (object sender, EventArgs e)
		{
			// Implement this if you want to do additional work when the picker is cancelled by the user.
		}
	}
}
