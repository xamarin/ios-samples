using System;
using EventKit;
using Foundation;

namespace PrivacyPrompts
{
	public class EKEntityPrivacyController : PrivacyDetailViewController
	{
		EKEventStore eventStore = new EKEventStore();

		readonly EKEntityType type;

		public EKEntityPrivacyController (EKEntityType entityType)
		{
			type = entityType;
		}

		protected override string CheckAccess ()
		{
			return EKEventStore.GetAuthorizationStatus (type).ToString ();
		}

		protected override void RequestAccess ()
		{
			eventStore.RequestAccess (type, (granted, accessError) => {
				string text = string.Format ("Access {0}", granted ? "allowed" : "denied");
				InvokeOnMainThread (() => AccessStatus.Text = text);
			});
		}
	}
}

