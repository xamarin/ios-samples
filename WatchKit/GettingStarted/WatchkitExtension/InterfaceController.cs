using System;

using WatchKit;
using Foundation;

namespace WK2X
{
	public partial class InterfaceController : WKInterfaceController
	{
		public InterfaceController (IntPtr handle) : base (handle)
		{
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);
		}

		int clickCount = 0;

		partial void OnButtonPress ()
		{
			var msg = String.Format("Clicked {0} times", ++clickCount);
			myLabel.SetText(msg);
		}

		public override void WillActivate ()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine ("{0} did deactivate", this);
		}
	}
}

