using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Example_CoreAnimation.Screens.iPad.BasicUIViewAnimation
{
	public partial class BasicUIViewAnimationScreen : UIViewController, IDetailView
	{
		public event EventHandler ContentsButtonClicked;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnContents.TouchUpInside += (sender, e) => {
				if (ContentsButtonClicked != null) {
					ContentsButtonClicked (sender, e);
				}
			};

			btnClickMe.TouchUpInside += (s, e) => {
				UIView.Animate (0.2, () => {
					// move the image one way or the other
					if (imgToAnimate.Frame.Y == 73) {
						imgToAnimate.Frame = new CGRect (
							imgToAnimate.Frame.X, imgToAnimate.Frame.Y + 400,
							imgToAnimate.Frame.Size.Width, imgToAnimate.Frame.Size.Height);
					} else {
						imgToAnimate.Frame = new CGRect (
							imgToAnimate.Frame.X, imgToAnimate.Frame.Y - 400,
							imgToAnimate.Frame.Size.Width, imgToAnimate.Frame.Size.Height);
					}					
				});
			};
		}
	}
}

