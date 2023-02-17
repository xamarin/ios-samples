using System;

using WatchKit;
using Foundation;
using ClockKit;

namespace iOSApp.WatchAppExtension {
	public partial class InterfaceController : WKInterfaceController {
		protected InterfaceController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			// Configure interface objects here.
			Console.WriteLine ("{0} awake with context", this);
		}

		public override void WillActivate ()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine ("{0} will activate", this);

			var c = NSUserDefaults.StandardUserDefaults ["complication"];
			if (c != null) {
				MessageText.SetText (c.ToString ()); // display what they typed last time
			}
		}


		public override void DidDeactivate ()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine ("{0} did deactivate", this);
		}

		string enteredText = "";
		partial void SetMessageClicked ()
		{
			var suggest = new string [] { "Hi Watch!" };

			PresentTextInputController (suggest, WatchKit.WKTextInputMode.AllowEmoji, (result) => {
				// action when the "text input" is complete
				if (result != null && result.Count > 0) {
					// this only works if result is a text response (Plain or AllowEmoji)
					enteredText = result.GetItem<NSObject> (0).ToString ();
					Console.WriteLine (enteredText);
					// do something, such as myLabel.SetText(enteredText);
					MessageText.SetText (enteredText);
					NSUserDefaults.StandardUserDefaults.SetString (enteredText, "complication");
				}
			});
		}


		partial void UpdateClicked ()
		{
			Console.WriteLine ("BUTTON CLICKED");
			var complicationServer = CLKComplicationServer.SharedInstance; // is null :-(
			if (complicationServer.ActiveComplications != null) {
				Console.WriteLine ("Active complications!!!!!!!!!!");
				foreach (var complication in complicationServer.ActiveComplications) {
					Console.WriteLine ("Active " + complication.Description ?? "null");

					complicationServer.ReloadTimeline (complication);
				}
			} else {
				Console.WriteLine ("No active complications");
			}
		}
	}
}
