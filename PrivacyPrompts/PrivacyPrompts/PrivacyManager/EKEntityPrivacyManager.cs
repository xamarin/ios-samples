using System;
using EventKit;
using Foundation;

namespace PrivacyPrompts
{
	public class EKEntityPrivacyManager : IPrivacyManager
	{
		readonly IPrivacyViewController viewController;
		readonly EKEntityType type;
		readonly EKEventStore eventStore = new EKEventStore();

		public EKEntityPrivacyManager (IPrivacyViewController vc, EKEntityType entityType)
		{
			viewController = vc;
			type = entityType;
		}

		public void RequestAccess ()
		{
			eventStore.RequestAccess (type, (granted, accessError) => {
				string text = string.Format ("Access {0}", granted ? "allowed" : "denied");
				eventStore.InvokeOnMainThread (() => viewController.AccessStatus.Text = text);
			});
		}

		public string CheckAccess ()
		{
			return EKEventStore.GetAuthorizationStatus (type).ToString ();
		}

		public void Dispose ()
		{
			eventStore.Dispose ();
		}
	}
}