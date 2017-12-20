using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MobileCoreServices;
using CoreFoundation;

namespace DragSource
{
	/**
	 A SlowDraggableImageView is an image view that supports
	 dragging and vends its image when dragged. The item
	 provider loading is adjusted to artificially take a long
	 time to complete.

	 Drag items coming from this view can be dropped into the
	 drop destination app to demonstrate custom progress UI.
	 */
	public class SlowDraggableImageView : UIImageView, IUIDragInteractionDelegate
	{
		#region Computed Properties
		public float Delay { get; set; } = 0f;

		public UIDragItem[] DragItems
		{
			get
			{
				var results = new List<UIDragItem>();

				var itemProvider = new NSItemProvider();
				itemProvider.RegisterDataRepresentation(UTType.PNG, NSItemProviderRepresentationVisibility.All, SlowImageTransfer);
				results.Add(new UIDragItem(itemProvider));

				return results.ToArray();
			}
		}
		#endregion

		#region Constructors
		public SlowDraggableImageView()
		{
		}

		public SlowDraggableImageView(NSCoder coder) : base(coder)
		{
		}

		public SlowDraggableImageView(UIImage image, float delay) : base(image)
		{
			// Initialize
			Delay = delay;
			UserInteractionEnabled = true;
			AddInteraction(new UIDragInteraction(this));
		}
		#endregion

		#region Private Methods
		private NSProgress SlowImageTransfer(ItemProviderDataCompletionHandler completionHandler){

			var progress = NSProgress.FromTotalUnitCount(100);
			var loadStartDate = new NSDate();

			DispatchQueue.MainQueue.DispatchAsync(()=>{
				progress.BecomeCurrent(100);
				NSTimer.CreateScheduledTimer(0.033f, true, (timer) => {
					progress.CompletedUnitCount = CompletedUnitCount(loadStartDate);
					if (progress.CompletedUnitCount >= 100) {
						completionHandler(Image.AsPNG(), null);
						timer.Invalidate();
					}
				});
			});

			return progress;
		}

		private Int64 CompletedUnitCount(NSDate loadStartDate) {
			return (long)Math.Round(Math.Max(0, Math.Min(1, NSDate.FromTimeIntervalSinceReferenceDate(loadStartDate.SecondsSinceReferenceDate).SecondsSinceReferenceDate / Delay)) * 100);
		}
		#endregion

		#region UIDragInteractionDelegate
		public UIDragItem[] GetItemsForBeginningSession(UIDragInteraction interaction, IUIDragSession session)
		{
			return DragItems;
		}

		[Export("dragInteraction:itemsForAddingToSession:withTouchAtPoint:")]
		public UIDragItem[] GetItemsForAddingToSession(UIDragInteraction interaction, IUIDragSession session, CGPoint point)
		{
			return DragItems;
		}
		#endregion
	}
}
