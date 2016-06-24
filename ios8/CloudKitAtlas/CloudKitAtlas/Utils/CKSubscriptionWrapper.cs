using System;
using System.Collections.Generic;
using CloudKit;

namespace CloudKitAtlas
{
	public class CKSubscriptionWrapper : IResult
	{
		readonly CKSubscription subscription;

		public CKSubscriptionWrapper (CKSubscription subscription)
		{
		}

		public List<AttributeGroup> AttributeList {
			get {
				throw new NotImplementedException ();
			}
		}

		public string SummaryField {
			get {
				throw new NotImplementedException ();
			}
		}
	}
}

