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
			messages = new List<Message> () {
				new Message{ Type = MessageType.Incoming, Text = "You're encountering the side effect of a fantastic new feature in iOS8's Tableviews: Automatic Row Heights" },
				new Message{ Type = MessageType.Outgoing, Text = "You're encountering the side effect of a fantastic new feature in iOS8's Tableviews: Automatic Row Heights" },
				new Message{ Type = MessageType.Incoming, Text = "Hi!" },
				new Message{ Type = MessageType.Incoming, Text = "1" },
				new Message{ Type = MessageType.Outgoing, Text = "2" },
				new Message{ Type = MessageType.Incoming, Text = "3" },
				new Message{ Type = MessageType.Incoming, Text = "4" },
				new Message{ Type = MessageType.Incoming, Text = "5" },
				new Message{ Type = MessageType.Incoming, Text = "6" },
				new Message{ Type = MessageType.Incoming, Text = "7" },
				new Message{ Type = MessageType.Incoming, Text = "8" },
				new Message{ Type = MessageType.Incoming, Text = "9" },
				new Message{ Type = MessageType.Incoming, Text = "10" },
				new Message{ Type = MessageType.Incoming, Text = "11" },
				new Message{ Type = MessageType.Incoming, Text = "12" },
				new Message{ Type = MessageType.Incoming, Text = "13" },
				new Message{ Type = MessageType.Incoming, Text = "14" },
				new Message{ Type = MessageType.Incoming, Text = "15" },
			};
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			chatSrc = new ChatSource (messages);
			SendButton.TouchUpInside += OnSendClicked;
			TextView.Changed += OnTextChanged;
			TableView.Source = chatSrc;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			willShowToken = UIKeyboard.Notifications.ObserveWillShow (KeyboardWillShowHandler);
			willHideToken = UIKeyboard.Notifications.ObserveWillHide (KeyboardWillHideHandler);

			UpdateButtonState ();
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
			var text = TextView.Text;
			TextView.Text = string.Empty; // this will not generate change text event
			UpdateButtonState ();

			if (string.IsNullOrWhiteSpace (text))
				return;

			var msg = new Message {
				Type = MessageType.Outgoing,
				Text = text.Trim ()
			};

			messages.Add (msg);

			var indexPaths = new NSIndexPath[] {
				NSIndexPath.FromRowSection (messages.Count - 1, 0)
			};
			TableView.InsertRows(indexPaths, UITableViewRowAnimation.None);
			TableView.ScrollToRow (indexPaths [0], UITableViewScrollPosition.Bottom, true);
		}

		void OnTextChanged (object sender, EventArgs e)
		{
			UpdateButtonState ();
		}

		void UpdateButtonState()
		{
			SendButton.Enabled = !string.IsNullOrWhiteSpace (TextView.Text);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			willShowToken.Dispose ();
			willHideToken.Dispose ();
		}
	}
}