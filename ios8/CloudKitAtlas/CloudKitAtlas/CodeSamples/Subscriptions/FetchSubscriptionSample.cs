using System;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas
{
	public class FetchSubscriptionSample : CodeSample
	{
		public FetchSubscriptionSample ()
			: base (title: "FetchSubscription",
					className: "CkDatabase",
					methodName: ".FetchSubscription()",
					descriptionKey: "Subscriptions.FetchSubscription",
					inputs: new Input [] {
						new TextInput (label: "subscriptionID", value: string.Empty, isRequired: true)
					})
		{
		}

		public async override Task<Results> Run ()
		{
			string subscriptionId;
			if (!TryGetString ("subscriptionID", out subscriptionId))
				throw new InvalidProgramException ();

			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			var subscription = await privateDB.FetchSubscriptionAsync (subscriptionId);
			var results = new Results ();

			if (subscription != null)
				results.Items.Add (new CKSubscriptionWrapper (subscription));

			return results;
		}
	}
}
