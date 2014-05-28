using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Hello_ComplexUniversal.Screens
{
	public partial class HomeScreen_iPhone : UIViewController
	{
		//loads the HomeScreen_iPhone.xib file and connects it to this object
		public HomeScreen_iPhone () : base ("HomeScreen_iPhone", null)
		{
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.btnOne.TouchUpInside += (sender, e) => {
				this.lblOutput.Text = "Button 1 Clicked";
			};

		}
		
	}
}
