using System;

using UIKit;

namespace RawExpose
{
	public partial class ViewController : UIViewController
	{
		protected ViewController (IntPtr handle)
			: base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
	}
}
