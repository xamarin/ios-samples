using System;
using WatchKit;
using Foundation;

namespace WatchTablesExtension  
{
	public partial class Table2InterfaceController 
		: WKInterfaceController
	{
		public Table2InterfaceController
		(IntPtr handle) : base (handle)
		{
		}

		string contextFromPreviousScreen = "";

		public override void Awake (NSObject context)
		{
			base.Awake (context);
			// Configure interface objects here.
			if (context is NSString) {
				contextFromPreviousScreen = context.ToString ();
			}

			Console.WriteLine ("{0} awake with context", this);
		}
		public override void WillActivate ()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine ("{0} will activate", this);

			table2Label.SetText (contextFromPreviousScreen);
		}
		public override void DidDeactivate ()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine ("{0} did deactivate", this);
		}
	}
}