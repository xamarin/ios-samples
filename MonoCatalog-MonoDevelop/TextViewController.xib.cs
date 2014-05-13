//
// C# port of the textview sample
//
using Foundation;
using UIKit;

using System;

namespace MonoCatalog {
	
	public partial class TextViewController : UIViewController {
		UITextView textView;
		NSObject obs1, obs2;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Text View";
			textView = new UITextView (View.Frame){
				TextColor = UIColor.Black,
				Font = UIFont.FromName ("Arial", 18f),
				BackgroundColor = UIColor.White,
				Text = "This code brought to you by ECMA 334, ECMA 335\n\n\nEmbrace the CIL!",
				ReturnKeyType = UIReturnKeyType.Default,
				KeyboardType = UIKeyboardType.Default,
				ScrollEnabled = true,
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight,
			};
	
			// Provide our own save button to dismiss the keyboard
			textView.Started += delegate {
				var saveItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, delegate {
					textView.ResignFirstResponder ();
					NavigationItem.RightBarButtonItem = null;
					});
				NavigationItem.RightBarButtonItem = saveItem;
			};
			
			View.AddSubview (textView);
		}
	
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
	
			obs1 = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, delegate (NSNotification n){
				var kbdRect = UIKeyboard.BoundsFromNotification (n);
				var duration = UIKeyboard.AnimationDurationFromNotification (n);
				var frame = View.Frame;
				frame.Height -= kbdRect.Height;
				UIView.BeginAnimations ("ReCGSizeorKeyboard");
				UIView.SetAnimationDuration (duration);
				View.Frame = frame;
				UIView.CommitAnimations ();
				});
	
						obs2 = NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UIKeyboardWillHideNotification"), delegate (NSNotification n){
				var kbdRect = UIKeyboard.BoundsFromNotification (n);
				var duration = UIKeyboard.AnimationDurationFromNotification (n);
				var frame = View.Frame;
				frame.Height += kbdRect.Height;
				UIView.BeginAnimations ("ReCGSizeorKeyboard");
				UIView.SetAnimationDuration (duration);
				View.Frame = frame;
				UIView.CommitAnimations ();
			});
		}
	
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (obs1);
			NSNotificationCenter.DefaultCenter.RemoveObserver (obs2);
		}
	}
}
