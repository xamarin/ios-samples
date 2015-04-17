using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace DesignerWalkthrough
{
	public partial class FlipsideViewController : UIViewController
	{
		public event EventHandler Done;

		public FlipsideViewController (IntPtr handle) : base (handle)
		{
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		partial void done (UIBarButtonItem sender)
		{
			DismissViewController (true, null);

			if (Done != null)
				Done (this, EventArgs.Empty);
		}
	}
}

