using System;
using System.Drawing;

using UIKit;
using Foundation;
using CoreGraphics;
using System.Linq;

namespace GpsWatch
{
	public partial class GpsWatchViewController : UIViewController
	{
		UILabel lbl;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public GpsWatchViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Console.WriteLine ("GpsWatchViewController loaded");
			ShowDescription ();
		}

		void ShowDescription()
		{
			lbl = new UILabel {
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
				Text = "This is Main iOS app. It hosts watchkit app and watchkit extension. Watch extension requests location changes from MainApp Aslo and passes it to WatchApp",
			};
			lbl.Frame = new CGRect (0, 0, View.Frame.Width - 20, 0);
			lbl.SizeToFit ();
			View.AddSubview (lbl);

			lbl.Center = View.Center;
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

	}
}

