using System;

using Foundation;
using UIKit;

namespace SegueCatalog
{
	public partial class OuterViewController : UIViewController
	{
		public OuterViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Action("unwindToOuter:")]
		void UnwindToOuter(UIStoryboardSegue segue)
		{
			/*
			Empty. Exists solely so that "unwind to outer" segues can find 
			instances of this class.
			*/
		}
	}
}
