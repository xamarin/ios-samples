using System;
using System.Threading.Tasks;
using AdSupport;

namespace PrivacyPrompts
{
	public class AdvertisingPrivacyManager : IPrivacyManager
	{
		public Task RequestAccess ()
		{
			return Task.FromResult<object> (null);
		}

		public string CheckAccess ()
		{
			return
				ASIdentifierManager.SharedManager.IsAdvertisingTrackingEnabled ?
				"granted" : "denied";
		}
	}
}

