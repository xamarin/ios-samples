using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Foundation;

namespace DragSource
{
	/**
	 The DraggableQRCodeImageView is a subclass of QRCodeDetectedImageView
	 supports for dragging cropped images representing the QR codes that
	 appear in the image view.
	 */
	public class DraggableQRCodeImageView : QRCodeDetectedImageView, IUIDragInteractionDelegate
	{
		#region Constructors
		public DraggableQRCodeImageView()
		{
		}

		public DraggableQRCodeImageView(NSCoder coder) : base(coder)
		{
		}

		public DraggableQRCodeImageView(UIImage image) : base(image)
		{
			// Initialize
			UserInteractionEnabled = true;
			AddInteraction(new UIDragInteraction(this));
		}
		#endregion

		#region Private Methods
		private UITargetedDragPreview DragPreviewForItem(UIDragItem item) {
			var qrCode = item.LocalObject as QRDetectionResult;
			if (qrCode == null) return new UITargetedDragPreview(this);
			var previewView = new UIImageView(qrCode.CroppedImage);
			previewView.Frame = ConvertRectToView(this.ConvertFromImageRect(qrCode.RectInOriginalImage), Window);

			var target = new UIDragPreviewTarget(Window, previewView.Center);
			var parameters = new UIDragPreviewParameters()
			{
				VisiblePath = UIBezierPath.FromRoundedRect(previewView.Bounds, 20f)
			};

			return new UITargetedDragPreview(previewView, parameters, target);
		}
		#endregion

		#region UIDragInteractionDelegate
		UIDragItem[] IUIDragInteractionDelegate.GetItemsForBeginningSession(UIDragInteraction interaction, IUIDragSession session)
		{
			var results = new List<UIDragItem>();

			foreach(QRDetectionResult qrCode in QRCodes) {
				var itemProvider = new NSItemProvider((NSString)qrCode.Message);
				itemProvider.RegisterObject(qrCode.CroppedImage, NSItemProviderRepresentationVisibility.All);
				var item = new UIDragItem(itemProvider) {
					LocalObject = qrCode
				};
				results.Add(item);
			}

			return results.ToArray();
		}

		[Export("dragInteraction:previewForLiftingItem:session:")]
		public UITargetedDragPreview GetPreviewForLiftingItem(UIDragInteraction interaction, UIDragItem item, IUIDragSession session)
		{
			return DragPreviewForItem(item);
		}

		[Export("dragInteraction:previewForCancellingItem:withDefault:")]
		public UITargetedDragPreview GetPreviewForCancellingItem(UIDragInteraction interaction, UIDragItem item, UITargetedDragPreview defaultPreview)
		{
			return DragPreviewForItem(item);
		}

		[Export("dragInteraction:willAnimateLiftWithAnimator:session:")]
		public void WillAnimateLift(UIDragInteraction interaction, IUIDragAnimating animator, IUIDragSession session)
		{
			animator.AddAnimations(()=>{
				Alpha = 0.5f;
			});
		}

		[Export("dragInteraction:item:willAnimateCancelWithAnimator:")]
		public void WillAnimateCancel(UIDragInteraction interaction, UIDragItem item, IUIDragAnimating animator)
		{
			animator.AddAnimations(() => {
				Alpha = 1f;
			});
		}

		[Export("dragInteraction:session:didEndWithOperation:")]
		public void SessionDidEnd(UIDragInteraction interaction, IUIDragSession session, UIDropOperation operation)
		{
			Alpha = 1f;
		}
		#endregion
	}
}
