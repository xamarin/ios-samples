using System;
using MonoTouch.EventKit;
using MonoTouch.Foundation;

namespace PrivacyPrompts
{
	public class EKEntityPrivacyController : PrivacyDetailViewController
	{
		EKEventStore eventStore = new EKEventStore();

		public EKEntityPrivacyController (EKEntityType entityType)
		{
			CheckAccess = () => CheckEventStoreAccess (entityType);
			RequestAccess = () => RequestEventStoreAccess (entityType);
		}


		string CheckEventStoreAccess (EKEntityType type)
		{
			return EKEventStore.GetAuthorizationStatus (type).ToString ();
		}

		public void RequestEventStoreAccess (EKEntityType type)
		{
			eventStore.RequestAccess (type, delegate (bool granted, NSError error) {
				InvokeOnMainThread(() => accessStatus.Text = "Access " + (granted ? "allowed" : "denied"));
			});
		}

	}
}

