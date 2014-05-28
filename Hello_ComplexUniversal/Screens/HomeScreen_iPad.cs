using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Hello_ComplexUniversal.Screens
{
	public partial class HomeScreen_iPad : UIViewController
	{
		//loads the HomeScreen_iPad.xib file and connects it to this object
		public HomeScreen_iPad () : base ("HomeScreen_iPad", null)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.btnOne.TouchUpInside += (sender, e) => {
				this.lblOutput.Text = "Button 1 Clicked";
			};
			this.btnTwo.TouchUpInside += (sender, e) => {
				this.lblOutput.Text = "Button 2 Clicked";
			};
			this.btnThree.TouchUpInside += (sender, e) => {
				this.lblOutput.Text = "Button 3 Clicked";
			};
		}
		
	}
}
