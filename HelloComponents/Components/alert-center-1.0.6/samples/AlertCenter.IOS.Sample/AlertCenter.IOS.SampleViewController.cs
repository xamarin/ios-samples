using System;
using MonoTouch.UIKit;

namespace AlertCenter.IOS.Sample
{
	public partial class AlertCenter_IOS_SampleViewController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public AlertCenter_IOS_SampleViewController (IntPtr handle) : base (handle)
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
			
			// Perform any additional setup after loading the view, typically from a nib.
			Xamarin.Controls.AlertCenter.Default.PostMessage ("Knock knock!", "Who's there?");
			Xamarin.Controls.AlertCenter.Default.PostMessage ("Interrupting cow.", "Interrupting cow who?",
				UIImage.FromFile ("176.png"), delegate {
					Console.WriteLine ("Moo!");
				});

			Xamarin.Controls.AlertCenter.Default.PostMessage ("Long message.", "Lorem Ipsum is simply dummy text of the printing and typesetting industry. \n\nLorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
				UIImage.FromFile ("176.png"), delegate {
					Console.WriteLine ("Moo!");
				});
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

		partial void SendAlert (UIButton sender)
		{
			Xamarin.Controls.AlertCenter.Default.PostMessage ("Download Complete", "\"Puppies Playing.mov\" finished downloading.", UIImage.FromFile ("176.png"));
		}
	}
}

