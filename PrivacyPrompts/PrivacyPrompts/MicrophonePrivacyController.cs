using System;
using Foundation;
using UIKit;
using CoreGraphics;
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

		public MicrophonePrivacyController()
		{
			CheckAccess = CheckMicAccess;
			RequestAccess = RequestMicAccess;
		}

		//There is no way to check access prior to showing the dialog
		string CheckMicAccess()
		{
			return micAccessText;
		}

		void RequestMicAccess()
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
				audioSession.RequestRecordPermission (delegate(bool granted) {
					micAccessText = granted ? "granted" : "denied";
					UpdateStatus();
				});
			}
		}
	}

}
