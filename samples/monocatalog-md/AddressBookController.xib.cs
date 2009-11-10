
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.AddressBookUI;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonoCatalog
{
	public partial class AddressBookController : UIViewController
	{
		public AddressBookController () : base ("AddressBookController", null)
		{
		}
	
		public override void ViewDidLoad ()
		{
		}
	
		public override void ViewWillAppear (bool animated)
		{
		}
	
		public override void ViewWillDisappear (bool animated)
		{
		}
	
	  partial void showPicker (MonoTouch.UIKit.UIButton sender)
		{
			Console.Error.WriteLine ("# Select Contacts pushed!");
			using (var p = new ABPeoplePickerNavigationController ()) {
				p.SelectPerson += (o, e) => {
					Console.Error.WriteLine ("# select Person: {0}", e.Person);
					toString.Text   = e.Person.ToString ();
					firstName.Text  = e.Person.FirstName;
					lastName.Text   = e.Person.LastName;
					property.Text   = "";
					identifier.Text = "";
					e.Continue = selectProperty.On;
					if (!e.Continue)
						DismissModalViewControllerAnimated (true);
				};
				p.PerformAction += (o, e) => {
					Console.Error.WriteLine ("# perform action; person={0}", e.Person);
					toString.Text   = e.Person.ToString ();
					firstName.Text  = e.Person.FirstName;
					lastName.Text   = e.Person.LastName;
					property.Text   = e.Property.ToString ();
					identifier.Text = e.Identifier.HasValue ? e.Identifier.ToString () : "";
					e.Continue = performAction.On;
					if (!e.Continue)
						DismissModalViewControllerAnimated (true);
				};
				p.Cancelled += (o, e) => {
					Console.Error.WriteLine ("# select Person cancelled.");
					toString.Text   = "Cancelled";
					firstName.Text  = "";
					lastName.Text   = "";
					property.Text   = "";
					identifier.Text = "";
					DismissModalViewControllerAnimated (true);
				};
				PresentModalViewController (p, true);
			}
		}
	}
}
