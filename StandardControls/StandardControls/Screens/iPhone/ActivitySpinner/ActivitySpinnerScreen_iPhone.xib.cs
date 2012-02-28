
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_StandardControls.Screens.iPhone.ActivitySpinner
{
	public partial class ActivitySpinnerScreen_iPhone : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public ActivitySpinnerScreen_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ActivitySpinnerScreen_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ActivitySpinnerScreen_iPhone () : base("ActivitySpinnerScreen_iPhone", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Activity Spinners";
		}
		
		
	}
}

