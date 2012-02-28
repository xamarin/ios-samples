using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Example_StandardControls.Controls;

namespace Example_StandardControls.Screens.iPhone.PagerControl
{
	public class Controller_3 : UIViewController
	{
		UILabel lblMain;


		#region -= constructors =-

		public Controller_3 () : base()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set the background color of the view to white
			this.View.BackgroundColor = UIColor.FromRGB (.5f, .5f, 1);
			
			lblMain = new UILabel (new RectangleF (20, 200, 280, 33));
			lblMain.Text = "Controller 3";
			lblMain.BackgroundColor = UIColor.Clear;
			this.View.AddSubview (lblMain);
		}
		
	}
}
