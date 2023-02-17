using System;
using System.Threading.Tasks;
using Speech;

namespace PrivacyPrompts {
	public class SpeechRecognitionPrivacyManager : IPrivacyManager {
		public string CheckAccess ()
		{
			return SFSpeechRecognizer.AuthorizationStatus.ToString ();
		}

		public Task RequestAccess ()
		{
			var tcs = new TaskCompletionSource<object> ();

			SFSpeechRecognizer.RequestAuthorization (_ => {
				tcs.SetResult (null);
			});

			return tcs.Task;
		}
	}
}
