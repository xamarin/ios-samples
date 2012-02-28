
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_StandardControls.Screens.iPhone.Toolbar
{
	public partial class Toolbar1_iPhone : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public Toolbar1_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public Toolbar1_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public Toolbar1_iPhone () : base("Toolbar1_iPhone", null)
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
			
			this.Title = "Toolbar";
			
			this.btnOne.Clicked += (s, e) => { new UIAlertView ("click!", "btnOne clicked", null, "OK", null).Show (); };
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

