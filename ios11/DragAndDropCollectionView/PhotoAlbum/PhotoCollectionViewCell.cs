using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace PhotoAlbum {
	/// <summary>
	/// A collection view cell used to display a photo in a photo album.
	/// </summary>
	public partial class PhotoCollectionViewCell : UICollectionViewCell {
		public static string Identifier = "PhotoCollectionViewCell";

		public PhotoCollectionViewCell (IntPtr handle) : base (handle)
		{
			photoImageView = new UIImageView ();
			photoImageView.Frame = new CGRect (0, 0, 200, 200);
			photoImageView.AlignmentRectForFrame (new CGRect (0, 0, 200, 200));
			AddSubview (photoImageView);
		}

		public CGRect ClippingRectForPhoto {
			get {
				return photoImageView.ContentClippingRect ();
			}
		}

		/// <summary>
		/// Configures the cell to display the photo.
		/// </summary>
		public void Configure (Photo photo)
		{
			photoImageView.Image = photo.image;
		}
	}
}
