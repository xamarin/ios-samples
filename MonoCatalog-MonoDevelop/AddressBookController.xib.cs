
using System;
using System.Collections.Generic;
using System.Linq;
using AddressBookUI;
using Foundation;
using UIKit;

namespace MonoCatalog
{
	public partial class AddressBookController : UIViewController
	{

		ABPeoplePickerNavigationController p;

		public AddressBookController () : base ("AddressBookController", null)
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (p != null) {
				p.Dispose ();
				p = null;
			}
			base.Dispose (disposing);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBar.Translucent = false;
		}

		ABPeoplePickerNavigationController GetPicker ()
		{
			if (p != null)
				return p;

			p = new ABPeoplePickerNavigationController ();
			p.SelectPerson += (o, e) => {
				HandlePersonSelection(e.Person);
				e.Continue = selectProperty.On;
				if (!e.Continue)
					DismissModalViewController (true);
			};
			p.SelectPerson2 += (sender, e) => {
				HandlePersonSelection(e.Person);
			};

			p.PerformAction += (o, e) => {
				HandlePersonPropertySelection(e.Person, e.Property, e.Identifier);
				if (!e.Continue)
					DismissModalViewController (true);
			};
			p.PerformAction2 += (sender, e) => {
				HandlePersonPropertySelection(e.Person, e.Property, e.Identifier);
			};

			p.Cancelled += (o, e) => {
				Console.Error.WriteLine ("# select Person cancelled.");
				toString.Text = "Cancelled";
				firstName.Text = "";
				lastName.Text = "";
				property.Text = "";
				identifier.Text = "";
				DismissModalViewController (true);
			};
			return p;
		}

		partial void showPicker (UIKit.UIButton sender)
		{
			Console.Error.WriteLine ("# Select Contacts pushed!");
			PresentModalViewController (GetPicker (), true);
		}

		void HandlePersonSelection(AddressBook.ABPerson person)
		{
			Console.Error.WriteLine ("# select Person: {0}", person);
			toString.Text = person.ToString ();
			firstName.Text = person.FirstName;
			lastName.Text = person.LastName;
			property.Text = "";
			identifier.Text = "";
		}

		void HandlePersonPropertySelection(AddressBook.ABPerson person, AddressBook.ABPersonProperty property, int? id)
		{
			Console.Error.WriteLine ("# perform action; person={0}", person);
			toString.Text = person.ToString ();
			firstName.Text = person.FirstName;
			lastName.Text = person.LastName;
			identifier.Text = id.ToString ();
			identifier.Text = id.HasValue ? id.ToString () : "";
		}
	}
}
