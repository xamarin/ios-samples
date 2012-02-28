using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace Example_StandardControls.Screens.iPad.ScrollView
{
	public class Controller : UIViewController
	{
		UIScrollView scrollView;

		#region -= constructors =-

		public Controller () : base()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set the background color of the view to white
			View.BackgroundColor = UIColor.White;
			
			scrollView = new UIScrollView (View.Frame);
			View.AddSubview (scrollView);
		}
	}
}
