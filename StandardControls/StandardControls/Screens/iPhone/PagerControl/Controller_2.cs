using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Example_StandardControls.Controls;

namespace Example_StandardControls.Screens.iPhone.PagerControl
{
	public class Controller_2 : UIViewController
	{
		UILabel lblMain;


		#region -= constructors =-

		public Controller_2 () : base()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set the background color of the view to white
			this.View.BackgroundColor = UIColor.LightGray;
			
			lblMain = new UILabel (new RectangleF (20, 200, 280, 33));
			lblMain.Text = "Controller 2";
			lblMain.BackgroundColor = UIColor.Clear;
			this.View.AddSubview (lblMain);
		}
		
	}
}
