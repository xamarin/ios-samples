using System;

using UIKit;

namespace CustomTransitions
{
	public partial class CDSecondViewController : UIViewController
	{
		

		public CDSecondViewController(IntPtr handle)
			: base (handle)
		{

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

		partial void dismiisAction(UIButton sender)
		{
			this.DismissViewController(true, null);
		}
	}
}

