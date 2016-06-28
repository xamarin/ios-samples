using System.Collections.Generic;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	public class PermissionStatus : IResult
	{
		readonly Attribute attribute;

		public string SummaryField { get; }

		public List<AttributeGroup> AttributeList { get; }

		public PermissionStatus (CKApplicationPermissionStatus status)
		{
			var value = (status == CKApplicationPermissionStatus.Granted) ? "Granted" : "Denied";
			attribute = new Attribute ("CKApplicationPermissionStatus", value);

			AttributeList = new List<AttributeGroup> {
				new AttributeGroup ("Discoverability Status:", new Attribute [] { attribute })
			};
		}
	}

	public class RequestApplicationPermissionSample : CodeSample
	{
		public RequestApplicationPermissionSample ()
			: base (title: "RequestApplicationPermission",
					className: "CKContainer",
					methodName: ".RequestApplicationPermission()",
					descriptionKey: "Discoverability.RequestApplicationPermission")
		{
		}

		public async override Task<Results> Run ()
		{
			var container = CKContainer.DefaultContainer;
			var status = await container.RequestApplicationPermissionAsync (CKApplicationPermissions.UserDiscoverability);
			return new Results (new IResult [] { new PermissionStatus(status) });
		}
	}
}