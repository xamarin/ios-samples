
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace CoreAnimationExample
{
	public partial class BackTransitionViewController : UIViewController
	{
		public event EventHandler<EventArgs> BackClicked;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			btnBack.TouchUpInside += (s, e) => {
				if(BackClicked != null)
					BackClicked(this, e);
			};
		}
	}
}

