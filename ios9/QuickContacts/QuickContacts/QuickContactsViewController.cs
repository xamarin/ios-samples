using System;
using UIKit;
using Foundation;
using CoreFoundation;

using Contacts;
using ContactsUI;

namespace QuickContacts
{
	[Register("QuickContactsViewController")]
	public class QuickContactsViewController : UITableViewController, ICNContactPickerDelegate, ICNContactViewControllerDelegate
	{
		const int UIEditUnknownContactRowHeight = 81;
		const string CellIdentifier = "Cell";

		NSMutableArray menuArray;

		public QuickContactsViewController (IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "QuickContacts";

			menuArray = new NSMutableArray (0);

			CheckContactStore ();
		}

		void CheckContactStore ()
		{
			switch (CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts)) {
				case CNAuthorizationStatus.Authorized:
					AccessGrantedForContactStore();
					break;
				case CNAuthorizationStatus.NotDetermined:
					RequestContactStoreAccess();
					break;
				case CNAuthorizationStatus.Denied:
				case CNAuthorizationStatus.Restricted:
					UIAlertController alert = UIAlertController.Create("Privacy", "Permission was not granted for Contacts", UIAlertControllerStyle.Alert);
					alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					PresentViewController(alert, true, null);
					break;
				default:
					break;
			}
		}

		void RequestContactStoreAccess ()
		{
			var contactStore = new CNContactStore();
			contactStore.RequestAccess(CNEntityType.Contacts, (granted, error) => {
				if (granted) { 
					DispatchQueue.MainQueue.DispatchAsync(() => AccessGrantedForContactStore());
				}
			});
		}

		void AccessGrantedForContactStore ()
		{
			string plistPath = NSBundle.MainBundle.PathForResource ("Menu", "plist");

			menuArray = NSMutableArray.FromFile (plistPath);
			TableView.ReloadData ();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return (nint)menuArray.Count;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			NSDictionary item = menuArray.GetItem<NSDictionary> ((nuint)indexPath.Section);

			var cell = tableView.DequeueReusableCell (CellIdentifier);
			if (cell == null) {
				if (indexPath.Section < 2) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier);
					cell.TextLabel.TextAlignment = UITextAlignment.Center;
				} else {
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, CellIdentifier);
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					cell.DetailTextLabel.Lines = 0;
					cell.DetailTextLabel.Text = (NSString)item ["description"];
				}
			}

			cell.TextLabel.Text = (NSString)item ["title"];
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			ActionType type = (ActionType)indexPath.Section;
			switch (type) {
				case ActionType.PickContact:
					ShowPeoplePickerController ();
					break;
				case ActionType.CreateNewContact:
					ShowNewPersonViewController ();
					break;
				case ActionType.DisplayContact:
					ShowPersonViewController ();
					break;
				case ActionType.EditUnknownContact:
					ShowUnknownPersonViewController ();
					break;
				default:
					ShowPeoplePickerController ();
					break;
			}
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return indexPath.Section == (int)ActionType.EditUnknownContact ? UIEditUnknownContactRowHeight : tableView.RowHeight;
		}

		void ShowPeoplePickerController ()
		{
			var picker = new CNContactPickerViewController ();
			picker.Delegate = this;

			picker.PredicateForEnablingContact = NSPredicate.FromValue (true); // Enable selection for any person
			picker.PredicateForSelectionOfContact = NSPredicate.FromValue (false); // Don't select the person. Let me browse his properties
			picker.PredicateForSelectionOfProperty = NSPredicate.FromValue (true); // Invoke call back when user tap on any property

			NSString[] propertiesArray = { CNContactKey.EmailAddresses, CNContactKey.PhoneNumbers, (CNContactKey.NonGregorianBirthday)};
			picker.DisplayedPropertyKeys = propertiesArray;

			PresentViewController (picker, true, null);
		}



		void ShowPersonViewController ()
		{
			var predicate = CNContact.GetPredicateForContacts("Appleseed");

			ICNKeyDescriptor[] toFetch = { CNContactViewController.DescriptorForRequiredKeys };

			CNContactStore store = new CNContactStore();

			NSError fetchError;
			CNContact[] contacts = store.GetUnifiedContacts(predicate, toFetch, out fetchError);

			if (contacts != null && contacts.Length > 0)
			{
				CNContact contact = contacts[0];
				var peopleViewController = CNContactViewController.FromContact(contact);
				peopleViewController.AllowsEditing = true;

				NavigationController.PushViewController(peopleViewController, true);
			}
			else {
				var alert = UIAlertController.Create("Error", "Could not find Appleseed in the Contacts application", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, null));
				PresentViewController(alert, true, null);
			}
		}

		void ShowNewPersonViewController ()
		{
			var newContact = new CNContact();
			var newContactViewController = CNContactViewController.FromNewContact(newContact);
			newContactViewController.Delegate = this;
			var navController = new UINavigationController(newContactViewController);

			PresentViewController(navController, true, null);
		}

		void ShowUnknownPersonViewController ()
		{
			using (var unknowContact = new CNMutableContact()) {
				try
				{
					var unknownContactVC = CNContactViewController.FromUnknownContact(unknowContact);
					unknownContactVC.ContactStore = new CNContactStore();
					unknownContactVC.AllowsActions = true;
					unknownContactVC.AlternateName = "John Appleseed";
					unknownContactVC.Title = "John Appleseed";
					unknownContactVC.Message = "Company, Inc";
					NavigationController.PushViewController(unknownContactVC, true);
				}
				catch (Exception)
				{
					var alert = UIAlertController.Create("Error", "Could not create unknown user.", UIAlertControllerStyle.Alert);
					alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, null));
					PresentViewController(alert, true, null);
				}
			}
		}


		//ContactsViewControllerDelegate
		[Export("contactViewController:didCompleteWithContact:")]
		public void DidComplete(CNContactViewController viewController, CNContact contact)
		{
			//NavigationController.PopViewController(true);
			DismissViewController(true, null);
		}


		//ContactsPickerDelegate
		[Export("contactPicker:didSelectContact:")]
		public void DidSelectContact(CNContactPickerViewController picker, CNContact contact)
		{
			string contactName = contact.GivenName;
			string message = $"Picked -> {contactName}";
			Console.WriteLine(message);
		}

		[Export("contactPicker:didSelectContactProperty:")]
		public void DidSelectContactProperty(CNContactPickerViewController picker, CNContactProperty contactProperty)
		{
			string message = $"Picked -> {contactProperty.Identifier}";
			Console.WriteLine(message);
		}
	}
}

