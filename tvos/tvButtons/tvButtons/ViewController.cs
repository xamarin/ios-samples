using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace MySingleView
{
	public partial class ViewController : UIViewController
	{
		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			// Add a button via code
			var button = new UIButton(UIButtonType.System);
			button.Frame = new CGRect (ClickMeButton.Frame.Left, ClickMeButton.Frame.Top+ClickMeButton.Frame.Height+25, ClickMeButton.Frame.Width, ClickMeButton.Frame.Height);
			button.SetTitle ("Hello", UIControlState.Normal);
			button.AllEvents += (sender, e) => {
				Console.WriteLine("Hello button clicked!");
			};
			button.SetTitleColor (UIColor.Red, UIControlState.Normal);
			button.SetTitleShadowColor(UIColor.Black, UIControlState.Normal);
			button.ReverseTitleShadowWhenHighlighted = true;
			View.AddSubview (button);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion

		#region Custom Actions
		partial void ButtonPressed (Foundation.NSObject sender) {
			Console.WriteLine("The button was clicked.");
		}
		#endregion
	}
}


