
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using AddressBookUI;
using AddressBook;

namespace Example_SharedResources.Screens.iPhone.Contacts
{
	public partial class ContactPickerScreen : UIViewController
	{
		// you must declare the Address Book Controllers at the class-level, otherwise they'll get 
		// garbage collected when the method that creates them returns. When the events fire, the handlers
		// will have also been GC'd
		protected ABPeoplePickerNavigationController addressBookPicker;
		protected ABPersonViewController addressBookViewPerson;
	
		protected ABPerson selectedPerson = null;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public ContactPickerScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ContactPickerScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ContactPickerScreen () : base("ContactPickerScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Address Book Controllers";
			
			// displays the contact picker controller when the choose contact button is clicked
			btnChooseContact.TouchUpInside += (s, e) => {
				// create the picker control
				addressBookPicker = new ABPeoplePickerNavigationController();
				
				// in this case, we can call present modal view controller from the nav controller, 
				// but we could have just as well called PresentModalViewController(...)
				NavigationController.PresentModalViewController(addressBookPicker, true);
				
				// when cancel is clicked, dismiss the controller
				addressBookPicker.Cancelled += (sender, eventArgs) => { NavigationController.DismissModalViewController(true); };
				
				// when a contact is chosen, populate the page with details and dismiss the controller
				addressBookPicker.SelectPerson += (object sender, ABPeoplePickerSelectPersonEventArgs args) => {
					selectedPerson = args.Person;
					lblFirstName.Text = selectedPerson.FirstName;
					lblLastName.Text = selectedPerson.LastName;
					NavigationController.DismissModalViewController(true);
				};
			};
			
			// shows the view/edit contact controller when the button is clicked
			btnViewSelectedContact.TouchUpInside += (s, e) => {
				
				// if a contact hasn't been selected, show an alert and return out
				if(selectedPerson == null)
				{
					new UIAlertView ("Alert", "You must select a contact first.", null, "OK", null).Show (); 
					return;
				}
				
				// instantiate a new controller
				addressBookViewPerson = new ABPersonViewController ();
				
				// set the contact to display
				addressBookViewPerson.DisplayedPerson = selectedPerson;
				
				// allow editing
				addressBookViewPerson.AllowsEditing = true;
				
				// push the controller onto the nav stack. the view/edit controller requires a nav
				// controller and handles it's own dismissal
				NavigationController.PushViewController (addressBookViewPerson, true);
			};
		}
	}
}

