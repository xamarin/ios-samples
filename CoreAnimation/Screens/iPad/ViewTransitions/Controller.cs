using System;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Example_CoreAnimation.Screens.iPad.ViewTransitions
{
	public class Controller : UIViewController, IDetailView
	{
		protected UIToolbar toolbar;
		protected TransitionViewController controller1;
		protected BackTransitionViewController controller2;
		
		public Controller ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			toolbar = new UIToolbar (new RectangleF (0, 0, this.View.Frame.Width, 44));
			this.View.Add (toolbar);
			
			RectangleF mainFrame = new RectangleF (0, toolbar.Frame.Height, this.View.Frame.Width, this.View.Frame.Height - toolbar.Frame.Height);
			
			controller1 = new TransitionViewController ();
			controller1.View.Frame = mainFrame;
			controller2 = new BackTransitionViewController ();
			controller2.View.Frame = mainFrame;
			
			View.AddSubview (controller1.View);
			
			// controller2.View.Hidden = true;
			
			 controller1.TransitionClicked += (s, e) => {
				UIView.Animate (1, 0,  controller1.SelectedTransition, () => {
					controller1.View.RemoveFromSuperview ();
					View.AddSubview (controller2.View);
					// controller1.View.Hidden = false;
					// controller2.View.Hidden = true;
				}, null);
				UIView.BeginAnimations("ViewChange");
//				UIView.SetAnimationTransition(UIViewAnimationTransition.FlipFromLeft, this.View, true);
//				{
//					 controller1.View.RemoveFromSuperview();
//					this.View.AddSubview( controller2.View);
//				}
//				UIView.CommitAnimations();

			};
			
			 controller2.BackClicked += (s, e) => {
				UIView.Animate(.75, 0,  controller1.SelectedTransition, () => {
					controller2.View.RemoveFromSuperview ();
					View.AddSubview (controller1.View);
				}, null);
			};
		}
		
		public void AddContentsButton (UIBarButtonItem button)
		{
			button.Title = "Contents";
			 toolbar.SetItems(new UIBarButtonItem[] { button }, false );
		}
		
		public void RemoveContentsButton ()
		{
			 toolbar.SetItems(new UIBarButtonItem[0], false);
		}

	}
}

