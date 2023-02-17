using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace PhotoAlbum {
	public partial class Photo {
		/// <summary>
		/// Returns an item provider that can load the image for this photo.
		/// </summary>
		public NSItemProvider ItemProvider {
			get {
				return new NSItemProvider (image);
			}
		}

		/// <summary>
		/// Returns a closure that will generate a drag preview for this photo.
		/// </summary>
		public UIDragPreview PreviewProvider ()
		{
			var imageView = new UIImageView (image);
			imageView.Bounds = new CGRect (imageView.Bounds.Location, new CGSize (200, 200));
			imageView.Frame = imageView.ContentClippingRect ();
			return new UIDragPreview (imageView);
		}
	}
}
