using System;
using Foundation;
using UIKit;
using System.Threading.Tasks;
namespace MusicKitSample.Controllers
{
	public class ImageCacheManager
	{
		#region Fields

		NSCache imageCache;

		#endregion

		#region Constructors

		public ImageCacheManager ()
		{
			imageCache = new NSCache {
				Name = "ImageCacheManager",
				CountLimit = 20, // Max 20 images in memory.
				TotalCostLimit = 10 * 1024 * 1024 // Max 10MB used.
			};
		}

		#endregion

		#region Public Functionality - Image Caching Methods

		public UIImage GetCachedImage (NSUrl url)
		{
			if (url != null)
				return imageCache.ObjectForKey (new NSString (url.AbsoluteString)) as UIImage;
			else
				return new UIImage ();
		}

		public async Task<UIImage> FetchImage (NSUrl url)
		{
			var dataTaskRequest = await NSUrlSession.SharedSession.CreateDataTaskAsync (url);
			var urlResponse = dataTaskRequest.Response as NSHttpUrlResponse;

			if (urlResponse?.StatusCode != 200 || dataTaskRequest.Data == null) {
				// Your application should handle these errors 
				// appropriately depending on the kind of error.
				return null;
			}

			var image = UIImage.LoadFromData (dataTaskRequest.Data);

			if (image != null) {
				imageCache.SetObjectforKey (image, new NSString (url.AbsoluteString));
				return image;
			} else
				return new UIImage ();
		}

		#endregion
	}
}
