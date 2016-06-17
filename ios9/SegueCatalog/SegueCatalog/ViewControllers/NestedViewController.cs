using System;

using Foundation;
using UIKit;

namespace SegueCatalog
{
	public partial class NestedViewController : UIViewController
	{
		public NestedViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Action ("unwindToNested:")]
		void UnwindToNested (UIStoryboardSegue segue)
		{
			/*
			Empty. Exists solely so that "unwind to nested" segues can find instances
			of this class.

			Notably, if an instance of this class is currently showing a Current
			Context presentation, unwinding to that instance via this action will
			only dismiss that presentation if the unwind source is contained within 
			the presentation.

			This is why the "Dismiss via Unwind" button in this app's storyboard
			will cause the containing presentation to be dismissed, while the "Unwind 
			to Nested" button will not.
			*/
		}
	}
}
