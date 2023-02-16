/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A basic collection view cell that displays an image.
*/

using System;

using Foundation;
using UIKit;

namespace Gallery {
	public partial class GalleryCollectionViewCell : UICollectionViewCell {
		public static readonly NSString Key = new NSString (nameof (GalleryCollectionViewCell));
		public static readonly UINib Nib;

		public UIImage Image {
			get => imageView.Image;
			set => imageView.Image = value;
		}

		static GalleryCollectionViewCell ()
		{
			Nib = UINib.FromName ("GalleryCollectionViewCell", NSBundle.MainBundle);
		}

		protected GalleryCollectionViewCell (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
	}
}
