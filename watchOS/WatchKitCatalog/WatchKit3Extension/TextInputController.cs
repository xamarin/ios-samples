/*
 * This controller demonstrates using the Text Input Controller.
*/

using System;

using WatchKit;
using Foundation;
using WatchConnectivity;

namespace WatchkitExtension {
	public partial class TextInputController : WKInterfaceController {
		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine ("{0} did deactivate", this);
		}

		partial void ReplyWithTextInputController (NSObject obj)
		{
			// Using the WKTextInputMode enum, you can specify which aspects of the Text Input Controller are shown when presented.
			PresentTextInputController (new [] { "Yes", "No", "Maybe" }, WKTextInputMode.AllowAnimatedEmoji, delegate (NSArray results)
			{
				Console.WriteLine ("Text Input Results: {0}", results);

				if (results != null) {
					// Sends a non-nil result to the parent iOS application.
					WCSession.DefaultSession.SendMessage (new NSDictionary<NSString, NSObject> (new NSString ("TextInput"), results.GetItem<NSString> (0)), delegate (NSDictionary<NSString, NSObject> replyMessage)
					{
						Console.WriteLine ("Reply Info: {0}", replyMessage);
					}, delegate (NSError error)
					{
						Console.WriteLine ("Error: {0}", error != null ? error.LocalizedDescription : "null");
					});
				}
			});
		}
	}
}

