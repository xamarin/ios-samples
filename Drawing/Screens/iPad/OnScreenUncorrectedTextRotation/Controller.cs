using System;
using UIKit;

namespace Example_Drawing.Screens.iPad.OnScreenUncorrectedTextRotation
{
	public class Controller : UIViewController
	{

		#region -= constructors =-

		public Controller () : base() { }

		#endregion
		
		public override void LoadView ()
		{
			View = new View ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set the background color of the view to white
			View.BackgroundColor = UIColor.White;			
		}		
	}
}

