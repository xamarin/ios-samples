using Contacts.PickerDelegates;
using Contacts.Helpers;
using ContactsUI;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace Contacts
{
    public partial class ContactPickerController : UIViewController, IUITableViewDelegate, IUITableViewDataSource
    {
        private readonly List<Section> sections = new List<Section>();

        private ICNContactPickerDelegate contactDelegate;

        public ContactPickerController(IntPtr handle) : base(handle) { }

        public PickerMode Mode { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.tableView.Hidden = true;
            this.headerLabel.Hidden = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            switch (this.Mode)
            {
                case PickerMode.SingleContact:
                    this.headerLabel.Text = "Selected Contact:";
                    this.descriptionLabel.Text = "This page allows you to select a single contact from the picker. Tap \"Show Picker\" to launch the view controller.";
                    break;

                case PickerMode.MultupleContacts:
                    this.headerLabel.Text = "Selected Contacts:";
                    this.descriptionLabel.Text = "This page allows you to select multiple contacts from the picker. Tap \"Show Picker\" to launch the view controller.";
                    break;

                case PickerMode.SingleProperty:
                    this.headerLabel.Text = "Selected Property:";
                    this.descriptionLabel.Text = "This page allows you to select a single property from the picker. Tap \"Show Picker\" to launch the view controller.";
                    break;

                case PickerMode.MultipleProperties:
                    this.headerLabel.Text = "Selected Properties:";
                    this.descriptionLabel.Text = "This page allows you to select one or more e-mail properties. Tap \"Show Picker\" to launch the view controller.";
                    break;
            }
        }

        partial void ShowPicker(NSObject sender)
        {
            this.UpdateInterface(true);
            switch (this.Mode)
            {
                case PickerMode.SingleContact:
                    this.contactDelegate = new SingleContactPickerDelegate(this.Update);
                    var picker = new CNContactPickerViewController { Delegate = this.contactDelegate };
                    base.PresentViewController(picker, true, null);
                    break;

                case PickerMode.MultupleContacts:
                    this.contactDelegate = new MultipleContactPickerDelegate(this.Update);
                    var contactsPicker = new CNContactPickerViewController { Delegate = this.contactDelegate };
                    base.PresentViewController(contactsPicker, true, null);
                    break;

                case PickerMode.SingleProperty:
                    this.contactDelegate = new SinglePropertyPickerDelegate(this.Update);
                    var propertyPicker = new CNContactPickerViewController { Delegate = this.contactDelegate };
                    propertyPicker.DisplayedPropertyKeys = new NSString[] { CNContactKey.GivenName, 
                                                                            CNContactKey.FamilyName, 
                                                                            CNContactKey.EmailAddresses,
                                                                            CNContactKey.PhoneNumbers, 
                                                                            CNContactKey.PostalAddresses };
                    base.PresentViewController(propertyPicker, true, null);
                    break;

                case PickerMode.MultipleProperties:
                    this.contactDelegate = new MultiplePropertyPickerDelegate(this.Update);
                    var propertiesPicker = new CNContactPickerViewController { Delegate = this.contactDelegate };
                    propertiesPicker.PredicateForSelectionOfProperty = NSPredicate.FromFormat("key == 'emailAddresses'");
                    base.PresentViewController(propertiesPicker, true, null);
                    break;
            }
        }

        private void UpdateInterface(bool hide)
        {
            this.tableView.Hidden = hide;
            this.headerLabel.Hidden = hide;

            if (!hide)
            {
                this.tableView.ReloadData();
            }
        }

        #region IUITableViewDataSource

        [Export("numberOfSectionsInTableView:")]
        public nint NumberOfSections(UITableView tableView)
        {
            return this.sections.Count;
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return this.sections[(int)section].Items.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = this.sections[indexPath.Section].Items[indexPath.Row];
            var cell = tableView.DequeueReusableCell("cellID");
            cell.TextLabel.Text = item;

            return cell;
        }

        #endregion

        private void Update(List<Section> items)
        {
            this.sections.Clear();
            this.sections.AddRange(items);

            // Show and update the table view.
            this.UpdateInterface(false);
        }
    }
}