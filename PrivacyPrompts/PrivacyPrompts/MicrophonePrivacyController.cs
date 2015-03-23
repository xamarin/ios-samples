using System;
using Foundation;
using UIKit;
using CoreGraphics;
using AddressBook;
using EventKit;
using AssetsLibrary;
using AVFoundation;
using CoreBluetooth;
using Accounts;
using AdSupport;
using CoreLocation;

namespace PrivacyPrompts
{

	public class MicrophonePrivacyController : PrivacyDetailViewController
	{
		string micAccessText = "Not determined";

		// There is no way to check access prior to showing the dialog
		protected override string CheckAccess ()
		{
			return micAccessText;
		}

		protected override void RequestAccess ()
		{
			RequestMicrophoneAccess (true);
			RequestMicrophoneAccess (false);
		}

		public void RequestMicrophoneAccess (bool usePermissionAPI)
		{
			AVAudioSession audioSession = AVAudioSession.SharedInstance ();
			if (!usePermissionAPI) {
				NSError error;
				audioSession.SetCategory (AVAudioSession.CategoryRecord, out error);
				UpdateStatus ();
			} else {
				audioSession.RequestRecordPermission (granted => {
					micAccessText = string.Format ("Access {0}", granted ? "allowed" : "denied");
					UpdateStatus();
				});
			}
		}
	}

}
