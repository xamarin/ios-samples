using System;
using System.Threading.Tasks;
using CloudKit;

namespace CloudKitAtlas
{
	public class DiscoverUserInfoWithUserRecordIdSample : CodeSample
	{
		public DiscoverUserInfoWithUserRecordIdSample ()
			: base (title: "discoverUserInfoWithUserRecordID",
					className: "CKContainer",
					methodName: ".discoverUserInfoWithUserRecordID()", // TODO: fix method name
					descriptionKey: "Discoverability.DiscoverUserInfoWithUserRecordID",
					inputs: new Input [] {
						new TextInput ("recordName", string.Empty, isRequired: true),
						new TextInput ("zoneName", CKRecordZone.DefaultName, isRequired: true)
					})
		{
		}

		public async override Task<Results> Run ()
		{
			object recordName, zoneName;
			if (Data.TryGetValue ("recordName", out recordName) && Data.TryGetValue ("zoneName", out zoneName)) {
				var container = CKContainer.DefaultContainer;
				var zoneId = new CKRecordZoneID ((string)zoneName, CKContainer.OwnerDefaultName);
				var userRecordID = new CKRecordID ((string)recordName, zoneId);

				CKDiscoveredUserInfo userInfo = await container.DiscoverUserInfoAsync (userRecordID);
				return new Results (new IResult [] { new CKDiscoveredUserInfoWrapper (userInfo) });
			}
			throw new InvalidProgramException ("there are no recordName and zoneName");
		}
	}
}
