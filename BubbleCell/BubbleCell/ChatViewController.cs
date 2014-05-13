//
// The Chat View Controller displays both the conversation
// using the BubbleElement as well as the actual text region
// where the user enters the text to be sent.
//
// The code to automatically grow the text that is being entered
// comes from AcaniChat, licensed under the MIT X11 license
// from https://github.com/acani/AcaniChat
// 
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Dialog;
using Foundation;
using UIKit;
using CoreGraphics;

namespace BubbleCell
{
	[Register ("ChatViewController")]
	public class ChatViewController : UIViewController {
		DialogViewController discussion;
		RootElement root;
		UIView discussionHost;
		UITextView entry;
		UIImageView chatBar;
		UIButton sendButton;
		
		const float messageFontSize = 16;
		const float maxContentHeight = 84;
		const int entryHeight = 40;
		nfloat previousContentHeight;
		
		NSObject showObserver, hideObserver;
		
		public ChatViewController (RootElement root)
		{
			this.root = root;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			NavigationController.NavigationBar.Translucent = false;
			var bounds = View.Bounds;
			
			var backgroundColor =  new UIColor (0.859f, 0.866f, 0.929f, 1);
			
			View.BackgroundColor = backgroundColor;
			//
			// Add the bubble chat interface
			//
			discussionHost = new UIView (new CGRect (bounds.X, bounds.Y, bounds.Width, bounds.Height-entryHeight)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				AutosizesSubviews = true,
				UserInteractionEnabled = true
			};
			View.AddSubview (discussionHost);

			discussion = new DialogViewController (UITableViewStyle.Plain, root, true);	
			discussionHost.AddSubview (discussion.View);
			discussion.View.BackgroundColor = backgroundColor;
			
			// 
			// Add styled entry
			//
			chatBar = new UIImageView (new CGRect (0, bounds.Height-entryHeight, bounds.Width, entryHeight)) {
				ClearsContextBeforeDrawing = false,
				AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth,
				Image = UIImage.FromFile ("ChatBar.png").StretchableImage (18, 20),
				UserInteractionEnabled = true
			};
			View.AddSubview (chatBar);
			
			entry = new UITextView (new CGRect (10, 9, 234, 22)) {
				ContentSize = new CGSize (234, 22),
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				ScrollEnabled = true,
				ScrollIndicatorInsets = new UIEdgeInsets (5, 0, 4, -2),
				ClearsContextBeforeDrawing = false,
				Font = UIFont.SystemFontOfSize (messageFontSize),
				DataDetectorTypes = UIDataDetectorType.All,
				BackgroundColor = UIColor.Clear,
			};
			
			// Fix a scrolling glitch
			entry.ShouldChangeText = (textView, range, text) => {
				entry.ContentInset = new UIEdgeInsets (0, 0, 3, 0);
				return true;
			};
			previousContentHeight = entry.ContentSize.Height;
			chatBar.AddSubview (entry);
			
			// 
			// The send button
			//
			sendButton = UIButton.FromType (UIButtonType.Custom);
			sendButton.ClearsContextBeforeDrawing = false;
			sendButton.Frame = new CGRect (chatBar.Frame.Width - 70, 8, 64, 26);
			sendButton.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;
			
			var sendBackground = UIImage.FromFile ("SendButton.png");
			sendButton.SetBackgroundImage (sendBackground, UIControlState.Normal);
			sendButton.SetBackgroundImage (sendBackground, UIControlState.Disabled);
			sendButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize (16);
			sendButton.TitleLabel.ShadowOffset = new CGSize (0, -1);
			sendButton.SetTitle ("Send", UIControlState.Normal);
			sendButton.SetTitleShadowColor (new UIColor (0.325f, 0.463f, 0.675f, 1), UIControlState.Normal);
			sendButton.AddTarget (SendMessage, UIControlEvent.TouchUpInside);
			DisableSend ();
			chatBar.AddSubview (sendButton);
			                     
			//
			// Listen to keyboard notifications to animate
			//
			showObserver = UIKeyboard.Notifications.ObserveWillShow (PlaceKeyboard);
			hideObserver = UIKeyboard.Notifications.ObserveWillHide (PlaceKeyboard);
			
			ScrollToBottom (false);
			// Track changes in the entry to resize the view accordingly
			entry.Changed += HandleEntryChanged;
		}
		
		public override void ViewDidUnload ()
		{
			showObserver.Dispose ();
			hideObserver.Dispose ();
			discussion = null;
			discussionHost = null;
			root = null;
			entry = null;
			chatBar = null;

			base.ViewDidUnload ();
		}
		
		void EnableSend ()
		{
			sendButton.Enabled = true;
			sendButton.TitleLabel.Alpha = 1;
		}
		
		void DisableSend ()
		{
			sendButton.Enabled = false;
			sendButton.TitleLabel.Alpha = 0.5f;
		}
		
		bool side;
		
		// Just show messages alternating
		void SendMessage (object sender, EventArgs args)
		{
			side = !side;
			discussion.Root [0].Add (new ChatBubble (side, entry.Text));
			entry.Text = "";
			ScrollToBottom (true);
		}
		
		//
		// Resizes the UITextView based on the current text
		//
		void HandleEntryChanged (object sender, EventArgs e)
		{
			var contentHeight = entry.ContentSize.Height - messageFontSize + 2;
			if (entry.HasText){
				if (contentHeight != previousContentHeight){
					if (contentHeight <= maxContentHeight){
						SetChatBarHeight (contentHeight + 18);
						if (previousContentHeight > maxContentHeight)
							entry.ScrollEnabled = false;
						entry.ContentOffset = new CGPoint (0, 6);
					} else if (previousContentHeight <= maxContentHeight){
						entry.ScrollEnabled = true;
						entry.ContentOffset = new CGPoint (0, contentHeight-68);
						if (previousContentHeight < maxContentHeight){
							ExpandChatBarHeight ();
						}
					}
				}
			} else {
				if (previousContentHeight > 22){
					ResetChatBarHeight ();
					if (previousContentHeight > maxContentHeight)
						entry.ScrollEnabled = false;
				}
				AdjustEntry ();
			}
			if (entry.Text != "") 
				EnableSend ();
			else
				DisableSend ();
			
			previousContentHeight = contentHeight;
			
		}
		
		// Resizes the chat bar to the specified height
		void SetChatBarHeight (nfloat height)
		{
			var chatFrame = discussion.View.Frame;
			chatFrame.Height = View.Frame.Height-height;
			discussion.View.Frame = chatFrame;
			
			UIView.BeginAnimations ("");
			UIView.SetAnimationDuration (.3);
			discussion.View.Frame = chatFrame;
			chatBar.Frame = new CGRect (chatBar.Frame.X, chatFrame.Height, chatFrame.Width, height);
			UIView.CommitAnimations ();
		}
		
		// Sets the default height
		void ResetChatBarHeight ()
		{
			SetChatBarHeight (entryHeight);
		}
		
		// Sets the maximum height
		void ExpandChatBarHeight ()
		{
			SetChatBarHeight (94);
		}
		
		// Adjusts the UITextView after an update
		void AdjustEntry ()
		{
			// This fixes a rendering glitch
			entry.ContentOffset = new CGPoint (0, 6);
		}
		
		// 
		// Custom layout: when our discussionHost changes, so does the discussion's view
		//
		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			discussion.View.Frame = discussionHost.Frame;
		}
		
		// 
		// When the keyboard appears, animate the new position for the entry
		// and scroll the chat to the bottom
		//
		void PlaceKeyboard (object sender, UIKeyboardEventArgs args)
		{
			UIView.BeginAnimations (""); {
				UIView.SetAnimationCurve (args.AnimationCurve);
				UIView.SetAnimationDuration (args.AnimationDuration);
				var viewFrame = View.Frame;
				var endRelative = View.ConvertRectFromView (args.FrameEnd, null);
				viewFrame.Height = endRelative.Y;
				View.Frame = viewFrame;
			} UIView.CommitAnimations ();
			
			ScrollToBottom (true);
			AdjustEntry ();
		}
		
		void ScrollToBottom (bool animated)
		{
			int row = discussion.Root [0].Elements.Count-1;
			if (row == -1)
				return;
			discussion.TableView.ScrollToRow (NSIndexPath.FromRowSection (row, 0), UITableViewScrollPosition.Bottom, true);
		}
		
		public override bool AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers {
			get {
				return true;
			}
		}
	}
}