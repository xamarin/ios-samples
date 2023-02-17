using System.Collections.Generic;

using CloudKit;

namespace CloudKitAtlas {
	public class CKRecordZoneIdWrapper : IResult {
		readonly CKRecordZoneID zoneId;

		public string SummaryField {
			get {
				return zoneId.ZoneName;
			}
		}

		public List<AttributeGroup> AttributeList {
			get {
				return new List<AttributeGroup> {
					new AttributeGroup("Record Zone ID:", new Attribute[] {
						new Attribute("zoneName", zoneId.ZoneName),
						new Attribute("ownerName", zoneId.OwnerName)
					})
				};
			}
		}

		public CKRecordZoneIdWrapper (CKRecordZoneID zoneId)
		{
			this.zoneId = zoneId;
		}
	}
}
