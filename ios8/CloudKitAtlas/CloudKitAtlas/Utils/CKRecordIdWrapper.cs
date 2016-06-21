using System.Collections.Generic;
using CloudKit;

namespace CloudKitAtlas
{
	public class CKRecordIdWrapper : IResult
	{
		readonly CKRecordID record;

		public CKRecordIdWrapper (CKRecordID record)
		{
			this.record = record;
		}

		public List<AttributeGroup> AttributeList {
			get {
				var zoneName = record.ZoneId.ZoneName;
				var ownerName = record.ZoneId.OwnerName;

				return new List<AttributeGroup> {
					new AttributeGroup ("Record ID:",new Attribute [] {
						new Attribute (key: "recordName", value: record.RecordName),
						new Attribute (key: "zoneID"),
						new Attribute (key: "zoneName", value: zoneName, isNested: true),
						new Attribute (key: "ownerName", value: ownerName, isNested: true)
					})
				};
			}
		}

		public string SummaryField {
			get {
				return record.RecordName;
			}
		}
	}
}