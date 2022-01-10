using Contacts.Helpers;
using ContactsUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contacts.PickerDelegates
{
    public class MultipleContactPickerDelegate : CNContactPickerDelegate
    {
        private readonly Action<List<Section>> callback;

        public MultipleContactPickerDelegate(Action<List<Section>> callback)
        {
            this.callback = callback;
        }

        public override void DidSelectContacts(CNContactPickerViewController picker, CNContact[] contacts)
        {
            if(contacts != null && contacts.Any())
            {
                var section = new Section { Items = new List<string>() };
                foreach (var contact in contacts)
                {
                    section.Items.Add(contact.GetFormattedName());
                }

                this.callback(new List<Section> { section });
            }
        }
    }
}