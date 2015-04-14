using System;
using System.Drawing;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace Chat
{
	public partial class ChatViewController : UIViewController
	{
		NSObject willShowToken;
		NSObject willHideToken;
		NSObject willHideMenuToken;

		List<Message> messages;
		ChatSource chatSrc;

		// We need dummy input for keeping keyboard visible during showing menu
		DummyInput hiddenInput;

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

			hiddenInput = new DummyInput(this) {
				Hidden = true
			};
			View.AddSubview (hiddenInput);

			chatSrc = new ChatSource (messages, ShowMenu);
			SendButton.TouchUpInside += OnSendClicked;
			TextView.Changed += OnTextChanged;
			TableView.Source = chatSrc;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			willShowToken = UIKeyboard.Notifications.ObserveWillShow (KeyboardWillShowHandler);
			willHideToken = UIKeyboard.Notifications.ObserveWillHide (KeyboardWillHideHandler);
			willHideMenuToken = UIMenuController.Notifications.ObserveWillHideMenu (MenuWillHide);

			UpdateTableInsets ();
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
			UIView.Animate (e.AnimationDuration, 0, ConvertToAnimationOptions (e.AnimationCurve), () => {
				View.LayoutIfNeeded ();
				Tuple<nfloat, nfloat> changes = UpdateTableInsets ();

				// Move content with keyboard
				var offset = TableView.ContentOffset;
				offset.Y += changes.Item1 - changes.Item2;
				TableView.ContentOffset = offset;
			}, null);
		}

		// returns changes in ContentInsetY and ContentOffsetY values
		Tuple<nfloat, nfloat> UpdateTableInsets()
		{
			UIEdgeInsets oldInset = TableView.ContentInset;
			CGPoint oldOffset = TableView.ContentOffset;

			nfloat hiddenHeight = TableView.Frame.GetMaxY () - Chat.Frame.GetMinY();

			UIEdgeInsets newInset = oldInset;
			newInset.Bottom = hiddenHeight;

			TableView.ContentInset = newInset; // this may change ContentOffset property implicitly
			TableView.ScrollIndicatorInsets = newInset;

			return new Tuple<nfloat, nfloat> (newInset.Bottom - oldInset.Bottom, TableView.ContentOffset.Y - oldOffset.Y);
		}

		UIViewAnimationOptions ConvertToAnimationOptions(UIViewAnimationCurve curve)
		{
			// Looks like a hack. But it is correct.
			// UIViewAnimationCurve and UIViewAnimationOptions are shifted by 16 bits
			// http://stackoverflow.com/questions/18870447/how-to-use-the-default-ios7-uianimation-curve/18873820#18873820
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
			willHideMenuToken.Dispose ();
		}

		[Export("messageCopyTextAction:")]
		internal void CopyMessage(NSObject sender)
		{
			var selected = TableView.IndexPathForSelectedRow;
			Message msg = messages [selected.Row];
			UIPasteboard.General.String = msg.Text;
		}

		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}

		void ShowMenu(UIGestureRecognizer gesture)
		{
			CGPoint location = gesture.LocationInView (TableView);
			NSIndexPath indexPath = TableView.IndexPathForRowAtPoint (location);
			TableView.SelectRow (indexPath, false, UITableViewScrollPosition.None);

			if (TextView.IsFirstResponder) {
				hiddenInput.Text = string.Empty;
				hiddenInput.BecomeFirstResponder ();
			} else {
				hiddenInput.BecomeFirstResponder ();
				BecomeFirstResponder ();
			}

			UIMenuController menu = UIMenuController.SharedMenuController;
			menu.SetTargetRect (gesture.View.Frame, gesture.View.Superview);
			menu.MenuItems = new UIMenuItem[] {
				new UIMenuItem { Title = "Copy", Action = new Selector ("messageCopyTextAction:") }
			};
			menu.SetMenuVisible (true, true);
		}

		void MenuWillHide (object sender, NSNotificationEventArgs e)
		{
			var selected = TableView.IndexPathForSelectedRow;
			TableView.DeselectRow (selected, false);
		}
	}

	public class DummyInput : UITextView
	{
		readonly ChatViewController viewController;

		public DummyInput (ChatViewController vc)
		{
			viewController = vc;
		}

		public override bool CanPerform (ObjCRuntime.Selector action, NSObject withSender)
		{
			return action.Name == "messageCopyTextAction:";
		}

		[Export("messageCopyTextAction:")]
		void CopyMessage(NSObject sender)
		{
			viewController.CopyMessage (sender);
		}
	}
}