using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_CoreAnimation.Screens.iPad.ViewTransitions
{
	public partial class TransitionViewController : UIViewController
	{
		public Action ContentsClicked;
		public event EventHandler<EventArgs> TransitionClicked;

		public UIViewAnimationOptions SelectedTransition {
			get {
				switch (sgmntTransitionType.SelectedSegment) {
				case 0:
					return UIViewAnimationOptions.TransitionCurlDown;
					break;
				case 1:
					return UIViewAnimationOptions.TransitionCurlUp;
					break;
				case 2:
					return UIViewAnimationOptions.TransitionFlipFromLeft;
					break;
				case 3:
					return UIViewAnimationOptions.TransitionFlipFromRight;
					break;
				default:
					return UIViewAnimationOptions.TransitionCurlDown;
					break;
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetToolbarVisibility (InterfaceOrientation);
			btnContents.TouchUpInside += (sender, e) => {
				if (ContentsClicked != null)
					ContentsClicked ();
			};

			btnTransition.TouchUpInside += (s, e) => {
				if (TransitionClicked != null)
					TransitionClicked (this, e);
			};
		}

		public void SetToolbarVisibility (UIInterfaceOrientation interfaceOrientation)
		{
			toolbar.Hidden = interfaceOrientation == UIInterfaceOrientation.LandscapeLeft ||
			                 interfaceOrientation == UIInterfaceOrientation.LandscapeRight;
		}
	}
}

