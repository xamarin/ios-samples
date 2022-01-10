using Contacts.Helpers;
using ContactsUI;
using System;
using System.Collections.Generic;

namespace Contacts.PickerDelegates
{
    public class SingleContactPickerDelegate : CNContactPickerDelegate
    {
        private readonly Action<List<Section>> callback;

        public SingleContactPickerDelegate(Action<List<Section>> callback)
        {
            this.callback = callback;
        }

        public override void DidSelectContact(CNContactPickerViewController picker, CNContact contact)
        {
            if (contact != null)
            {
                var sections = new List<Section>
                {
                    new Section { Items = new List<string> { contact.GetFormattedName() } }
                };

                this.callback(sections);
            }
        }
    }
}