using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace StoryboardTable
{
	public partial class DetailViewController : UIViewController
	{
		object detailItem;

		public DetailViewController (IntPtr handle) : base (handle)
		{
		}

		public void SetDetailItem (object newDetailItem)
		{
			if (detailItem != newDetailItem) {
				detailItem = newDetailItem;

				// Update the view
				ConfigureView ();
			}
		}

		void ConfigureView ()
		{
			// Update the user interface for the detail item
			if (IsViewLoaded && detailItem != null)
				detailDescriptionLabel.Text = detailItem.ToString ();
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
			
			// Perform any additional setup after loading the view, typically from a nib.
			ConfigureView ();
		}
	}
}

