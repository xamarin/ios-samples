using System;
using UIKit;
using System.Collections.Generic;
using AddressBook;
using Foundation;
using CoreFoundation;
using AddressBookUI;

namespace QuickContacts
{
	[Register("QuickContactsViewController")]
	public class QuickContactsViewController : UITableViewController, IABPeoplePickerNavigationControllerDelegate
	{
		const int UIEditUnknownContactRowHeight = 81;
		const string CellIdentifier = "Cell";

		ABAddressBook addressBook;
		NSMutableArray menuArray;

		public QuickContactsViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "QuickContacts";

			menuArray = new NSMutableArray (0);

			NSError error;
			addressBook = ABAddressBook.Create (out error);

			CheckAddressBookAccess ();
		}

		void CheckAddressBookAccess ()
		{
			switch (ABAddressBook.GetAuthorizationStatus ()) {
				case ABAuthorizationStatus.Authorized:
					AccessGrantedForAddressBook ();
					break;

				case ABAuthorizationStatus.NotDetermined:
					RequestAddressBookAccess ();
					break;

				case ABAuthorizationStatus.Denied:
				case ABAuthorizationStatus.Restricted:
					UIAlertController alert = UIAlertController.Create ("Privacy", "Permission was not granted for Contacts", UIAlertControllerStyle.Alert);
					alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
					PresentViewController (alert, true, null);
					break;

				default:
					break;
			}
		}

		void RequestAddressBookAccess ()
		{
			addressBook.RequestAccess ((bool granted, NSError error) => {
				if (!granted)
					return;
				DispatchQueue.MainQueue.DispatchAsync (() => AccessGrantedForAddressBook ());
			});
		}

		void AccessGrantedForAddressBook ()
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
			var picker = new ABPeoplePickerNavigationController ();
			// TODO: https://trello.com/c/ov8Rd2z1
			picker.PerformAction2 += HandlePerformAction2;
			picker.PredicateForEnablingPerson = NSPredicate.FromValue (true); // Enable selection for any person
			picker.PredicateForSelectionOfPerson = NSPredicate.FromValue (false); // Don't select the person. Let me browse his properties
			picker.PredicateForSelectionOfProperty = NSPredicate.FromValue (true); // Invoke call back when user tap on any property

			// TODO: use flags instead https://trello.com/c/c4cwIMdE
			picker.DisplayedProperties.Add (ABPersonProperty.Phone);
			picker.DisplayedProperties.Add (ABPersonProperty.Email);
			picker.DisplayedProperties.Add (ABPersonProperty.Birthday);

			PresentViewController (picker, true, null);
		}

		void ShowPersonViewController ()
		{
			ABPerson[] people = addressBook.GetPeopleWithName ("Appleseed");

			if (people != null && people.Length > 0) {
				ABPerson person = people [0];
				var pvc = new ABPersonViewController () {
					DisplayedPerson = person,
					AllowsEditing = true
				};
				pvc.PerformDefaultAction += HandlePerformDefaultAction;

				NavigationController.PushViewController (pvc, true);
			} else {
				var alert = UIAlertController.Create ("Error", "Could not find Appleseed in the Contacts application", UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Default, null));
				PresentViewController (alert, true, null);
			}
		}

		void ShowNewPersonViewController ()
		{
			var npvc = new ABNewPersonViewController ();
			npvc.NewPersonComplete += HandleNewPersonComplete;

			var navigation = new UINavigationController (npvc);
			PresentViewController (navigation, true, null);
		}

		void ShowUnknownPersonViewController ()
		{
			using (var contact = new ABPerson ()) {
				using (var email = new ABMutableStringMultiValue ()) {
					bool didAdd = email.Add ("John-Appleseed@mac.com", ABLabel.Other);

					if (didAdd) {
						try {
							contact.SetEmails (email);

							var upvc = new ABUnknownPersonViewController {
								DisplayedPerson = contact,
								AllowsAddingToAddressBook = true,
								AllowsActions = true,
								AlternateName = "John Appleseed",
								Title = "John Appleseed",
								Message = "Company, Inc"
							};
							upvc.PersonCreated += HandlePersonCreated;
							upvc.PerformDefaultAction += HandlePerformDefaultAction;

							NavigationController.PushViewController (upvc, true);
						} catch (Exception) {
							var alert = UIAlertController.Create ("Error", "Could not create unknown user.", UIAlertControllerStyle.Alert);
							alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Default, null));
							PresentViewController (alert, true, null);
						}
					}
				}
			}
		}

		void HandlePerformAction2 (object sender, ABPeoplePickerPerformAction2EventArgs e)
		{
			string contactName = e.Person.FirstName;
			string propValue = GetPropertyValue (e.Person, e.Property, e.Identifier);
			string message = String.Format ("Picked {0} for {1}", propValue, contactName);

			Console.WriteLine (message);
		}

		string GetPropertyValue(ABPerson person, ABPersonProperty property, int? identifier)
		{
			switch (property) {
				case ABPersonProperty.Birthday:
					return person.Birthday.ToString ();

				case ABPersonProperty.Email:
					return GetEmail (person, identifier.Value);

				case ABPersonProperty.Phone:
					return GetPhone (person, identifier.Value);

				default:
					throw new NotImplementedException ();
			}
		}

		string GetEmail(ABPerson person, int identifier)
		{
			ABMultiValue<string> mails = person.GetEmails ();
			return GetValue (mails, identifier);
		}

		string GetPhone(ABPerson person, int identifier)
		{
			ABMultiValue<string> phones = person.GetPhones ();
			return GetValue (phones, identifier);
		}

		string GetValue(ABMultiValue<string> multiProperty, int identifier)
		{
			nint i = multiProperty.GetIndexForIdentifier (identifier);
			string[] values = multiProperty.GetValues ();
			return values [(int)i];
		}

		void HandlePerformDefaultAction (object sender, ABPersonViewPerformDefaultActionEventArgs e)
		{
			e.ShouldPerformDefaultAction = false;
		}

		void HandleNewPersonComplete (object sender, ABNewPersonCompleteEventArgs e)
		{
			DismissViewController (true, null);
		}

		void HandlePersonCreated (object sender, ABUnknownPersonCreatedEventArgs e)
		{
			NavigationController.PopViewController (true);
		}
	}
}

