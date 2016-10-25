using Foundation;
using System;
using UIKit;
using WatchKit;

namespace WatchTables.OnWatchExtension
{
	public partial class DetailController : WKInterfaceController
	{
		string contextFromPreviousScreen = "";

		public DetailController(IntPtr handle) : base(handle)
		{
		}

		public override void Awake(NSObject context)
		{
			base.Awake(context);
			// Configure interface objects here.
			if (context is NSString)
			{
				contextFromPreviousScreen = context.ToString();
			}

			Console.WriteLine("{0} awake with context", this);
		}
		public override void WillActivate()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine("{0} will activate", this);

			SelectedLabel.SetText(contextFromPreviousScreen);
		}
	}
}