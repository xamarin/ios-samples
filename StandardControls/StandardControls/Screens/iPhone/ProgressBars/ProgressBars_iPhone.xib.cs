
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_StandardControls.Screens.iPhone.ProgressBars
{
	public partial class ProgressBars_iPhone : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public ProgressBars_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ProgressBars_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ProgressBars_iPhone () : base("ProgressBars_iPhone", null)
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
			
			this.Title = "Progress Bars";
			
			//UIProgressView progressBar = new UIProgressView (UIProgressViewStyle.Bar);
			//progressBar.Progress = .5f;
		}
		
	}
}

