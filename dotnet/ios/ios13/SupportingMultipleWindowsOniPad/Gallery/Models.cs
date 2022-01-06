/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A file that defines our data models (Photo and PhotoSection).
*/

namespace Gallery;
public static class GalleryOpenDetailData {
	public static readonly string ActivityType = "com.xamarin.Gallery.openDetail";
	public static readonly string DetailPath = "openDetail";
	public static readonly string PhotoIdKey = "photoId";
}

public class Photo : NSObject {
	public string? Name { get; set; }

	public NSUserActivity OpenDetailUserActivity ()
	{
		// Create an NSUserActivity from our photo model.
		// Note: The ActivityType string below must be included in your Info.plist file under the `NSUserActivityTypes` array.
		// More info: https://developer.apple.com/documentation/foundation/nsuseractivity
		return new NSUserActivity (GalleryOpenDetailData.ActivityType) {
			Title = GalleryOpenDetailData.DetailPath,
			UserInfo = NSDictionary.FromObjectsAndKeys (new [] { Name }, new [] { GalleryOpenDetailData.PhotoIdKey }, 1)
		};
	}
}

public class PhotoSection {
	public string? Name { get; set; }
	public Photo []? Photos { get; set; }
}

public class PhotoManager {
	static Lazy<PhotoManager> lazy = new Lazy<PhotoManager> (() => new PhotoManager ());
	public static PhotoManager SharedInstance { get; } = lazy.Value;

	PhotoManager ()	{ }

	public PhotoSection [] Sections { get; } = {
		new PhotoSection {
			Name = "Section 1",
			Photos = new [] {
				new Photo { Name = "1.jpg" },
				new Photo { Name = "2.jpg" }
			}
		},
		new PhotoSection {
			Name = "Section 2",
			Photos = new [] {
				new Photo { Name = "3.jpg" },
				new Photo { Name = "4.jpg" },
				new Photo { Name = "5.jpg" }
			}
		},
		new PhotoSection {
			Name = "Section 3",
			Photos = new [] {
				new Photo { Name = "6.jpg" },
				new Photo { Name = "7.jpg" }
			}
		},
	};
}
