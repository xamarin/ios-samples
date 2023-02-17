using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace PhotoAlbum {
	/// <summary>
	/// A table view cell used to display a photo album in the photo library.
	/// </summary>
	public partial class AlbumTableViewCell : UITableViewCell {
		public static string Identifier = "AlbumTableViewCell";

		public AlbumTableViewCell (IntPtr handle) : base (handle)
		{
		}

		/// <summary>
		/// Returns a rect for the image view that displays the album thumbnail in the coordinate space of the cell, if it is visible.
		/// </summary>
		public CGRect? RectForAlbumThumbnail {
			get {
				var imageView = this.ImageView;

				if (imageView != null && imageView.Bounds.Size.Width > 0 && imageView.Bounds.Size.Height > 0 && imageView.Superview != null) {
					return this.ConvertRectToView (imageView.Bounds, imageView);
				}
				return null;
			}
		}

		/// <summary>
		/// Configures the cell to display the album.
		/// </summary>
		public void Configure (PhotoAlbum album)
		{
			Accessory = UITableViewCellAccessory.DisclosureIndicator;
			TextLabel.Text = album.Title;
			ImageView.Image = album?.Thumbnail;
		}
	}
}
