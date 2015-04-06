using System;
using EventKit;
using Foundation;
using System.Threading.Tasks;

namespace PrivacyPrompts
{
	public class EKEntityPrivacyManager : IPrivacyManager, IDisposable
	{
		readonly EKEntityType type;
		readonly EKEventStore eventStore = new EKEventStore();

		public EKEntityPrivacyManager (EKEntityType entityType)
		{
			type = entityType;
		}

		public Task RequestAccess ()
		{
			var tcs = new TaskCompletionSource<object> ();

			eventStore.RequestAccess (type, (granted, accessError) => tcs.SetResult (null));
			return tcs.Task;
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