using System;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	public class DiscoverUserInfoWithEmailAddressSample : CodeSample
	{
		public DiscoverUserInfoWithEmailAddressSample ()
			: base (title: "discoverUserInfoWithEmailAddress",
					className: "CKContainer",
					methodName: ".DiscoverUserInfo(email)",
					descriptionKey: "Discoverability.DiscoverUserInfoWithEmailAddress",
					inputs: new Input [] {
				new TextInput (label: "emailAddress", value: string.Empty, isRequired: true, type: TextInputType.Email)
			})
		{
		}

		public async override Task<Results> Run ()
		{
			object emailAddress;
			if (!Data.TryGetValue ("emailAddress", out emailAddress))
				throw new InvalidProgramException ();

			var container = CKContainer.DefaultContainer;
			CKDiscoveredUserInfo userInfo = await container.DiscoverUserInfoAsync ((string)emailAddress);
			return new Results (new IResult [] { new CKDiscoveredUserInfoWrapper (userInfo) });
		}
	}
}