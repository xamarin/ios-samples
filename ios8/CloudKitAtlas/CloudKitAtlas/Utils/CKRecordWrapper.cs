using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CloudKit;
using CoreLocation;

namespace CloudKitAtlas
{
	public class CKRecordWrapper : IResult
	{
		public CKRecord Record { get; }

		public CKRecordWrapper (CKRecord record)
		{
			Record = record;
		}

		public List<AttributeGroup> AttributeList {
			get {
				var dateFormatter = new NSDateFormatter {
					DateStyle = NSDateFormatterStyle.Medium
				};

				var metadata = new List<Attribute> ();
				var fields = new List<Attribute> ();

				metadata.Add (new Attribute (key: "RecordType", value: Record.RecordType));
				metadata.Add (new Attribute (key: "RecordId"));
				metadata.Add (new Attribute (key: "RecordName", value: Record.Id.RecordName, isNested: true));
				metadata.Add (new Attribute (key: "ZoneId.ZoneName", value: Record.Id.ZoneId.ZoneName, isNested: true));
				metadata.Add (new Attribute (key: "ZoneId.OwnerName", value: Record.Id.ZoneId.OwnerName, isNested: true));
				metadata.Add (new Attribute (key: "RecordChangeTag", value: Record.RecordChangeTag ?? string.Empty));

				var modificationDate = Record.ModificationDate;
				if (modificationDate != null)
					metadata.Add (new Attribute (key: "ModificationDate", value: dateFormatter.StringFor (modificationDate)));

				var creationDate = Record.CreationDate;
				if (creationDate != null)
					metadata.Add (new Attribute (key: "CreationDate", value: dateFormatter.StringFor (creationDate)));

				foreach (var key in Record.AllKeys ()) {
					NSObject value = Record [key];

					string str = value as NSString;
					if (str != null) {
						fields.Add (new Attribute (key, str));
						break;
					}

					var location = value as CLLocation;
					if (location != null) {
						var coordinate = location.Coordinate;
						fields.Add (new Attribute (key));
						fields.Add (new Attribute (key: "Coordinate.Latitude", value: coordinate.Latitude.ToString (), isNested: true));
						fields.Add (new Attribute (key: "Coordinate.Longitude", value: coordinate.Longitude.ToString (), isNested: true));
						break;
					}
					var asset = value as CKAsset;
					var path = asset?.FileUrl?.Path;
					if (path != null) {
						fields.Add (new Attribute (key));
						fields.Add (new Attribute (key: "FileUrl.Path", value: path, image: UIImage.FromFile (path)));
						break;
					}
				}

				var attributeGroups = new List<AttributeGroup> {
					new AttributeGroup (title: "Metadata:", attributes: metadata)
				};
				if (fields.Count > 0)
					attributeGroups.Add (new AttributeGroup (title: "Fields:", attributes: fields));

				return attributeGroups;
			}
		}

		public string SummaryField {
			get {
				string name;

				if (TryGetString ("name", out name))
					return name;

				if (TryGetString ("Name", out name))
					return name;

				string value;
				foreach (var key in Record.AllKeys ()) {
					if (TryGetString (key, out value))
						return value;
				}

				return Record.Id.RecordName;
			}
		}

		bool TryGetString (string key, out string value)
		{
			var str = Record [key] as NSString;
			value = str;

			return value != null;
		}
	}
}
