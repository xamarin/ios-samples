using System;

using Foundation;
using UIKit;
using AddressBookUI;
using AddressBook;
using ObjCRuntime;

namespace PeoplePicker
{
	[Register ("CompatibleEmailPickerViewController")]
	public class CompatibleEmailPickerViewController : UIViewController
	{
		[Outlet]
		UILabel ResultLabel { get ; set; }

		public CompatibleEmailPickerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export("showPicker:")]
		void ShowPicker(NSObject sender)
		{
			ABPeoplePickerNavigationController picker = new ABPeoplePickerNavigationController ();
			#pragma warning disable 618
			picker.SelectPerson += HandleSelectPerson;
			picker.SelectPerson2 += HandleSelectPerson2;
			picker.PerformAction += HandlePerformAction;
			picker.PerformAction2 += HandlePerformAction2;
			#pragma warning restore 618
			picker.Cancelled += HandleCancelled;

			// The people picker will only display the person's name, image and email properties in ABPersonViewController.
			picker.DisplayedProperties.Add (ABPersonProperty.Email);

			// The people picker will enable selection of persons that have at least one email address.
			if(picker.RespondsToSelector(new Selector("setPredicateForEnablingPerson:")))
				picker.PredicateForEnablingPerson = NSPredicate.FromFormat ("emailAddresses.@count > 0");

			// The people picker will select a person that has exactly one email address and call peoplePickerNavigationController:didSelectPerson:,
			// otherwise the people picker will present an ABPersonViewController for the user to pick one of the email addresses.
			if(picker.RespondsToSelector(new Selector("setPredicateForSelectionOfPerson:")))
				picker.PredicateForSelectionOfPerson = NSPredicate.FromFormat ("emailAddresses.@count = 1");

			PresentViewController (picker, true, null);
		}

		// iOS7 and below
		void HandleSelectPerson (object sender, ABPeoplePickerSelectPersonEventArgs e)
		{
			var peoplePicker = (ABPeoplePickerNavigationController)sender;

			e.Continue = false;
			using (ABMultiValue<string> emails = e.Person.GetEmails ())
				e.Continue = emails.Count == 1;

			if (!e.Continue) {
				ResultLabel.Text = PersonFormatter.GetPickedEmail (e.Person);
				peoplePicker.DismissViewController (true, null);
			}
		}

		// iOS8+
		void HandleSelectPerson2 (object sender, ABPeoplePickerSelectPerson2EventArgs e)
		{
			ResultLabel.Text = PersonFormatter.GetPickedEmail (e.Person);
		}

		// iOS7 and below
		void HandlePerformAction (object sender, ABPeoplePickerPerformActionEventArgs e)
		{
			var peoplePicker = (ABPeoplePickerNavigationController)sender;

			ResultLabel.Text = PersonFormatter.GetPickedEmail (e.Person, e.Identifier);
			peoplePicker.DismissViewController (true, null);

			e.Continue = false;
		}

		// iOS8+
		void HandlePerformAction2 (object sender, ABPeoplePickerPerformAction2EventArgs e)
		{
			ResultLabel.Text = PersonFormatter.GetPickedEmail (e.Person, e.Identifier);
		}

		void HandleCancelled (object sender, EventArgs e)
		{
			var peoplePicker = (ABPeoplePickerNavigationController)sender;
			peoplePicker.DismissViewController (true, null);
		}
	}
}
