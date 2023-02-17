using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas {
	class SubscriptionIDResult : IResult {
		readonly string subscriptionId;

		public string SummaryField { get; set; }

		public List<AttributeGroup> AttributeList {
			get {
				return new List<AttributeGroup> {
					new AttributeGroup (title: string.Empty, attributes: new Attribute[] {
						new Attribute (key: "subscriptionID", value: subscriptionId)
					})
				};
			}
		}

		public SubscriptionIDResult (string subscriptionId)
		{
			this.subscriptionId = subscriptionId;
		}
	}

	public class DeleteSubscriptionSample : CodeSample {
		public DeleteSubscriptionSample ()
			: base (title: "DeleteSubscription",
					className: "CKDatabase",
					methodName: ".DeleteSubscription()",
					descriptionKey: "Subscriptions.DeleteSubscription",
					inputs: new Input [] {
					new TextInput (label: "subscriptionID", value: string.Empty, isRequired: true)
					})
		{
		}

		public async override Task<Results> Run ()
		{
			string subscriptionID;
			if (!TryGetString ("subscriptionID", out subscriptionID))
				throw new InvalidProgramException ();

			var container = CKContainer.DefaultContainer;
			var privateDB = container.PrivateCloudDatabase;

			var id = await privateDB.DeleteSubscriptionAsync (subscriptionID);
			var results = new Results ();

			if (id != null)
				results.Items.Add (new SubscriptionIDResult (id));

			return results;
		}
	}
}
