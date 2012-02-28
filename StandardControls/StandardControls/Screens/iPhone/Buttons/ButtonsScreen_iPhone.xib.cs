
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_StandardControls.Screens.iPhone.Buttons
{
	public partial class ButtonsScreen_iPhone : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public ButtonsScreen_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ButtonsScreen_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ButtonsScreen_iPhone () : base("ButtonsScreen_iPhone", null)
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
			this.Title = "Buttons";
			
			this.btnOne.TouchUpInside += HandleBtnOneTouchUpInside;
			this.btnTwo.TouchUpInside += delegate {
				new UIAlertView ("button two click!", "TouchUpInside Handled", null, "OK", null).Show ();
			};
			
			UIButton button = UIButton.FromType (UIButtonType.RoundedRect);
			button.SetTitle ("My Button", UIControlState.Normal);
		}

		protected void HandleBtnOneTouchUpInside (object sender, EventArgs e)
		{
			new UIAlertView ("button one click!", "TouchUpInside Handled", null, "OK", null).Show();
		}
		
	}
}

