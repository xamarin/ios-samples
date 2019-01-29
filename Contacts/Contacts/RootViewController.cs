using Contacts.Helpers;
using ContactsUI;
using CoreFoundation;
using Foundation;
using System;
using System.Linq;
using UIKit;

namespace Contacts
{
    public partial class RootViewController : UITableViewController, ICNContactViewControllerDelegate
    {
        private readonly CNContactStore store = new CNContactStore();

        public RootViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.CheckContactsAccess();
        }

        private void CheckContactsAccess()
        {
            var status = CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts);
            switch (status)
            {
                // Access was granted. Update the UI with the default navigation menu.
                case CNAuthorizationStatus.Authorized:
                    Console.WriteLine("App is authorized");
                    break;

                case CNAuthorizationStatus.NotDetermined:
                    this.store.RequestAccess(CNEntityType.Contacts, (granted, _) =>
                    {
                        if (granted)
                        {
                            Console.WriteLine("App is authorized");
                        }
                    });
                    break;

                // Access was denied or restricted.
                case CNAuthorizationStatus.Restricted:
                case CNAuthorizationStatus.Denied:
                    Console.WriteLine("Access denied or restricted.");
                    break;
            }
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "PickerSegue")
            {
                if (segue.DestinationViewController is ContactPickerController contactPickerController)
                {
                    contactPickerController.Mode = (PickerMode)(sender as NSNumber).Int32Value;
                }
            }
            else if (segue.Identifier == "PredicatePickerSegue")
            {
                if (segue.DestinationViewController is PredicateContactViewController predicateContactViewController)
                {
                    predicateContactViewController.Mode = (PredicatePickerMode)(sender as NSNumber).Int32Value;
                }
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var selectedCell = tableView.CellAt(indexPath);

            // regular contacts logic
            if (indexPath.Section == 0)
            {
                this.PerformContactAction(selectedCell);
            }
            // picker logic
            else if (indexPath.Section == 1)
            {
                this.PerformContactPickerAction(selectedCell);
            }
            // predicate picker logic
            else if (indexPath.Section == 2)
            {
                this.PerformContactPredicatePickerAction(selectedCell);
            }

            tableView.DeselectRow(indexPath, false);
        }

        #region Contact predicate picker logic (3d section)

        private void PerformContactPredicatePickerAction(UITableViewCell selectedCell)
        {
            if (selectedCell == this.enableContactsPredicateCell)
            {
                base.PerformSegue("PredicatePickerSegue", new NSNumber((int)PredicatePickerMode.EnableContacts));
            }
            else if (selectedCell == this.selectContactsPredicateCell)
            {
                base.PerformSegue("PredicatePickerSegue", new NSNumber((int)PredicatePickerMode.SelectContacts));
            }
        }

        #endregion

        #region Contact picker logic (2st section)

        private void PerformContactPickerAction(UITableViewCell selectedCell)
        {
            var mode = default(PickerMode);
            if (selectedCell == this.pickSingleContactCell)
            {
                mode = PickerMode.SingleContact;
            }
            else if (selectedCell == this.pickSinglePropertyCell)
            {
                mode = PickerMode.SingleProperty;
            }
            else if (selectedCell == this.pickMultipleContactsCell)
            {
                mode = PickerMode.MultupleContacts;
            }
            else if (selectedCell == this.pickMultiplePropertiesCell)
            {
                mode = PickerMode.MultipleProperties;
            }

            base.PerformSegue("PickerSegue", new NSNumber((int)mode));
        }

        #endregion

        #region Regular contact logic (1st section)

        private void PerformContactAction(UITableViewCell selectedCell)
        {
            if (selectedCell == this.createNewContactCell)
            {
                // Create an empty contact view controller.
                var contactViewController = CNContactViewController.FromNewContact(null);
                // Set its delegate.
                contactViewController.Delegate = this;
                // Push it using the navigation controller.
                base.NavigationController.PushViewController(contactViewController, true);
            }
            // Called when users tap "Create New Contact With Existing Data" in the UI.
            // Create and launch a contacts view controller with pre - filled fields.
            else if (selectedCell == this.createNewContactExistingData)
            {
                var contact = new CNMutableContact
                {
                    // Given and family names.
                    FamilyName = Name.Family,
                    GivenName = Name.Given,
                };

                // Phone numbers.
                contact.PhoneNumbers = new CNLabeledValue<CNPhoneNumber>[]
                {
                        new CNLabeledValue<CNPhoneNumber>(PhoneNumber.IPhone,
                                                          new CNPhoneNumber(PhoneNumber.IPhone)),
                        new CNLabeledValue<CNPhoneNumber>(PhoneNumber.Mobile,
                                                          new CNPhoneNumber(PhoneNumber.Mobile))
                };

                // Postal address.
                var homeAddress = new CNMutablePostalAddress
                {
                    Street = Address.Street,
                    City = Address.City,
                    State = Address.State,
                    PostalCode = Address.PostalCode,
                };

                contact.PostalAddresses = new CNLabeledValue<CNPostalAddress>[] { new CNLabeledValue<CNPostalAddress>(CNLabelKey.Home, homeAddress) };

                // Create a contact view controller with the above contact.
                var contactViewController = CNContactViewController.FromNewContact(contact);
                // Set its delegate.
                contactViewController.Delegate = this;
                // Push it using the navigation controller.
                base.NavigationController.PushViewController(contactViewController, true);
            }
            // Called when users tap "Edit Unknown Contact" in the UI. 
            // The view controller displays some contact information that you can either add to an existing contact or use them to create a new contact.
            else if (selectedCell == this.editContactCell)
            {
                var contact = new CNMutableContact();

                // Phone number.
                contact.PhoneNumbers = new CNLabeledValue<CNPhoneNumber>[] { new CNLabeledValue<CNPhoneNumber>(CNLabelPhoneNumberKey.iPhone, new CNPhoneNumber(PhoneNumber.Mobile)) };

                // Postal address.
                var homeAddress = new CNMutablePostalAddress()
                {
                    Street = Address.Street,
                    City = Address.City,
                    State = Address.State,
                    PostalCode = Address.PostalCode,
                };

                contact.PostalAddresses = new CNLabeledValue<CNPostalAddress>[] { new CNLabeledValue<CNPostalAddress>(CNLabelKey.Home, homeAddress) };

                // Create a view controller that allows editing.
                var contactViewController = CNContactViewController.FromUnknownContact(contact);
                contactViewController.AllowsEditing = true;
                contactViewController.ContactStore = new CNContactStore();
                contactViewController.Delegate = this;

                // Push the unknown contact in the view controler.
                base.NavigationController.PushViewController(contactViewController, true);
            }
            // Called when users tap "Display and Edit Contact" in the UI. 
            // Searches for the contact specified whose last name and first name are respectively specified by contact.family and contact.given
            else if (selectedCell == this.displayEditCell)
            {
                var name = $"{Name.Given} {Name.Family}";
                this.FetchContact(name, (contacts) =>
                {
                    if (contacts.Any())
                    {
                        var contactViewController = CNContactViewController.FromContact(contacts[0]);
                        contactViewController.AllowsEditing = true;
                        contactViewController.AllowsActions = true;
                        contactViewController.Delegate = this;

                        /*
                            Set the view controller's highlightProperty if
                            highlightedPropertyIdentifier exists. Thus, ensuring
                            that the contact's phone number specified by
                            highlightedPropertyIdentifier will be highlighted in the
                            UI.
                        */

                        var highlightedPropertyIdentifiers = contacts[0].PhoneNumbers.FirstOrDefault()?.Identifier;
                        if (!string.IsNullOrEmpty(highlightedPropertyIdentifiers))
                        {
                            contactViewController.HighlightProperty(new NSString("phoneNumbers"), highlightedPropertyIdentifiers);
                        }

                        // Show the view controller.
                        base.NavigationController.PushViewController(contactViewController, true);
                    }
                    else
                    {
                        this.ShowAlert($"Could not find {name} in Contacts.");
                    }
                });
            }
        }

        /// <summary>
        /// Exising contacts matching the specified name.
        /// </summary>
        private void FetchContact(string name, Action<CNContact[]> completion)
        {
            var result = store.GetUnifiedContacts(CNContact.GetPredicateForContacts(name),
                                                  new ICNKeyDescriptor[] { CNContactViewController.DescriptorForRequiredKeys },
                                                  out NSError error);
            if (error != null)
            {
                Console.WriteLine($"Error: {error.LocalizedDescription}");
            }
            else
            {
                DispatchQueue.MainQueue.DispatchAsync(() => completion(result));
            }
        }

        #endregion

        #region ICNContactViewControllerDelegate

        /// <summary>
        /// Setting it to false prevents users to perform default actions such as dialing a phone number, when they select a contact property.
        /// </summary>
        [Export("contactViewController:shouldPerformDefaultActionForContactProperty:")]
        public bool ShouldPerformDefaultAction(CNContactViewController viewController, CNContactProperty property)
        {
            return false;
        }

        /// <summary>
        /// Used to dismiss the view controller when using init(for​New​Contact:​) to create a new contact.
        /// </summary>
        [Export("contactViewController:didCompleteWithContact:")]
        public void DidComplete(CNContactViewController viewController, CNContact contact)
        {
            base.NavigationController.PopViewController(true);
            if (contact != null)
            {
                base.NavigationController.ShowAlert($"{contact.GetFormattedName()} was successfully added.");
            }
        }

        #endregion

        /*  helpers  */

        static class PhoneNumber
        {
            public static string IPhone { get; } = "(408) 555-0126";
            public static string Mobile { get; } = "(415) 123-4567";
        }

        static class Name
        {
            public static string Family { get; } = "Appleseed";
            public static string Given { get; } = "Jane";
        }

        static class Address
        {
            public static string Street { get; } = "1 Infinite Loop";
            public static string City { get; } = "Cupertino";
            public static string State { get; } = "CA";
            public static string PostalCode { get; } = "95014";
        }
    }
}