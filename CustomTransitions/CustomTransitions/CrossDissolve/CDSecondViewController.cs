using System;

using UIKit;

namespace CustomTransitions {
	public partial class CDSecondViewController : UIViewController {
		public CDSecondViewController (IntPtr handle) : base (handle)
		{
		}

		partial void DismissAction(UIButton sender)
		{
			DismissViewController (true, null);
		}
	}
}
