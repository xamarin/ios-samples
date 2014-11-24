using System;
using UIKit;
using Foundation;

namespace AdaptivePhotos
{
	public class Photo : NSObject
	{
		public UIImage Image {
			get {
				return UIImage.FromBundle (ImageName + ".jpg");
			}
		}

		public string ImageName { get; private set; }

		public string Comment { get; private set; }

		public nuint Rating { get; set; }

		public static Photo PhotoWithDictionary (NSDictionary dictionary)
		{
			return new Photo {
				ImageName = (NSString)dictionary.ObjectForKey (new NSString ("imageName")),
				Comment = (NSString)dictionary.ObjectForKey (new NSString ("comment")),
				Rating = ((NSNumber)dictionary.ObjectForKey (new NSString ("rating"))).UInt32Value,
			};
		}
	}
}

