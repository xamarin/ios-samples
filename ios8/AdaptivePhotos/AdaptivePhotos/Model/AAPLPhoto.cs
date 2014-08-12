using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace AdaptivePhotos
{
	public class AAPLPhoto : NSObject
	{
		public UIImage Image {
			get {
				return UIImage.FromBundle (ImageName + ".jpg");
			}
		}

		public string ImageName { get; private set; }

		public string Comment { get; private set; }

		public int Rating { get; set; }

		public static AAPLPhoto PhotoWithDictionary (NSDictionary dictionary)
		{
			return new AAPLPhoto {
				ImageName = (NSString)dictionary.ObjectForKey (new NSString ("imageName")),
				Comment = (NSString)dictionary.ObjectForKey (new NSString ("comment")),
				Rating = ((NSNumber)dictionary.ObjectForKey (new NSString ("rating"))).IntValue,
			};
		}
	}
}

