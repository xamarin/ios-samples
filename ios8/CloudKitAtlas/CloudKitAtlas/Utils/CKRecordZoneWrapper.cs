using System.Collections.Generic;

using CloudKit;

namespace CloudKitAtlas
{
	public class CKRecordZoneWrapper : IResult
	{
		readonly CKRecordZone zone;

		public string SummaryField {
			get {
				return zone.ZoneId.ZoneName;
			}
		}

		public List<AttributeGroup> AttributeList {
			get {
				return new List<AttributeGroup> {
					new AttributeGroup ("Record Zone:", new Attribute[] {
						new Attribute ("zoneID"),
						new Attribute ("zoneName", zone.ZoneId.ZoneName, isNested: true),
						new Attribute ("ownerName", value: zone.ZoneId.OwnerName, isNested: true),
						new Attribute ("capabilities"),
						new Attribute ("FetchChanges", FetchChangesValue , isNested: true),
						new Attribute ("Atomic", AtomicValue, isNested: true)
					})
				};
			}
		}

		string FetchChangesValue {
			get {
				return zone.Capabilities.HasFlag (CKRecordZoneCapabilities.FetchChanges).ToString ();
			}
		}

		string AtomicValue {
			get {
				return zone.Capabilities.HasFlag (CKRecordZoneCapabilities.Atomic).ToString ();
			}
		}

		public CKRecordZoneWrapper (CKRecordZone zone)
		{
			this.zone = zone;
		}
	}
}
