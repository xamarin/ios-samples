using Contacts.Helpers;
using ContactsUI;
using Foundation;
using System;
using UIKit;

namespace Contacts
{
    public partial class PredicateContactViewController : UITableViewController, ICNContactPickerDelegate
    {
        private UITableViewCell previousSelectedCell;

        private string message;

        public PredicatePickerMode Mode { get; internal set; }

        public PredicateContactViewController(IntPtr handle) : base(handle) { }

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
                    // Enable all contacts in the contacts picker view.
                    this.HandleAllContacts();
                    break;
                case 1:
                    // Only enable contacts with email addresses in the contacts picker view.
                    this.HandleContactsWithEmailAddresses();
                    break;
                case 2:
                    // Only enable contacts with postal addresses in the contacts picker view.
                    this.HandleContactsWithPostalAddresses();
                    break;
                case 3:
                    // Only enable contacts with 2 or more phone numbers in the contacts picker view.
                    this.HandleContactsWithPhoneNumbers();
                    break;
                case 4:
                    // Only enable contacts with a profile picture in the contacts picker view.
                    this.HandleContactsWithProfilePicture();
                    break;
            }
        }

        private void HandleContactsWithProfilePicture()
        {
            var picker = this.CreatePicker();

            // Only show contacts with email addresses.
            picker.PredicateForEnablingContact = NSPredicate.FromFormat("imageDataAvailable == true");
            base.NavigationController.PresentViewController(picker, true, null);
        }

        private void HandleContactsWithPhoneNumbers()
        {
            var picker = this.CreatePicker();

            // Only show contacts with email addresses.
            picker.PredicateForEnablingContact = NSPredicate.FromFormat("phoneNumbers.@count > 1");
            base.NavigationController.PresentViewController(picker, true, null);
        }

        private void HandleContactsWithPostalAddresses()
        {
            var picker = this.CreatePicker();

            // Only show contacts with email addresses.
            picker.PredicateForEnablingContact = NSPredicate.FromFormat("postalAddresses.@count > 0");
            base.NavigationController.PresentViewController(picker, true, null);
        }

        private void HandleContactsWithEmailAddresses()
        {
            var picker = this.CreatePicker();

            // Only show contacts with email addresses.
            picker.PredicateForEnablingContact = NSPredicate.FromFormat("emailAddresses.@count > 0");
            base.NavigationController.PresentViewController(picker, true, null);
        }

        private void HandleAllContacts()
        {
            var picker = this.CreatePicker();
            base.NavigationController.PresentViewController(picker, true, null);
        }

        private CNContactPickerViewController CreatePicker()
        {
            var controller = new CNContactPickerViewController();
            if (this.Mode == PredicatePickerMode.SelectContacts)
            {
                controller.Delegate = this;
            }

            return controller;
        }

        #region ICNContactPickerDelegate

        [Export("contactPicker:didSelectContact:")]
        public void DidSelectContact(CNContactPickerViewController picker, CNContact contact)
        {
            var name = contact?.GetFormattedName();
            if (!string.IsNullOrEmpty(name))
            {
                this.message = $"{name} was selected.";
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