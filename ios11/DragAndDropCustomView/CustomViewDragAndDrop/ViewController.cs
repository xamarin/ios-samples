using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using MobileCoreServices;
using UIKit;

namespace CustomViewDragAndDrop {
	public partial class ViewController : UIViewController, IUIDragInteractionDelegate, IUIDropInteractionDelegate {
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ImageView.Layer.BorderColor = UIColor.Green.CGColor;
			ImageView.Layer.BorderWidth = 0.0f;
			View.Layer.BorderColor = UIColor.Red.CGColor;

			// 'UserInteractionEnabled' is needed for drag and drop.
			ImageView.UserInteractionEnabled = true;

			// Allow dragging.
			var dragInteraction = new UIDragInteraction (this);
			ImageView.AddInteraction (dragInteraction);

			// Allow dropping.
			var dropInteraction = new UIDropInteraction (this);
			View.AddInteraction (dropInteraction);
		}

		#region IUIDragInteractionDelegate

		/// <summary>
		/// 'GetItemsForBeginningSession' allows dragging from a view.
		/// </summary>
		public UIDragItem [] GetItemsForBeginningSession (UIDragInteraction interaction, IUIDragSession session)
		{
			var image = ImageView.Image;
			if (image == null)
				return new UIDragItem [] { };

			var provider = new NSItemProvider (image);
			var item = new UIDragItem (provider) {
				LocalObject = image
			};

			// If a non empty array is returned, dragging is enabled.
			return new UIDragItem [] { item };
		}

		/// <summary>
		/// Customize the lift animation (optional).
		/// </summary>
		[Export ("dragInteraction:previewForLiftingItem:session:")]
		public UITargetedDragPreview GetPreviewForLiftingItem (UIDragInteraction interaction, UIDragItem item, IUIDragSession session)
		{
			var image = item.LocalObject as UIImage;
			if (image == null)
				return null;

			CGRect frame;
			if (image.Size.Width > image.Size.Height) {
				var multiplier = ImageView.Frame.Width / image.Size.Width;
				frame = new CGRect (0, 0, ImageView.Frame.Width, ImageView.Frame.Height * multiplier);
			} else {
				var multiplier = ImageView.Frame.Height / image.Size.Height;
				frame = new CGRect (0, 0, ImageView.Frame.Width * multiplier, ImageView.Frame.Height);
			}

			var previewImageView = new UIImageView (image) {
				ContentMode = UIViewContentMode.ScaleAspectFit,
				Frame = frame
			};

			var target = new UIDragPreviewTarget (ImageView, ImageView.Center);
			return new UITargetedDragPreview (previewImageView, new UIDragPreviewParameters (), target);
		}

		#endregion

		#region IUIDropInteractionDelegate

		[Export ("dropInteraction:canHandleSession:")]
		public bool CanHandleSession (UIDropInteraction interaction, IUIDropSession session)
		{
			return session.HasConformingItems (new [] { UTType.Image.ToString () }) && session.Items.Length == 1;
		}

		[Export ("dropInteraction:sessionDidEnter:")]
		public void SessionDidEnter (UIDropInteraction interaction, IUIDropSession session)
		{
			var dropLocation = session.LocationInView (View);
			UpdateLayers (dropLocation);
		}

		/// <summary>
		/// Specify how to handle the dropped items.
		/// </summary>
		[Export ("dropInteraction:sessionDidUpdate:")]
		public UIDropProposal SessionDidUpdate (UIDropInteraction interaction, IUIDropSession session)
		{
			var dropLocation = session.LocationInView (View);
			UpdateLayers (dropLocation);

			var operation = new UIDropOperation ();

			if (ImageView.Frame.Contains (dropLocation)) {
				operation = session.LocalDragSession == null ? UIDropOperation.Copy : UIDropOperation.Move;
			} else {
				// Cancel dropping if it's not inside the image view.
				operation = UIDropOperation.Cancel;
			}

			return new UIDropProposal (operation);
		}

		/// <summary>
		/// Access the drag item object when drop is performed.
		/// </summary>
		[Export ("dropInteraction:performDrop:")]
		public void PerformDrop (UIDropInteraction interaction, IUIDropSession session)
		{
			// Get the drag items (UIImage in this case).
			session.LoadObjects<UIImage> (images => {
				ImageView.Image = images?.First ();
			});

			var dropLocation = session.LocationInView (View);
			UpdateLayers (dropLocation);
		}

		[Export ("dropInteraction:sessionDidExit:")]
		public void SessionDidExit (UIDropInteraction interaction, IUIDropSession session)
		{
			var dropLocation = session.LocationInView (View);
			UpdateLayers (dropLocation);
		}

		[Export ("dropInteraction:sessionDidEnd:")]
		public void SessionDidEnd (UIDropInteraction interaction, IUIDropSession session)
		{
			var dropLocation = session.LocationInView (View);
			UpdateLayers (dropLocation);
		}

		void UpdateLayers (CGPoint dropLocation)
		{
			if (ImageView.Frame.Contains (dropLocation)) {
				View.Layer.BorderWidth = 0.0f;
				ImageView.Layer.BorderWidth = 2.0f;
			} else if (View.Frame.Contains (dropLocation)) {
				View.Layer.BorderWidth = 5.0f;
				ImageView.Layer.BorderWidth = 0.0f;
			} else {
				View.Layer.BorderWidth = 0.0f;
				ImageView.Layer.BorderWidth = 0.0f;
			}
		}

		#endregion
	}
}
