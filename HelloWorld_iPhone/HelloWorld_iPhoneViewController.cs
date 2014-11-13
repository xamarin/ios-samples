using UIKit;
using CoreGraphics;
using System;
using Foundation;

namespace HelloWorld_iPhone
{
	public partial class HelloWorld_iPhoneViewController : UIViewController
	{
		/// <summary>
		/// a class-level variable that tracks the number of times the button has been clicked.
		/// </summary>
		protected int _numberOfTimesClicked = 0;
		
		public HelloWorld_iPhoneViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
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

	}
}
