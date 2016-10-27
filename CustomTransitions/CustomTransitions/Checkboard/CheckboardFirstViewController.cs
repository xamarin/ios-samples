using System;
using UIKit;

namespace CustomTransitions
{
	public partial class CheckboardFirstViewController : UIViewController
	{
		partial void unwindToMenuViewController(UIBarButtonItem sender)
		{
			DismissViewController(true, null);
		}

		public CheckboardFirstViewController(IntPtr handle)
			: base (handle)
		{
			WeakTransitioningDelegate = this;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

