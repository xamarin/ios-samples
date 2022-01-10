using Contacts.Helpers;
using ContactsUI;
using Foundation;
using System;
using UIKit;

namespace Contacts
{
    public partial class PredicatePropertiesViewController : UITableViewController, ICNContactPickerDelegate
    {
        private UITableViewCell previousSelectedCell;

        private string message;

        public PredicatePropertiesViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var index = NSIndexPath.FromRowSection(0, 0);
            base.TableView.SelectRow(index, false, UITableViewScrollPosition.Top);
            this.previousSelectedCell = base.TableView.CellAt(index);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!string.IsNullOrEmpty(this.message))
            {
                base.NavigationController.ShowAlert(this.message);
                this.message = null;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                var selectedCell = tableView.CellAt(indexPath);
                if (selectedCell != this.previousSelectedCell)
                {
                    selectedCell.Accessory = UITableViewCellAccessory.Checkmark;

                    if (this.previousSelectedCell != null)
                    {
                        this.previousSelectedCell.Accessory = UITableViewCellAccessory.None;
                    }

                    this.previousSelectedCell = selectedCell;
                }
            }
            else
            {
                // show picker
                this.ShowPicker();
            }

            tableView.DeselectRow(indexPath, true);
        }

        private void ShowPicker()
        {
            var index = base.TableView.IndexPathForCell(this.previousSelectedCell);
            switch (index.Row)
            {
                case 0:
                    // Only show the given and family names, email addresses, phone numbers, and postal addresses of a contact.
                    this.HandleAllContacts();
                    break;
                case 1:
                    // When users select a contact's email address, it dismisses the view controller and returns the email address using 
                    // CNContactPickerDelegate's contact​Picker(_:​did​Select:​). When users select other properties, it implements their default action.
                    this.HandleContactsWithEmailAddresses();
                    break;
                case 2:
                    // When users select a contact's postal address, it dismisses the view controller and returns the birthday using
                    // CNContactPickerDelegate's contact​Picker(_:​did​Select:​). When users select other properties, it implements their default action.
                    this.HandleContactsWithPostalAddresses();
                    break;
                case 3:
                    // When users select a contact's phone number, it dismisses the view controller and returns the phone number using
                    // CNContactPickerDelegate's contact​Picker(_:​did​Select:​). When users select other properties, it implements their default action.
                    this.HandleContactsWithPhoneNumbers();
                    break;
            }
        }

        private void HandleContactsWithPhoneNumbers()
        {
            var picker = new CNContactPickerViewController { Delegate = this };

            // Only show contacts with email addresses.
            picker.PredicateForSelectionOfProperty = NSPredicate.FromFormat("key == 'phoneNumbers'");
            base.NavigationController.PresentViewController(picker, true, null);
        }

        private void HandleContactsWithPostalAddresses()
        {
            var picker = new CNContactPickerViewController { Delegate = this };

            // Only show contacts with email addresses.
            picker.PredicateForSelectionOfProperty = NSPredicate.FromFormat("key == 'postalAddresses'");
            base.NavigationController.PresentViewController(picker, true, null);
        }

        private void HandleContactsWithEmailAddresses()
        {
            var picker = new CNContactPickerViewController { Delegate = this };

            // Only show contacts with email addresses.
            picker.PredicateForSelectionOfProperty = NSPredicate.FromFormat("(key == 'emailAddresses')");
            base.NavigationController.PresentViewController(picker, true, null);
        }

        private void HandleAllContacts()
        {
            var picker = new CNContactPickerViewController { Delegate = this };
            /*
                Only show the given and family names, email addresses, phone numbers,
                and postal addresses of a contact.
            */
            picker.DisplayedPropertyKeys = new NSString[]
                                           {
                                                CNContactKey.GivenName,
                                                CNContactKey.FamilyName,
                                                CNContactKey.EmailAddresses,
                                                CNContactKey.PhoneNumbers,
                                                CNContactKey.PostalAddresses
                                            };
            base.NavigationController.PresentViewController(picker, true, null);
        }

        #region ICNContactPickerDelegate

        [Export("contactPicker:didSelectContactProperty:")]
        public void DidSelectContactProperty(CNContactPickerViewController picker, CNContactProperty contactProperty)
        {
            if (contactProperty != null)
            {
                var value = contactProperty.GetNameMatchingValue();
                var key = contactProperty.GetNameMatchingKey();
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(key))
                {
                    this.message = $"{contactProperty.Contact.GetFormattedName()}'s {key} ({value}) was selected.";
                }
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.previousSelectedCell != null)
            {
                this.previousSelectedCell.Dispose();
                this.previousSelectedCell = null;
            }
        }
    }
}