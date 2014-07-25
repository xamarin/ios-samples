
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using AddressBook;
using AddressBookUI;

namespace Example_SharedResources.Screens.iPhone.Contacts
{
	public partial class NewAndUnknownContactScreen : UIViewController
	{
		// you must declare the Address Book Controllers at the class-level, otherwise they'll get 
		// garbage collected when the method that creates them returns. When the events fire, the handlers
		// will have also been GC'd
		protected ABNewPersonViewController addressBookNewPerson;
		protected ABUnknownPersonViewController addressBookUnknownPerson;

		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public NewAndUnknownContactScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public NewAndUnknownContactScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public NewAndUnknownContactScreen () : base("NewAndUnknownContactScreen", null)
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
			
			Title = "New and Unknown Contacts";
			
			// shows the create new contact screen when the button is clicked
			btnCreateNewContact.TouchUpInside += (s, e) => {
				// instantiate a new ABNewPersonViewController
				addressBookNewPerson = new ABNewPersonViewController ();
				
				// create a person from the fields on the screen so we can prepopulate the 
				// controller with data
				ABPerson person = new ABPerson ();
				person.FirstName = txtFirstName.Text;
				person.LastName = txtLastName.Text;
				
				// prepopulate the controller with the person
				addressBookNewPerson.DisplayedPerson = person;
				
				// push the controller onto the nav stack
				NavigationController.PushViewController (addressBookNewPerson, true);
				
				// wire up the new person complete handler to pop the controller off the stack
				addressBookNewPerson.NewPersonComplete += (object sender, ABNewPersonCompleteEventArgs args) => {
					
					// if the "done" button was clicked, rather than cancel
					if(args.Completed) {
						// show an alert view with the new contact ID
						new UIAlertView ("Alert", "New contact created, ID: " + args.Person.Id.ToString(), null, "OK", null).Show();
					}
					
					// pop the controller off the stack
					NavigationController.PopViewController (true);
				};
			};
			
			// 
			btnPromptForUnknown.TouchUpInside += (s, e) => {
				// instantiate a new unknown person controller
				addressBookUnknownPerson = new ABUnknownPersonViewController ();
				
				// create a person from the fields on the screen so we can prepopulate the 
				// controller with data
				ABPerson person = new ABPerson ();
				person.FirstName = txtFirstName.Text;
				person.LastName = txtLastName.Text;
				
				// prepopulate the controller with the person
				addressBookUnknownPerson.DisplayedPerson = person;
				
				// allow adding to address book
				addressBookUnknownPerson.AllowsAddingToAddressBook = true;
				
				// allow them to share the contact, make calls, click on urls, etc in the controller
				addressBookUnknownPerson.AllowsActions = true;
				
				// push the controller onto the nav stack
				NavigationController.PushViewController (addressBookUnknownPerson, true);
				
				// handle the person created event
				addressBookUnknownPerson.PersonCreated += (object sender, ABUnknownPersonCreatedEventArgs args) => {
					Console.WriteLine ("PersonCreated event raised");
					
					// this dialog can be cancelled out of as well, but there is no Completed property, so we
					// just have to do a null check
					if(args.Person != null) {
						// show an alert view with the new contact ID
						new UIAlertView ("Alert", "New contact created, ID: " + args.Person.Id.ToString (), null, "OK", null).Show ();
					}
				};
				
				// you can also handle the perform default action event to determine whether or not the action should be allowed
				// to be perfomed.
				//addressBookUnknownPerson.PerformDefaultAction += (object sender, ABPersonViewPerformDefaultActionEventArgs args) => {
				//	if(args.Property == ABPersonProperty.Url)
				//	{
				//		args.ShouldPerformDefaultAction = false;						
				//	}
				//};
			};
		}
	}
}

