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

		ChatInputView chatInputView;
		UIButton SendButton {
			get {
				return chatInputView.SendButton;
			}
		}

		UITextView TextView {
			get {
				return chatInputView.TextView;
			}
		}

		NSIndexPath LastIndexPath {
			get {
				return NSIndexPath.FromRowSection (messages.Count - 1, 0);
			}
		}

		public ChatViewController (IntPtr handle)
			: base (handle)
		{
			messages = new List<Message> () {
				new Message{ Type = MessageType.Incoming, Text = "Hello!" },
				new Message{ Type = MessageType.Outgoing, Text = "Hi!" },
				new Message{ Type = MessageType.Incoming, Text = "Do you know about Xamarin?" },
				new Message{ Type = MessageType.Outgoing, Text = "Yes! Sure!" },
				new Message{ Type = MessageType.Incoming, Text = "And what do you think?" },
				new Message{ Type = MessageType.Outgoing, Text = "I think it is the best way to develop mobile applications." },
				new Message{ Type = MessageType.Incoming, Text = "Wow :-)" },
				new Message{ Type = MessageType.Outgoing, Text = "Yep. Check it out\nhttp://Xamarin.com" },
				new Message{ Type = MessageType.Incoming, Text = "Will do. Thanks" },
			};
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			hiddenInput = new DummyInput(this) {
				Hidden = true
			};
			View.AddSubview (hiddenInput);

			chatInputView = new ChatInputView ();

			SendButton.TouchUpInside += OnSendClicked;

			TextView.Changed += OnTextChanged;
			TextView.Layer.BorderColor = UIColor.FromRGB (200, 200, 205).CGColor;
			TextView.Layer.BorderWidth = (float)0.5;
			TextView.Layer.CornerRadius = 5;
			TextView.BackgroundColor = UIColor.FromWhiteAlpha (250, 1);

			chatSrc = new ChatSource (messages, ShowMenu);
			TableView.Source = chatSrc;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			UpdateToolbar ();
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

		void UpdateToolbar ()
		{
			chatInputView.TranslatesAutoresizingMaskIntoConstraints = false;
			Chat.AddSubview (chatInputView);

			var c1 = NSLayoutConstraint.FromVisualFormat ("H:|[chat]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"chat", chatInputView
			);
			var c2 = NSLayoutConstraint.FromVisualFormat ("V:|[chat]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"chat", chatInputView
			);
			Chat.AddConstraints (c1);
			Chat.AddConstraints (c2);
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

				// Move content with keyboard

				var oldOverlap = CalcContentOverlap();
				UpdateTableInsets ();
				var newOverlap = CalcContentOverlap();

				var offset = TableView.ContentOffset;
				offset.Y += newOverlap - oldOverlap;
				if(offset.Y < 0)
					offset.Y = 0;
				TableView.ContentOffset = offset;
			}, null);
		}

		nfloat CalcContentOverlap()
		{
			var onScreenHeight = CalcOnScreenContentHeight(); 	// >= 0

			// chat's input view with or without keyboard
			var obstacleHeight = TableView.ContentInset.Bottom; // >= 0

			var overlap = NMath.Max(onScreenHeight + obstacleHeight - TableView.Frame.Height, 0);
			return overlap;
		}

		nfloat CalcOnScreenContentHeight()
		{
			// Content which rendered on screen can't be bigger than table's height
			return NMath.Min (TableView.ContentSize.Height - TableView.ContentOffset.Y, TableView.Frame.Height);
		}

		// returns changes in ContentInsetY and ContentOffsetY values
		void UpdateTableInsets()
		{
			UIEdgeInsets oldInset = TableView.ContentInset;
			CGPoint oldOffset = TableView.ContentOffset;

			nfloat hiddenHeight = TableView.Frame.GetMaxY () - Chat.Frame.GetMinY();

			UIEdgeInsets newInset = oldInset;
			newInset.Bottom = hiddenHeight;

			TableView.ContentInset = newInset; // this may change ContentOffset property implicitly
			TableView.ScrollIndicatorInsets = newInset;
		}

		bool IsTableContentOverlapped()
		{
			return TableView.ContentSize.Height > TableView.Frame.Height - TableView.ContentInset.Bottom;
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

			var indexPaths = new NSIndexPath[] { LastIndexPath };
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