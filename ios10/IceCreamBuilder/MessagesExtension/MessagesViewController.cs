using System;

using Messages;
using Foundation;
using UIKit;

namespace MessagesExtension {
	public partial class MessagesViewController : MSMessagesAppViewController, IIceCreamsViewControllerDelegate, IBuildIceCreamViewControllerDelegate {
		public MessagesViewController (IntPtr handle) : base (handle)
		{
		}

		public override void WillBecomeActive (MSConversation conversation)
		{
			base.WillBecomeActive (conversation);

			// Present the view controller appropriate for the conversation and presentation style.
			PresentViewController (conversation, PresentationStyle);
		}

		public override void WillTransition (MSMessagesAppPresentationStyle presentationStyle)
		{
			var conversation = ActiveConversation;
			if (conversation == null)
				throw new Exception ("Expected an active converstation");

			// Present the view controller appropriate for the conversation and presentation style.
			PresentViewController (conversation, presentationStyle);
		}

		void PresentViewController (MSConversation conversation, MSMessagesAppPresentationStyle presentationStyle)
		{
			// Determine the controller to present.
			UIViewController controller;

			if (presentationStyle == MSMessagesAppPresentationStyle.Compact) {
				// Show a list of previously created ice creams.
				controller = InstantiateIceCreamsController ();
			} else {
				var iceCream = new IceCream (conversation.SelectedMessage);
				controller = iceCream.IsComplete ? InstantiateCompletedIceCreamController (iceCream) : InstantiateBuildIceCreamController (iceCream);
			}

			foreach (var child in ChildViewControllers) {
				child.WillMoveToParentViewController (null);
				child.View.RemoveFromSuperview ();
				child.RemoveFromParentViewController ();
			}

			AddChildViewController (controller);
			controller.View.Frame = View.Bounds;
			controller.View.TranslatesAutoresizingMaskIntoConstraints = false;
			View.AddSubview (controller.View);

			controller.View.LeftAnchor.ConstraintEqualTo (View.LeftAnchor).Active = true;
			controller.View.RightAnchor.ConstraintEqualTo (View.RightAnchor).Active = true;
			controller.View.TopAnchor.ConstraintEqualTo (View.TopAnchor).Active = true;
			controller.View.BottomAnchor.ConstraintEqualTo (View.BottomAnchor).Active = true;

			controller.DidMoveToParentViewController (this);
		}

		UIViewController InstantiateIceCreamsController ()
		{
			// Instantiate a `IceCreamsViewController` and present it.
			var controller = Storyboard.InstantiateViewController (IceCreamsViewController.StoryboardIdentifier) as IceCreamsViewController;
			if (controller == null)
				throw new Exception ("Unable to instantiate an IceCreamsViewController from the storyboard");

			controller.Builder = this;
			return controller;
		}

		UIViewController InstantiateBuildIceCreamController (IceCream iceCream)
		{
			// Instantiate a `BuildIceCreamViewController` and present it.
			var controller = Storyboard.InstantiateViewController (BuildIceCreamViewController.StoryboardIdentifier) as BuildIceCreamViewController;
			if (controller == null)
				throw new Exception ("Unable to instantiate an BuildIceCreamViewController from the storyboard");

			controller.IceCream = iceCream;
			controller.Builder = this;

			return controller;
		}

		public UIViewController InstantiateCompletedIceCreamController (IceCream iceCream)
		{
			// Instantiate a `BuildIceCreamViewController` and present it.
			var controller = Storyboard.InstantiateViewController (CompletedIceCreamViewController.StoryboardIdentifier) as CompletedIceCreamViewController;
			if (controller == null)
				throw new Exception ("Unable to instantiate an CompletedIceCreamViewController from the storyboard");

			controller.IceCream = iceCream;
			return controller;
		}

		public void DidSelectAdd (IceCreamsViewController controller)
		{
			Request (MSMessagesAppPresentationStyle.Expanded);
		}

		public void Build (BuildIceCreamViewController controller, IceCreamPart iceCreamPart)
		{
			var conversation = ActiveConversation;
			if (conversation == null)
				throw new Exception ("Expected a conversation");

			var iceCream = controller.IceCream;
			if (iceCream == null)
				throw new Exception ("Expected the controller to be displaying an ice cream");

			string messageCaption = string.Empty;
			var b = iceCreamPart as Base;
			var s = iceCreamPart as Scoops;
			var t = iceCreamPart as Topping;

			if (b != null) {
				iceCream.Base = b;
				messageCaption = "Let's build an ice cream";
			} else if (s != null) {
				iceCream.Scoops = s;
				messageCaption = "I added some scoops";
			} else if (t != null) {
				iceCream.Topping = t;
				messageCaption = "Our finished ice cream";
			} else {
				throw new Exception ("Unexpected type of ice cream part selected.");
			}

			// Create a new message with the same session as any currently selected message.
			var message = ComposeMessage (iceCream, messageCaption, conversation.SelectedMessage?.Session);

			// Add the message to the conversation.
			conversation.InsertMessage (message, null);

			// If the ice cream is complete, save it in the history.
			if (iceCream.IsComplete) {
				var history = IceCreamHistory.Load ();
				history.Append (iceCream);
				history.Save ();
			}

			Dismiss ();
		}

		MSMessage ComposeMessage (IceCream iceCream, string caption, MSSession session = null)
		{
			var components = new NSUrlComponents {
				QueryItems = iceCream.QueryItems
			};

			var layout = new MSMessageTemplateLayout {
				Image = iceCream.RenderSticker (true),
				Caption = caption
			};

			var message = new MSMessage (session ?? new MSSession ()) {
				Url = components.Url,
				Layout = layout
			};

			return message;
		}
	}
}
