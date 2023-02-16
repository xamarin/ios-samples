using System.Collections.Generic;

using CloudKit;
using Contacts;

namespace CloudKitAtlas {
	public class CKDiscoveredUserInfoWrapper : IResult {
		readonly CKDiscoveredUserInfo userInfo;

		public CKDiscoveredUserInfoWrapper (CKDiscoveredUserInfo userInfo)
		{
			this.userInfo = userInfo;
		}

		public List<AttributeGroup> AttributeList {
			get {
				var displayContact = userInfo.DisplayContact;
				if (displayContact == null) {
					return new List<AttributeGroup> {
						 new AttributeGroup ("No displayContact")
					};
				}

				var contactType = "-";

				switch (displayContact.ContactType) {
				case CNContactType.Organization:
					contactType = "Organization";
					break;

				case CNContactType.Person:
					contactType = "Person";
					break;
				}

				return new List<AttributeGroup> {
					new AttributeGroup ("Display Contact:", new Attribute [] {
						new Attribute ("identifier", displayContact.Identifier),
						new Attribute ("contactType", contactType),
						new Attribute ("givenName", displayContact.GivenName),
						new Attribute ("familyName", displayContact.FamilyName)
					})
				};
			}
		}

		public string SummaryField {
			get {
				var contact = userInfo.DisplayContact;
				return (contact != null)
					? $"{contact.GivenName} {contact.FamilyName}"
					: userInfo.UserRecordId.RecordName;
			}
		}
	}
}
