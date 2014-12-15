using UIKit;
using CoreGraphics;
using System;
using Foundation;

namespace Transitioning_To_Xcode_4
{
	public partial class Transitioning_To_Xcode_4ViewController : UIViewController
	{
		/// <summary>
		/// Internal variable that tracks number of times the click me button was clicked.
		/// </summary>
		protected int _numberOfTimesClicked = 0;

		public Transitioning_To_Xcode_4ViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//any additional setup after loading the view, typically from a nib.

			//---- wire up our click me button
			this.btnClickMe.TouchUpInside += (sender, e) => {
				this._numberOfTimesClicked++;
				this.lblOutput.Text = "Clicked [" + this._numberOfTimesClicked.ToString() + "] times!";
			};
		}

		/// <summary>
		/// This is our common action handler. Two buttons call this via an action method.
		/// </summary>
		partial void actnButtonClick (Foundation.NSObject sender)
		{
			this.lblOutput.Text = "Action button " +  ((UIButton)sender).CurrentTitle + " clicked.";
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			// Release any retained subviews of the main view.
			// e.g. this.myOutlet = null;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}
