
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_CoreAnimation.Screens.iPad.ViewTransitions
{
	public partial class BackTransitionViewController : UIViewController
	{
		public event EventHandler<EventArgs> BackClicked;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public BackTransitionViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public BackTransitionViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public BackTransitionViewController () : base("BackTransitionViewController", null)
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
			
			this.btnBack.TouchUpInside += (s, e) => {
				if(this.BackClicked != null)
					this.BackClicked(this, e);
			};
		}
	}
}

