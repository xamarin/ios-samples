using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace TicTacToe
{
	public class TTTNewMessageViewController : UIViewController
	{
		public TTTProfile Profile;
		UITextView messageTextView;
		UIWindow currentMessageWindow;
		UIWindow currentMessageSourceWindow;

		public TTTNewMessageViewController ()
		{
			Title = "New Message";
			View.TintAdjustmentMode = UIViewTintAdjustmentMode.Normal;
		}

		public override void LoadView ()
		{
			UIView baseView = new UIView ();
			baseView.BackgroundColor = UIColor.FromWhiteAlpha (0f, .15f);

			UIView view = new UIView (new RectangleF (-100f, -50f, 240f, 120f)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin |
				UIViewAutoresizing.FlexibleBottomMargin |
				UIViewAutoresizing.FlexibleTopMargin |
				UIViewAutoresizing.FlexibleRightMargin,
				BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("barBackground"))
			};
			baseView.AddSubview (view);

			UIButton cancelButton = UIButton.FromType (UIButtonType.System);
			cancelButton.TouchUpInside += close;
			cancelButton.TranslatesAutoresizingMaskIntoConstraints = false;
			cancelButton.SetTitle ("Cancel", UIControlState.Normal);
			view.AddSubview (cancelButton);

			UIButton postButton = UIButton.FromType (UIButtonType.System);
			postButton.TouchUpInside += post;
			postButton.TranslatesAutoresizingMaskIntoConstraints = false;
			postButton.SetTitle ("Post", UIControlState.Normal);
			view.AddSubview (postButton);

			messageTextView = new UITextView () {
				BackgroundColor = UIColor.Clear,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			view.AddSubview (messageTextView);

			NSDictionary views = NSDictionary.FromObjectsAndKeys (
				new NSObject[] { postButton, cancelButton, messageTextView },
				new NSString[] { new NSString ("postButton"),
				new NSString ("cancelButton"),
				new NSString ("messageTextView")
			}
			);

			baseView.AddConstraints (NSLayoutConstraint.FromVisualFormat (
				"|-8-[messageTextView]-8-|", (NSLayoutFormatOptions)0, null, views)
			);
			baseView.AddConstraints (NSLayoutConstraint.FromVisualFormat (
				"|-8-[cancelButton]->=20-[postButton]-8-|", (NSLayoutFormatOptions)0, null, views)
			);
			baseView.AddConstraints (NSLayoutConstraint.FromVisualFormat (
				"V:|-8-[messageTextView]-[cancelButton]-8-|", (NSLayoutFormatOptions)0, null, views)
			);
			baseView.AddConstraints (NSLayoutConstraint.FromVisualFormat (
				"V:|-8-[messageTextView]-[postButton]-8-|", (NSLayoutFormatOptions)0, null, views)
			);

			View = baseView;
		}

		public void PresentFromViewController (UIViewController controller)
		{
			UIView sourceView = controller.View;
			currentMessageSourceWindow = sourceView.Window;

			currentMessageWindow = new UIWindow (currentMessageSourceWindow.Frame) {
				TintColor = currentMessageSourceWindow.TintColor,
				RootViewController = this
			};
			currentMessageWindow.MakeKeyAndVisible ();
			messageTextView.BecomeFirstResponder ();
			View.Alpha = 0f;

			UIView.Animate (0.3f, delegate {
				View.Alpha = 1f;
				currentMessageSourceWindow.TintAdjustmentMode = UIViewTintAdjustmentMode.Dimmed;
			});
		}

		void close (object sender, EventArgs e)
		{
			UIView.Animate (0.3f, delegate {
				View.Alpha = 0f;
				currentMessageSourceWindow.TintAdjustmentMode = UIViewTintAdjustmentMode.Automatic;
			}, 
			                delegate {
				currentMessageWindow = null;
			});
		}

		void post (object sender, EventArgs e)
		{
			TTTMessage message = new TTTMessage (){
				Icon = Profile.Icon,
				Text = messageTextView.Text
			};
			TTTMessageServer.SharedMessageServer.AddMessage (message);

			close (null, EventArgs.Empty);
		}
	}
}

