using System;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;
using ImageIO;

namespace ImageIOAnimation {
	public class AnimatedImage : NSObject {

		public NSData data = null;
		public NSUrl url = null;

		// add backing store field - is there a better way to do this?
		private CGSize size;
		public CGSize Size {
			get { return GetSize (); }
			set { this.size = value; }
		}

		public AnimatedImage () : base ()
		{
			size = CGSize.Empty;
		}

		public static AnimatedImage AnimatedImageWithData (NSData data)
		{
			AnimatedImage animatedImage = new AnimatedImage ();
			animatedImage.data = data;
			return animatedImage;
		}

		public static AnimatedImage AnimatedImageWithURL (NSUrl url)
		{
			AnimatedImage animatedImage = new AnimatedImage ();
			animatedImage.url = url;
			return animatedImage;
		}

		public override bool Equals (object obj)
		{
			return obj is AnimatedImage image &&
			       base.Equals (obj) &&
			       EqualityComparer<NSData>.Default.Equals (data, image.data) &&
			       EqualityComparer<NSUrl>.Default.Equals (url, image.url);
		}

		public override int GetHashCode ()
		{
			return HashCode.Combine (base.GetHashCode (), data, url);
		}

		private CGSize GetSize ()
		{
			if (size.Equals (CGSize.Empty)) {
				CGImageSource imageSource = null;
				if (data != null) {
					imageSource = ImageIO.CGImageSource.FromData (data);
				} else if (url != null) {
					imageSource = ImageIO.CGImageSource.FromUrl (url);
				}
				if (imageSource != null) {
					if (imageSource.ImageCount > 0) {
						NSDictionary nullDict = null;
						NSDictionary properties = imageSource.CopyProperties (nullDict, 0);
						System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
						float width = float.Parse (properties ["PixelWidth"].ToString (), cultureInfo);
						float height = float.Parse (properties ["PixelHeight"].ToString (), cultureInfo);
						size = new CGSize (width, height);
					}
				}
			}
			return size;
		}
	}
}
