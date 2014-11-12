using System;
using UIKit;

namespace Example_Drawing.Screens.iPad.HitTesting
{
	public class Controller : UIViewController
	{
		#region -= constructors =-

		public Controller () : base() { }

		#endregion

		public override void LoadView ()
		{
			Console.WriteLine ("LoadView() Called");
			base.LoadView ();
			
			View = new View ();
			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}
	}
}

