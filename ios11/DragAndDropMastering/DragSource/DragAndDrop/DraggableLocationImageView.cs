using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Foundation;

namespace DragSource {
	/**
	 A DraggableLocationImageView is a LocationImageView that
	 supports dragging. This view vends its map item as well
	 as its image when dragging, and customizes its preview
	 when beginning a drag session.
	 */
	public class DraggableLocationImageView : LocationImageView, IUIDragInteractionDelegate {
		#region Constructors
		public DraggableLocationImageView ()
		{
		}

		public DraggableLocationImageView (NSCoder coder) : base (coder)
		{
		}

		public DraggableLocationImageView (UIImage image, CLLocation location) : base (image, location)
		{
			// Initialize
			UserInteractionEnabled = true;
			AddInteraction (new UIDragInteraction (this));
		}
		#endregion

		#region Private Methods
		private UIDragPreview DragPreviewForItem ()
		{

			if (Image == null || MapItem == null) return new UIDragPreview (this);
			var previewView = new LocationPlatterView (Image, MapItem);
			var inflatedBounds = previewView.Bounds.Inset (-20, -20);
			var parameters = new UIDragPreviewParameters () {
				VisiblePath = UIBezierPath.FromRoundedRect (inflatedBounds, 20f)
			};
			return new UIDragPreview (previewView, parameters);

		}
		#endregion

		#region UIDragInteractionDelegate
		public UIDragItem [] GetItemsForBeginningSession (UIDragInteraction interaction, IUIDragSession session)
		{
			var results = new List<UIDragItem> ();
			if (MapItem == null) return results.ToArray ();

			var itemProvider = new NSItemProvider (MapItem);
			itemProvider.RegisterObject (Image, NSItemProviderRepresentationVisibility.All);
			results.Add (new UIDragItem (itemProvider));

			return results.ToArray ();
		}

		[Export ("dragInteraction:sessionWillBegin:")]
		public void SessionWillBegin (UIDragInteraction interaction, IUIDragSession session)
		{
			session.Items [0].PreviewProvider = DragPreviewForItem;

		}
		#endregion
	}
}
