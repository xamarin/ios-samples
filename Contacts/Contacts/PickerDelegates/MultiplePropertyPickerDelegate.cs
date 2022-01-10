using Contacts.Helpers;
using ContactsUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contacts.PickerDelegates
{
    public class MultiplePropertyPickerDelegate : CNContactPickerDelegate
    {
        private readonly Action<List<Section>> callback;

        public MultiplePropertyPickerDelegate(Action<List<Section>> callback)
        {
            this.callback = callback;
        }

        public override void DidSelectContactProperties(CNContactPickerViewController picker, CNContactProperty[] contactProperties)
        {
            if (contactProperties != null && contactProperties.Any())
            {
                var sections = new List<Section>();
                foreach (var contactProperty in contactProperties)
                {
                    var section = new Section { Items = new List<string>() };

                    var nameKey = contactProperty.GetNameMatchingKey();
                    if (!string.IsNullOrEmpty(nameKey))
                    {
                        section.Items.Add($"Contact: {contactProperty.Contact.GetFormattedName()}");
                        section.Items.Add($"Key: {nameKey}");
                    }

                    // Attempt to fetch the localized label of the property.
                    var localizedLabel = contactProperty.GetNameMatchingLocalizedLabel();
                    if (!string.IsNullOrEmpty(localizedLabel))
                    {
                        section.Items.Add($"Label : {localizedLabel}");
                    }

                    // Attempt to fetch the value of the property.
                    var value = contactProperty.GetNameMatchingValue();
                    if (!string.IsNullOrEmpty(value))
                    {
                        section.Items.Add($"Value: {value}");
                    }

                    sections.Add(section);
                }

                this.callback(sections);
            }
        }
    }
}