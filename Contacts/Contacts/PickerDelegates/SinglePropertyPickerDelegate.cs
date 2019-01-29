using Contacts.Helpers;
using ContactsUI;
using System;
using System.Collections.Generic;

namespace Contacts.PickerDelegates
{
    public class SinglePropertyPickerDelegate : CNContactPickerDelegate
    {
        private readonly Action<List<Section>> callback;

        public SinglePropertyPickerDelegate(Action<List<Section>> callback)
        {
            this.callback = callback;
        }

        public override void DidSelectContactProperty(CNContactPickerViewController picker, CNContactProperty contactProperty)
        {
            if (contactProperty != null)
            {
                var sections = new List<Section>();
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
                this.callback(sections);
            }
        }
    }
}