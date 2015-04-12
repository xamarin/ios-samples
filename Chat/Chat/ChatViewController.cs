using System;
using System.Drawing;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;

namespace Chat
{
	public partial class ChatViewController : UIViewController
	{
		NSObject willShowToken;
		NSObject willHideToken;

		List<Message> messages;
		ChatSource chatSrc;

		public ChatViewController (IntPtr handle)
			: base (handle)
		{
			messages = new List<Message> ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SendButton.TouchUpInside += OnSendClicked;
			chatSrc = new ChatSource (messages);
			TableView.Source = chatSrc;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			willShowToken = UIKeyboard.Notifications.ObserveWillShow (KeyboardWillShowHandler);
			willHideToken = UIKeyboard.Notifications.ObserveWillHide (KeyboardWillHideHandler);
		}

		void KeyboardWillShowHandler (object sender, UIKeyboardEventArgs e)
		{
			UpdateButtomLayoutConstraint (e);
		}

		void KeyboardWillHideHandler (object sender, UIKeyboardEventArgs e)
		{
			UpdateButtomLayoutConstraint (e);
		}

		void UpdateButtomLayoutConstraint(UIKeyboardEventArgs e)
		{
			BottomConstraint.Constant = View.Bounds.GetMaxY () - e.FrameEnd.GetMinY ();

			UIViewAnimationCurve curve = e.AnimationCurve;
			UIView.Animate (e.AnimationDuration, 0, ConvertToAnimationOptions(e.AnimationCurve), ()=> {
				View.LayoutIfNeeded ();
			}, null);
		}

		UIViewAnimationOptions ConvertToAnimationOptions(UIViewAnimationCurve curve)
		{
			// Looks like a hack. But it is correct.
			// UIViewAnimationCurve and UIViewAnimationOptions are shifted by 16 bits
			return (UIViewAnimationOptions)((int)curve << 16);
		}

		void OnSendClicked (object sender, EventArgs e)
		{
			var text = TextView.Text.Trim ();
			TextView.Text = string.Empty;

			if (string.IsNullOrEmpty (text))
				return;

			var msg = new Message {
				Type = MessageType.Right,
				Text = text
			};

			messages.Add (msg);

			var indexPaths = new NSIndexPath[] {
				NSIndexPath.FromRowSection (messages.Count - 1, 0)
			};
			TableView.InsertRows(indexPaths, UITableViewRowAnimation.None);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			willShowToken.Dispose ();
			willHideToken.Dispose ();
		}
	}
}