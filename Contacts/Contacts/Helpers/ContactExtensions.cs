using System;
using System.Linq;
using Foundation;

namespace Contacts.Helpers
{
    public static class ContactExtensions
    {
        /// <summary>
        /// The formatted name of a contact if there is one and "No Name", otherwise.
        /// </summary>
        public static string GetFormattedName(this CNContact contact)
        {
            var name = contact != null ? CNContactFormatter.GetStringFrom(contact, CNContactFormatterStyle.FullName)?.Trim() : null;
            return !string.IsNullOrEmpty(name) ? name : "No Name";
        }
    }

    public static class ContactPropertyExtensions
    {
        /// <summary>
        /// The name matching the key of the property.
        /// </summary>
        public static string GetNameMatchingKey(this CNContactProperty property)
        {
            switch (property.Key)
            {
                case "emailAddresses":
                    return "Email address";
                case "phoneNumbers":
                    return "Phone numbers";
                case "postalAddresses":
                    return "Postal address";
                default:
                    return null;
            }
        }

        /// <summary>
        /// The name matching the value of the property.
        /// </summary>
        public static string GetNameMatchingValue(this CNContactProperty property)
        {
            switch (property.Key)
            {
                case "emailAddresses":
                    return property.Value as NSString;
                case "phoneNumbers":
                    if (property.Value is CNPhoneNumber phoneNumber)
                    {
                        return phoneNumber.StringValue;
                    }
                    break;

                case "postalAddresses":
                    if (property.Value is CNPostalAddress address)
                    {
                        return address.GetFormattedPostalAddress();
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// The name matching the localized label of the property.
        /// </summary>
        public static string GetNameMatchingLocalizedLabel(this CNContactProperty property)
        {
            var label = property?.Label;
            if (!string.IsNullOrEmpty(label))
            {
                var nativeLabel = new NSString(label);
                switch (property?.Label)
                {
                    case "emailAddresses":
                        return CNLabeledValue<NSString>.LocalizeLabel(nativeLabel);
                    case "phoneNumbers":
                        return CNLabeledValue<CNPhoneNumber>.LocalizeLabel(nativeLabel);
                    case "postalAddresses":
                        return CNLabeledValue<CNPostalAddress>.LocalizeLabel(nativeLabel);
                }
            }

            return null;
        }
    }

    public static class PostalAddressExtensions
    {
        /// <summary>
        /// The formatted postal address.
        /// </summary>
        public static string GetFormattedPostalAddress(this CNPostalAddress postalAddress)
        {
            string[] address = { postalAddress.Street, postalAddress.City, postalAddress.State, postalAddress.PostalCode, postalAddress.Country };
            var filteredArray = address.Where(item => !string.IsNullOrEmpty(item)).ToArray();

            return filteredArray.Any() ? string.Join(", ", filteredArray) : null;
        }
    }
}