using System;

using WatchKit;
using Foundation;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WatchConnectivity.OnWatchExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		protected InterfaceController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Awake(NSObject context)
		{
			base.Awake(context);

			// Configure interface objects here.
			Console.WriteLine("{0} awake with context", this);
            WCSessionManager.SharedManager.ApplicationContextUpdated += DidReceiveApplicationContext;
		}

		public override void WillActivate()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine("{0} will activate", this);
		}

		public override void DidDeactivate()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine("{0} did deactivate", this);
            WCSessionManager.SharedManager.ApplicationContextUpdated -= DidReceiveApplicationContext;
		}


        public void DidReceiveApplicationContext(WCSession session, Dictionary<string, object> applicationContext)
		{
			var message = (string)applicationContext["MessagePhone"];
			if (message != null)
			{
				Console.WriteLine($"Application context update received : {message}");
				label.SetText($"\ud83d\udcf1 : {message}");
			}
		}

		partial void TopLeftPressed()
		{
			sendEmoji("\ud83c\udf4f");
		}

		partial void TopRightPressed()
		{
			sendEmoji("\ud83c\udf53");
		}

		partial void BottomLeftPressed()
		{
			sendEmoji("\ud83c\udf4a");
		}

		partial void BottomRightPressed()
		{
			sendEmoji("\ud83c\udf4b");
		}


		private void sendEmoji(string emoji)
		{
			WCSessionManager.SharedManager.UpdateApplicationContext(new Dictionary<string, object>() { { "MessageWatch", $"{emoji}" } });
		}
	}
}
