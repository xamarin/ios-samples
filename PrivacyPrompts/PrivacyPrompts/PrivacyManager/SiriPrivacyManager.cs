using System;
using System.Threading.Tasks;
using Intents;

namespace PrivacyPrompts
{
    public class SiriPrivacyManager : IPrivacyManager
    {

        public string CheckAccess()
        {
            return INPreferences.SiriAuthorizationStatus.ToString();
        }

        public Task RequestAccess()
        {
            var tcs = new TaskCompletionSource<object>();

			INPreferences.RequestSiriAuthorization(_ => {
                tcs.SetResult(null);
			});

            return tcs.Task;
        }
    }
}
