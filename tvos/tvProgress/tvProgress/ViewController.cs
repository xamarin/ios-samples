using System;
using Foundation;
using UIKit;

namespace MySingleView
{
	public partial class ViewController : UIViewController
	{
		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion

		#region Custom Actions
		partial void StartStopAction (Foundation.NSObject sender) {

			// Take action based on the Activity Indicator state
			if (ActivityIndicator.IsAnimating) {
				StartStopButton.SetTitle("Start",UIControlState.Normal);
				ActivityIndicator.StopAnimating();
			} else {
				StartStopButton.SetTitle("Stop",UIControlState.Normal);
				ActivityIndicator.StartAnimating();
			}
		}

		partial void LessAction (Foundation.NSObject sender) {

			// At minimum?
			if (ProgressBar.Progress > 0) ProgressBar.Progress -=0.10f;
		}

		partial void MoreAction (Foundation.NSObject sender) {
			// At maximum?
			if (ProgressBar.Progress < 1) ProgressBar.Progress +=0.10f;
		}
		#endregion
	}
}


