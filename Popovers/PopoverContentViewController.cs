using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Foundation;
using UIKit;

namespace Popovers
{
	public partial class PopoverContentViewController : UIViewController
	{
		public PopoverContentViewController (IntPtr handle) : base (handle)
		{
		}
		
		//loads the PopoverContentViewController.xib file and connects it to this object
		public PopoverContentViewController () : base ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		
			var label = new UILabel (new RectangleF (0, 0, 320, 320));
			label.Text = "POP!";
			label.Font = UIFont.BoldSystemFontOfSize (100);
			label.TextAlignment = UITextAlignment.Center;
			label.TextColor = UIColor.Red;

			View.AddSubview (label);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}
