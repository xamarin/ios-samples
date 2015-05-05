
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using AddressBook;
using AddressBookUI;
using CoreGraphics;

namespace Example_SharedResources.Screens.iPhone.Contacts
{
	public partial class AddressBookScreen : UIViewController
	{
		/// <summary>
		/// Our address book picker control
		/// </summary>
		protected ABPeoplePickerNavigationController addressBookPicker;
		/// <summary>
		/// The table data source that will bind the phone numbers
		/// </summary>
		protected PhoneNumberTableSource tableDataSource;
		/// <summary>
		/// The ID of our person
		/// </summary>
		protected int contactID;
		/// <summary>
		/// Used to resize the scroll view to allow for keyboard
		/// </summary>
		CGRect contentViewSize = CGRect.Empty;

		ABAddressBook addressBook;

		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need
		// to be able to be created from a xib rather than from managed code

		public AddressBookScreen (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public AddressBookScreen (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		public AddressBookScreen () : base ("AddressBookScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			contentViewSize = View.Frame;
			scrlMain.ContentSize = contentViewSize.Size;
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = "Address Book";

			// add a button to the nav bar that will select a contact to edit
			UIButton btnSelectContact = UIButton.FromType (UIButtonType.RoundedRect);
			btnSelectContact.SetTitle ("Select Contact", UIControlState.Normal);
			NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (UIBarButtonSystemItem.Action, SelectContact), false);

			// disable first two fields until the user select a contact
			EnableTextFields (false);

			// wire up keyboard hiding
			txtPhoneLabel.ShouldReturn += (t) => {
				t.ResignFirstResponder ();
				return true;
			};
			txtPhoneNumber.ShouldReturn += (t) => {
				t.ResignFirstResponder ();
				return true;
			};
			txtFirstName.ShouldReturn += (t) => {
				t.ResignFirstResponder ();
				return true;
			};
			txtLastName.ShouldReturn += (t) => {
				t.ResignFirstResponder ();
				return true;
			};

			// wire up event handlers
			btnSaveChanges.TouchUpInside += BtnSaveChangesTouchUpInside;
			btnAddPhoneNumber.TouchUpInside += BtnAddPhoneNumberTouchUpInside;

			#region -= Sample code showing how to loop through all the records =-
			//==== This block of code writes out each person contact in the address book and
			// each phone number for that person to the console, just to illustrate the code
			// neccessary to access each item

			// instantiate a reference to the address book
			NSError err;
			addressBook = ABAddressBook.Create (out err);
			if (err != null)
				return;

			// if you want to subscribe to changes
			addressBook.ExternalChange += (object sender, ExternalChangeEventArgs e) => {
				// code to deal with changes
			};

			addressBook.RequestAccess (delegate (bool granted, NSError error) {

				if (!granted || error != null)
					return;

				// for each record
				foreach (ABRecord item in addressBook) {

					Console.WriteLine (item.Type.ToString () + " " + item.Id);
					// there are two possible record types, person and group
					if (item.Type == ABRecordType.Person) {
						// since we've already tested it to be a person, just create a shortcut to that
						// type
						ABPerson person = item as ABPerson;
						Console.WriteLine (person.FirstName + " " + person.LastName);

						// get the phone numbers
						ABMultiValue<string> phones = person.GetPhones ();
						foreach (ABMultiValueEntry<string> val in phones) {
							Console.Write (val.Label + ": " + val.Value);
						}
					}
				}

				// save changes (if you were to have made any)
				//addressBook.Save();
				// or cancel them
				//addressBook.Revert();
			});
			//====
			#endregion

			#region -= keyboard stuff =-

			// wire up our keyboard events
			NSNotificationCenter.DefaultCenter.AddObserver (
				UIKeyboard.WillShowNotification, delegate (NSNotification n) {
				KeyboardOpenedOrClosed (n, "Open");
			});
			NSNotificationCenter.DefaultCenter.AddObserver (
				UIKeyboard.WillHideNotification, delegate (NSNotification n) {
				KeyboardOpenedOrClosed (n, "Close");
			});

			#endregion
		}

		protected void BtnAddPhoneNumberTouchUpInside (object sender, EventArgs e)
		{
			// get a reference to the contact
			using (ABAddressBook addressBook = new ABAddressBook ()) {
				ABPerson contact = addressBook.GetPerson (contactID);
				if (contact != null) {
					// get the phones and copy them to a mutable set of multivalues (so we can edit)
					ABMutableMultiValue<string> phones = contact.GetPhones ().ToMutableMultiValue ();

					// add the phone number to the phones via the multivalue.Add method
					if (txtPhoneNumber.Text != null || txtPhoneLabel.Text != null) {
						phones.Add (new NSString (txtPhoneNumber.Text), new NSString (txtPhoneLabel.Text));

						// attach the phones back to the contact
						contact.SetPhones (phones);

						// save the address book changes
						addressBook.Save ();

						// show an alert, letting the user know the number addition was successful
						new UIAlertView ("Alert", "Phone Number Added", null, "OK", null).Show ();

						// update the page
						PopulatePage (contact);

						// we have to call reload to refresh the table because the action didn't originate
						// from the table.
						tblPhoneNumbers.ReloadData ();
					} else {
						// show an alert, letting the user know he has to fill the phone label and phone number
						new UIAlertView ("Alert", "You have to fill a label and a phone number", null, "OK", null).Show ();
					}
				} else {
					new UIAlertView ("Alert", "Please select a contact using the top right button", null, "OK", null).Show ();
				}
			}
		}

		// Called when a phone number is swiped for deletion. Illustrates how to delete a multivalue property
		protected void DeletePhoneNumber (int phoneNumberID)
		{
			using (ABAddressBook addressBook = new ABAddressBook ()) {
				ABPerson contact = addressBook.GetPerson (contactID);

				// get the phones and copy them to a mutable set of multivalues (so we can edit)
				ABMutableMultiValue<string> phones = contact.GetPhones ().ToMutableMultiValue ();

				// loop backwards and delete the phone number
				// HACK: Cast nint to int
				for (int i = (int)phones.Count - 1; i >= 0; i--) {
					if (phones [i].Identifier == phoneNumberID)
						phones.RemoveAt (i);
				}

				// attach the phones back to the contact
				contact.SetPhones (phones);

				// save the changes
				addressBook.Save ();

				// show an alert, letting the user know the number deletion was successful
				new UIAlertView ("Alert", "Phone Number Deleted", null, "OK", null).Show ();

				// repopulate the page
				PopulatePage (contact);
			}
		}

		protected void BtnSaveChangesTouchUpInside (object sender, EventArgs e)
		{
			using (ABAddressBook addressBook = new ABAddressBook ()) {
				ABPerson contact = addressBook.GetPerson (contactID);

				if (contact != null) {
					// save contact name information
					contact.FirstName = txtFirstName.Text;
					contact.LastName = txtLastName.Text;

					// get the phones and copy them to a mutable set of multivalues (so we can edit)
					ABMutableMultiValue<string> phones = contact.GetPhones ().ToMutableMultiValue ();

					// remove all phones data
					// HACK: Cast nint to int
					for (int i = (int)phones.Count - 1; i >= 0; i--) {
						phones.RemoveAt (i);
					}

					// add the phone number to the phones from the table data source
					for (int i = 0; i < PhoneNumberTableSource.labels.Count; i++) {
						phones.Add (new NSString (PhoneNumberTableSource.numbers [i]), new NSString (PhoneNumberTableSource.labels [i]));
					}

					// attach the phones back to the contact
					contact.SetPhones (phones);

					// save the address book changes
					addressBook.Save ();

					// show an alert, letting the user know information saved successfully
					new UIAlertView ("Alert", "Contact Information Saved!", null, "OK", null).Show ();

					// update the page
					PopulatePage (contact);

					// we have to call reload to refresh the table because the action didn't originate
					// from the table.
					tblPhoneNumbers.ReloadData ();
				} else {
					new UIAlertView ("Alert", "Please select a contact using the top right button", null, "OK", null).Show ();
				}
			}
		}

		protected void PopulatePage (ABPerson contact)
		{
			// save the ID of our person
			contactID = contact.Id;

			// set the data on the page
			txtFirstName.Text = contact.FirstName;
			txtLastName.Text = contact.LastName;
			tableDataSource = new AddressBookScreen.PhoneNumberTableSource (contact.GetPhones ());
			tblPhoneNumbers.Source = tableDataSource;

			// wire up our delete clicked handler
			tableDataSource.DeleteClicked +=
				(object sender, PhoneNumberTableSource.PhoneNumberClickedEventArgs e) => {
				DeletePhoneNumber (e.PhoneNumberID);
			};
		}

		// Opens up a contact picker and then populates the screen, based on the contact chosen
		protected void SelectContact (object s, EventArgs e)
		{
			// create the picker control
			addressBookPicker = new ABPeoplePickerNavigationController ();

			NavigationController.PresentModalViewController (addressBookPicker, true);

			// wire up the cancelled event to dismiss the picker
			addressBookPicker.Cancelled += (sender, eventArgs) => {
				// HACK: NavigationController.DismissModalViewControllerAnimated to NavigationController.DismissModalViewController
				NavigationController.DismissModalViewController (true);
			};

			// when a contact is chosen, populate the page and then dismiss the picker
			addressBookPicker.SelectPerson2 += (sender, args) => {
				PopulatePage (args.Person);
				EnableTextFields (true);
				// HACK: NavigationController.DismissModalViewControllerAnimated to NavigationController.DismissModalViewController
				NavigationController.DismissModalViewController(true);
			};
		}

		private void EnableTextFields (bool enable)
		{
			if (enable) {
				txtFirstName.BackgroundColor = UIColor.Clear;
				txtLastName.BackgroundColor = UIColor.Clear;
				txtPhoneLabel.BackgroundColor = UIColor.Clear;
				txtPhoneNumber.BackgroundColor = UIColor.Clear;
			} else {
				txtFirstName.BackgroundColor = UIColor.LightGray;
				txtLastName.BackgroundColor = UIColor.LightGray;
				txtPhoneLabel.BackgroundColor = UIColor.LightGray;
				txtPhoneNumber.BackgroundColor = UIColor.LightGray;
			}
			txtFirstName.Enabled = enable;
			txtLastName.Enabled = enable;
			txtPhoneLabel.Enabled = enable;
			txtPhoneNumber.Enabled = enable;
		}

		#region -= table binding stuff (not important to understanding the address API) =-

		/// <summary>
		/// A simple table view source to bind our phone numbers to the table
		/// </summary>
		protected class PhoneNumberTableSource : UITableViewSource
		{
			public event EventHandler<PhoneNumberClickedEventArgs> DeleteClicked;

			protected ABMultiValue<string> phoneNumbers { get; set; }

			public static List<string> labels;
			public static List<string> numbers;

			public PhoneNumberTableSource (ABMultiValue<string> phoneNumbers)
			{
				this.phoneNumbers = phoneNumbers;
				if (labels == null)
					labels = new List<string> ();
				else
					labels.Clear ();
				if (numbers == null)
					numbers = new List<string> ();
				else
					numbers.Clear ();
				for (int i = 0; i < phoneNumbers.Count; i++) {
					labels.Add (phoneNumbers [i].Label.Description);
					numbers.Add (phoneNumbers [i].Value);
				}
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return phoneNumbers.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				EditablePhoneTableCell cell = tableView.DequeueReusableCell ("PhoneCell") as EditablePhoneTableCell;
				if (cell != null)
					cell.dataIndex = indexPath.Row;
				if (cell == null) {
					cell = new EditablePhoneTableCell ("PhoneCell");
					cell.dataIndex = indexPath.Row;
					cell.txtLabel.EditingDidEnd += (sender, e) => {
						labels [cell.dataIndex] = cell.PhoneLabel;

					};
					cell.txtPhoneNumber.EditingDidEnd += (sender, e) => {
						numbers [cell.dataIndex] = cell.PhoneNumber;
					};
				}
//				cell.PhoneLabel = phoneNumbers[indexPath.Row].Label.ToString ().Replace ("_$!<", "").Replace (">!$_", "");
//				cell.PhoneNumber = phoneNumbers[indexPath.Row].Value.ToString ();
				cell.PhoneLabel = labels [indexPath.Row].Replace ("_$!<", "").Replace (">!$_", "");
				cell.PhoneNumber = numbers [indexPath.Row];
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;

				return cell;
			}

			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				return true;
			}

			public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return UITableViewCellEditingStyle.Delete;
			}

			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) {
					if (DeleteClicked != null)
						DeleteClicked (this, new PhoneNumberClickedEventArgs (phoneNumbers [indexPath.Row].Identifier));
				}
			}

			// We use this so we can pass the id of the phone number that was clicked along with the event
			public class PhoneNumberClickedEventArgs : EventArgs
			{
				public int PhoneNumberID { get; set; }

				public PhoneNumberClickedEventArgs (int phoneNumberID) : base ()
				{
					PhoneNumberID = phoneNumberID;
				}
			}

		}

		/// <summary>
		/// A simple, two text box cell, that will hold our phone label, and our phone number.
		/// </summary>
		protected class EditablePhoneTableCell : UITableViewCell
		{
			// label and phone number text boxes
			public UITextField txtLabel = new UITextField (new CGRect (10, 5, 110, 33));
			public UITextField txtPhoneNumber = new UITextField (new CGRect (130, 5, 140, 33));

			// properties
			public string PhoneLabel { get { return txtLabel.Text; } set { txtLabel.Text = value; } }

			public string PhoneNumber { get { return txtPhoneNumber.Text; } set { txtPhoneNumber.Text = value; } }

			public int dataIndex;

			public EditablePhoneTableCell (string reuseIdentifier) : base (UITableViewCellStyle.Default, reuseIdentifier)
			{
				AddSubview (txtLabel);
				AddSubview (txtPhoneNumber);

				txtLabel.ReturnKeyType = UIReturnKeyType.Done;
				txtLabel.BorderStyle = UITextBorderStyle.Line;
				txtLabel.ShouldReturn += (t) => {
					t.ResignFirstResponder ();
					return true;
				};
				txtPhoneNumber.ReturnKeyType = UIReturnKeyType.Done;
				txtPhoneNumber.BorderStyle = UITextBorderStyle.Line;
				txtPhoneNumber.ShouldReturn += (t) => {
					t.ResignFirstResponder ();
					return true;
				};

			}

		}

		#endregion

		#region -= keyboard/screen resizing =-

		/// <summary>
		/// resizes the view when the keyboard comes up or goes away, allows our scroll view to work
		/// </summary>
		protected void KeyboardOpenedOrClosed (NSNotification n, string openOrClose)
		{
			// if it's opening
			if (openOrClose == "Open") {
				Console.WriteLine ("Keyboard opening");
				// declare vars
				CGRect kbdFrame = UIKeyboard.BoundsFromNotification (n);
				double animationDuration = UIKeyboard.AnimationDurationFromNotification (n);
				CGRect newFrame = contentViewSize;
				// resize our frame depending on whether the keyboard pops in or out
				newFrame.Height -= kbdFrame.Height;
				// apply the size change
				UIView.BeginAnimations ("ResizeForKeyboard");
				UIView.SetAnimationDuration (animationDuration);
				scrlMain.Frame = newFrame;
				UIView.CommitAnimations ();
			} else { // if it's closing, resize
				// declare vars
				double animationDuration = UIKeyboard.AnimationDurationFromNotification (n);
				// apply the size change
				UIView.BeginAnimations ("ResizeForKeyboard");
				UIView.SetAnimationDuration (animationDuration);
				scrlMain.Frame = contentViewSize;
				UIView.CommitAnimations ();
			}
		}

		#endregion
	}
}

