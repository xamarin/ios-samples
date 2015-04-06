using System;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using UIKit;

namespace PrivacyPrompts
{
	public class MicrophonePrivacyManager : IPrivacyManager
	{
		string micAccessText = "Not determined";

		public Task RequestAccess ()
		{
			var tcs = new TaskCompletionSource<object> ();

			AVAudioSession audioSession = AVAudioSession.SharedInstance ();
			audioSession.RequestRecordPermission (granted => {
				micAccessText = GetTextStatus(granted);
				tcs.SetResult(null);
			});

			return tcs.Task;
		}

		// There is no way to check access prior to showing the dialog on iOS below 8.0
		public string CheckAccess ()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var permission = AVAudioSession.SharedInstance ().RecordPermission;
				if (permission == AVAudioSessionRecordPermission.Undetermined)
					return micAccessText;

				micAccessText = GetTextStatus (permission == AVAudioSessionRecordPermission.Granted);
			}
			return micAccessText;
		}

		static string GetTextStatus(bool granted)
		{
			return string.Format ("Access {0}", granted ? "allowed" : "denied");
		}
	}
}