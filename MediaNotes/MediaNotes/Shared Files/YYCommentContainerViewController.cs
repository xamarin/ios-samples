#define USE_AUTOLAYOUT
#define ENABLE_KEYBOARD_AVOIDANCE
#define ENABLE_LAYOUT_SUBVIEWS 
#define ENABLE_WILL_ROTATE_ADJUSTMENT
using System;
using CoreGraphics;
using UIKit;
using Foundation;

namespace MediaNotes
{
	[Register ("YYCommentContainerView")]
	public class YYCommentContainerView : UIView
	{
		public YYCommentContainerView (IntPtr ptr) : base (ptr)
		{
		
		}
		
		[Export ("requiresConstraintBasedLayout")]
		static bool Requires ()
		{
			return true;
		}
	}

	public class YYCommentContainerViewController : UIViewController
	{
		public YYCommentViewController commentViewController { get; set; }

		bool commentViewIsVisible;
		nfloat keyboardOverlap;
		UIView commentView;
		bool observersregistered;

		public UIViewController contentController { get; set; }

		public YYCommentContainerViewController (UIViewController child) : base ()
		{
			contentController = child;
			AddChildViewController (child);
			child.DidMoveToParentViewController (this);

			if (child.WantsFullScreenLayout) {
				WantsFullScreenLayout = true;
				UIApplication.SharedApplication.SetStatusBarStyle (UIStatusBarStyle.BlackTranslucent, false);
			}
			commentViewController = new YYCommentViewController ();
			commentViewController.associatedObject = (PhotoViewController)child;
		}

		public  YYCommentViewController  YYcommentViewController ()
		{

			YYCommentViewController controller = commentViewController;
			   
			if (contentController.Equals (controller.associatedObject))
				return controller;
			else
				return null;
		}
	
		public override string Title {
			get {
				return contentController.Title;
			}

		}

		public override void LoadView ()
		{
			CGRect rect = UIScreen.MainScreen.Bounds;
			View = new UIView (rect);
			commentView = commentViewController.View;
			commentView.Layer.CornerRadius = 8.0f;
			commentView.Alpha = 0.0f;
			contentController.View.Frame = rect;
			commentView.Bounds = new CGRect (0.0f, 0.0f, rect.Size.Width / 2.0f, rect.Size.Height / 4.0f);
			View.AddSubview (contentController.View);
			UILongPressGestureRecognizer gestureRecognizer = new UILongPressGestureRecognizer (this, new ObjCRuntime.Selector ("LongPressGesture:"));
			View.AddGestureRecognizer (gestureRecognizer);

# if USE_AUTOLAYOUT
			commentView.TranslatesAutoresizingMaskIntoConstraints = false;
# endif
		}

		public void AdjustCommentviewFrame ()
		{
			CGRect viewBounds = View.Bounds;
			nfloat height = viewBounds.Size.Height;
			nfloat width = viewBounds.Size.Width;
			CGRect rect = commentView.Frame;
			rect.Y = (height - commentView.Bounds.Size.Height);
			rect.X = (width - commentView.Bounds.Size.Width) / 2.0f;
			commentView.Frame = rect;
		}

		public void AdjustCommentViewYPosition (nfloat yOffset, nfloat duration, bool finished)
		{
			CGRect rect = commentView.Frame;
			rect.Y = rect.Y - yOffset;
			//Check
			UIView.Animate (duration, () => {
				commentView.Frame = rect;
			});

		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return contentController.ShouldAutorotate ();
		}

		public override bool ShouldAutomaticallyForwardAppearanceMethods {
			get {
				return false;
			}
		}

		[Export("LongPressGesture:")]
		void toggleCommentViewVisibility (UIGestureRecognizer gestureRecognizer)
		{
			bool began = (gestureRecognizer.State == UIGestureRecognizerState.Began);
			if (!began)
				return;
			if (commentViewIsVisible) {
				commentViewIsVisible = false;
				commentViewController.WillMoveToParentViewController (null);
				commentViewController.BeginAppearanceTransition (false, true);
				UIView.Animate (0.5f, () => {
					commentView.Alpha = 0.5f;
				}, () => { 
					commentView.RemoveFromSuperview ();
					commentViewController.EndAppearanceTransition ();
					commentViewController.RemoveFromParentViewController ();
				});
			} else {
				commentViewIsVisible = true;
				AddChildViewController (commentViewController);
				commentViewController.BeginAppearanceTransition (true, true);
				View.InsertSubviewAbove (commentView, contentController.View);

				# if (!USE_AUTOLAYOUT)
				AdjustCommentviewFrame();
				# else
				View.SetNeedsUpdateConstraints ();
				# endif
				UIView.Animate (.5, () => {
					commentView.Alpha = 0.5f;},
				() => {
					commentViewController.EndAppearanceTransition ();
					commentViewController.DidMoveToParentViewController (this);
				});
			}
		}
# if (!USE_AUTOLAYOUT && ENABLE_LAYOUT_SUBVIEWS)	

		public override void ViewWillLayoutSubviews ()
		{
			 if(commentViewIsVisible)
					AdjustCommentviewFrame();
		}
# else
		// If our content controller has been removed because of a memory warning we need to reinsert if we are appearing.
		public override void ViewWillLayoutSubviews ()
		{
			if (contentController.IsViewLoaded == false) {
				if (commentViewIsVisible) {
					View.InsertSubviewBelow (contentController.View, commentView);
				} else {
					View.AddSubview (contentController.View);
				}
			}
		}
# endif

# if ENABLE_WILL_ROTATE_ADJUSTMENT
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (commentViewIsVisible) {
				AdjustCommentviewFrame ();
			}
		}

# endif

# if USE_AUTOLAYOUT
		public override void UpdateViewConstraints ()
		{
			if (commentViewIsVisible) {
				NSLayoutConstraint constraint1 = NSLayoutConstraint.Create (commentView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
				                                                            View, NSLayoutAttribute.CenterX, 1.0f, 0.0f);
				NSLayoutConstraint constraint2 = NSLayoutConstraint.Create (commentView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,
				                                                            View, NSLayoutAttribute.Bottom, 1.0f, 0.0f);
				NSLayoutConstraint constraint3 = NSLayoutConstraint.Create (commentView, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
				                                                            View, NSLayoutAttribute.Width, 0.5f, 0.0f);
				NSLayoutConstraint constarint4 = NSLayoutConstraint.Create (commentView, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
				                                                            View, NSLayoutAttribute.Height, 0.25f, 0.0f);
				View.AddConstraints (new NSLayoutConstraint[] {
					constraint1,
					constraint2,
					constraint3,
					constarint4
				});
			}
			base.UpdateViewConstraints ();
		}

# endif
		public override void ViewWillAppear (bool animated)
		{
			contentController.BeginAppearanceTransition (true, true);
			if (commentViewIsVisible) {
				commentViewController.BeginAppearanceTransition (true, true);
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			contentController.BeginAppearanceTransition (false, false);
			if (commentViewIsVisible) {
				commentViewController.BeginAppearanceTransition (false, false);
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			contentController.EndAppearanceTransition ();
			if (commentViewIsVisible) {
				commentViewController.EndAppearanceTransition ();
			}

# if ENABLE_KEYBOARD_AVOIDANCE
			if (observersregistered == false) {
				NSNotificationCenter.DefaultCenter.AddObserver (this, new ObjCRuntime.Selector ("KeyBoardWillShow"), UIKeyboard.WillShowNotification, null);
				NSNotificationCenter.DefaultCenter.AddObserver (this, new ObjCRuntime.Selector ("KeyBoardWillHide"), UIKeyboard.WillHideNotification, null);
			} 		
#  endif
		}

		public override void ViewDidDisappear (bool animated)
		{
			contentController.EndAppearanceTransition ();
			if (commentViewIsVisible) {
				commentViewController.EndAppearanceTransition ();
			}

# if ENABLE_KEYBOARD_AVOIDANCE
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
			observersregistered = false;
# endif
		}


# if ENABLE_KEYBOARD_AVOIDANCE

		[Export("KeyBoardWillShow:")]
		public void KeyboardWillShow (NSNotification notification)
		{
			//Gather some info about the keyboard and its information
			CGRect keyboardEndFrame = CGRect.Empty;
			float animationDuration = 0.0f;
			notification.UserInfo.ObjectForKey (UIKeyboard.FrameEndUserInfoKey);
			//GetValues
			notification.UserInfo.ObjectForKey (UIKeyboard.AnimationDurationUserInfoKey);
			//GetValues
			keyboardEndFrame = View.ConvertRectFromView (keyboardEndFrame, null);
			keyboardOverlap = keyboardEndFrame.Size.Height;
			AdjustCommentViewYPosition (keyboardOverlap, animationDuration, false);
		}

		[Export("KeyBoardWillHide:")]
		public void KeyBoardWillHide (NSNotification notification)
		{
			//Gather some info about the keyboard and its information
			float animationDuration = 0.0f;
			notification.UserInfo.ObjectForKey (UIKeyboard.AnimationDurationUserInfoKey);
			AdjustCommentViewYPosition (-1.0f * keyboardOverlap, animationDuration, false);
			keyboardOverlap = 0;
		}
# endif
	}
}