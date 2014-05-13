//
// Port of the transition sample
//
using Foundation;
using UIKit;
using System;
using CoreGraphics;

namespace MonoCatalog {
	
	public partial class TransitionViewController : UIViewController {
	
		public TransitionViewController () : base ("TransitionViewController", null) {}
		
		const float kImageHeight = 200f;
		const float kImageWidth = 250f;
		const float kTransitionDuration = 0.75f;
		const float kTopPlacement = 80f;
	
		UIImageView mainView, flipToView;
		UIView containerView;
		
		public override void ViewDidLoad ()
		{
			Title = "Transition";
	
			containerView = new UIView (new CGRect ((View.Bounds.Width - kImageWidth)/2.0f, kTopPlacement, kImageWidth, kImageHeight));
			View.AddSubview (containerView);
	
			mainView = new UIImageView (new CGRect (0, 0, kImageWidth, kImageHeight)){
				Image = UIImage.FromFile ("images/scene1.jpg")
			};					
			containerView.AddSubview (mainView);
	
			flipToView = new UIImageView (new CGRect (0, 0, kImageWidth, kImageHeight)){
				Image = UIImage.FromFile ("images/scene2.jpg")				
			};
		}
	
		partial void flipAction (UIBarButtonItem sender)
		{
			UIView.BeginAnimations (null, IntPtr.Zero);
			UIView.SetAnimationDuration (kTransitionDuration);
			UIView.SetAnimationTransition (
				mainView.Superview != null ? UIViewAnimationTransition.FlipFromLeft : UIViewAnimationTransition.FlipFromRight,
				containerView, true);
	
			if (flipToView.Superview == null){
				mainView.RemoveFromSuperview ();
				containerView.AddSubview (flipToView);
			} else {
				flipToView.RemoveFromSuperview ();
				containerView.AddSubview (mainView);
			}
			UIView.CommitAnimations ();
		}
		
		partial void curlAction (UIBarButtonItem sender)
		{
			UIView.BeginAnimations (null, IntPtr.Zero);
			UIView.SetAnimationDuration (kTransitionDuration);
			UIView.SetAnimationTransition (
				mainView.Superview != null ? UIViewAnimationTransition.CurlUp : UIViewAnimationTransition.CurlDown,
				containerView, true);
	
			if (flipToView.Superview == null){
				mainView.RemoveFromSuperview ();
				containerView.AddSubview (flipToView);
			} else {
				flipToView.RemoveFromSuperview ();
				containerView.AddSubview (mainView);
			}
			UIView.CommitAnimations ();
		}
	
		public override void ViewWillDisappear (bool animated)
		{
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
		}
	
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;
		}
	}
}
