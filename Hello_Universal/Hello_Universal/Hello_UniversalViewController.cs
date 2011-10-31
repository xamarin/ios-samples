using MonoTouch.UIKit;
using System.Drawing;
using System;
using MonoTouch.Foundation;

namespace Hello_Universal
{
	public partial class Hello_UniversalViewController : UIViewController
	{
		public Hello_UniversalViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
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
			
			this.btnClickMe.TouchUpInside += (sender, e) => {
				this.lblOutput.Text = "Clicked @ " + DateTime.Now.ToShortTimeString();
			};
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
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} else {
				return true;
			}
		}
	}
}
