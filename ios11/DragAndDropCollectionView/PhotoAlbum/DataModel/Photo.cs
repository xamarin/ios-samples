using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace PhotoAlbum
{
	/// <summary>
	/// A model object that represents a photo.
	/// </summary>
	public partial class Photo : NSObject
    {
        public Photo(UIImage image)
        {
            this.image = image;
            thumbnail = GenerateThumbnail(image, new CGSize(50, 50));
        }

        public readonly Guid Identifier = Guid.NewGuid();
        public UIImage image;
        public UIImage thumbnail;

        #region Equatable
        public override bool Equals(System.Object obj)
		{
			return ((Photo)obj).Identifier == Identifier;
		}
		public bool Equals(Photo p)
		{
			return p.Identifier == Identifier;
		}
		public override int GetHashCode()
		{
			return Identifier.GetHashCode();
		}
        #endregion

        /// <summary>
        /// Generates and returns a thumbnail for the image using scale aspect fill.
        /// </summary>
        UIImage GenerateThumbnail (UIImage forImage, CGSize thumbnailSize){
            var imageSize = forImage.Size;

            var widthRatio = thumbnailSize.Width / imageSize.Width;
            var heightRatio = thumbnailSize.Height / imageSize.Height;
            var scaleFactor = widthRatio > heightRatio ? widthRatio : heightRatio;

            var renderer = new UIGraphicsImageRenderer(thumbnailSize);
            var newThumbnail = renderer.CreateImage((err) =>
            {
                var size = new CGSize(imageSize.Width * scaleFactor, imageSize.Height * scaleFactor);
                var x = (thumbnailSize.Width - size.Width) / 2.0;
                var y = (thumbnailSize.Height - size.Height) / 2.0;
                forImage.Draw(new CGRect(new CGPoint(x,y), size));
            });
            return newThumbnail;
        }
    }
}
