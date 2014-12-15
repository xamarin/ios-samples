using System;
using AddressBook;

namespace PeoplePicker
{
	public static class PersonFormatter
	{
		public static string GetPickedName(ABPerson person)
		{
			string contactName = person.ToString ();
			return string.Format ("Picked {0}", contactName ?? "No Name");
		}

		public static string GetPickedEmail(ABPerson person, int? identifier = null)
		{
			string emailAddress = "no email address";
			using (ABMultiValue<string> emails = person.GetEmails ()) {
				bool emailExists = emails != null && emails.Count > 0;

				if (emailExists) {
					nint index = identifier.HasValue ? emails.GetIndexForIdentifier (identifier.Value) : 0;
					emailAddress = emails [index].Value;
				}
			}

			return string.Format ("Picked {0}", emailAddress);
		}
	}
}
