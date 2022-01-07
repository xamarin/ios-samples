/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A basic collection view cell that displays an image.
*/

namespace Gallery;
public partial class GalleryCollectionViewCell : UICollectionViewCell {
	public static readonly NSString Key = new NSString (nameof (GalleryCollectionViewCell));
	public static readonly UINib Nib;

	public UIImage? Image
	{
		get
		{
			if (imageView is null || imageView.Image is null)
				return null;

			return imageView!.Image!;
		}
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
