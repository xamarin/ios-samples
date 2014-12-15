using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.AddressBook;
using MonoTouch.EventKit;
using MonoTouch.AssetsLibrary;
using MonoTouch.AVFoundation;
using MonoTouch.CoreBluetooth;
using MonoTouch.Accounts;
using MonoTouch.AdSupport;
using MonoTouch.CoreLocation;

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
