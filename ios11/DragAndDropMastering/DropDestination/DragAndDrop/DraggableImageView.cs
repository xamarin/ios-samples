using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;

namespace DropDestination {
	/**
	 A DraggableImageView is a UIImageView that can be
	 dragged, and also fades out when dragged. It is also
	 capable of presenting and hiding a progress spinner
	 within its view hierarchy, given a Progress.
	 */
	public class DraggableImageView : UIImageView, IUIDragInteractionDelegate {
		#region Computed Properties
		public ProgressSpinnerView ProgressView { get; set; }

		public UIDragItem [] DragItems {
			get {
				if (Image == null) return new UIDragItem [] { };
				var itemProvider = new NSItemProvider (Image);
				var item = new UIDragItem (itemProvider);
				item.LocalObject = this;
				return new UIDragItem [] { item };
			}
		}
		#endregion

		#region Constructors
		public DraggableImageView () : base (CGRect.Empty)
		{
			// Initialize
			UserInteractionEnabled = true;
			AddInteraction (new UIDragInteraction (this));
		}

		public DraggableImageView (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Public Methods
		public void StartShowingProgress (NSProgress progress)
		{
			StopShowingProgress ();
			ProgressView = new ProgressSpinnerView (Bounds, progress);
			AddSubview (ProgressView);
		}

		public void StopShowingProgress ()
		{
			if (ProgressView != null) {
				ProgressView.RemoveFromSuperview ();
				ProgressView.Dispose ();
				ProgressView = null;
			}
		}
		#endregion

		#region UIDragInteractionDelegate
		[Export ("dragInteraction:willAnimateLiftWithAnimator:session:")]
		public void WillAnimateLift (UIDragInteraction interaction, IUIDragAnimating animator, IUIDragSession session)
		{
			animator.AddAnimations (() => {
				Alpha = 0.25f;
			});
		}

		[Export ("dragInteraction:item:willAnimateCancelWithAnimator:")]
		public void WillAnimateCancel (UIDragInteraction interaction, UIDragItem item, IUIDragAnimating animator)
		{
			animator.AddAnimations (() => {
				Alpha = 1;
			});
		}

		[Export ("dragInteraction:previewForLiftingItem:session:")]
		public UITargetedDragPreview GetPreviewForLiftingItem (UIDragInteraction interaction, UIDragItem item, IUIDragSession session)
		{
			var imageView = new UIImageView (Image) {
				Frame = this.Frame,
				ClipsToBounds = true,
				ContentMode = UIViewContentMode.ScaleAspectFill
			};
			var target = new UIDragPreviewTarget (Superview, imageView.Center);
			return new UITargetedDragPreview (imageView, new UIDragPreviewParameters (), target);

		}

		public UIDragItem [] GetItemsForBeginningSession (UIDragInteraction interaction, IUIDragSession session)
		{
			return DragItems;
		}

		[Export ("dragInteraction:itemsForAddingToSession:withTouchAtPoint:")]
		public UIDragItem [] GetItemsForAddingToSession (UIDragInteraction interaction, IUIDragSession session, CGPoint point)
		{
			return DragItems;
		}
		#endregion
	}
}
