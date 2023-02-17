using System;
using System.IO;

using Foundation;
using Messages;

namespace MessagesExtension {
	public class IceCreamStickerCache : IDisposable {
		static IceCreamStickerCache cache;
		string cacheURL;
		bool disposedValue;

		public static IceCreamStickerCache Cache {
			get {
				cache = cache ?? new IceCreamStickerCache ();
				return cache;
			}
		}

		public MSSticker PlaceholderSticker {
			get {
				var bundle = NSBundle.MainBundle;
				var placeholderURL = bundle.GetUrlForResource ("sticker_placeholder", "png");
				if (placeholderURL == null)
					throw new Exception ("Unable to find placeholder sticker image");

				var description = "An ice cream sticker";

				NSError error;
				var sticker = new MSSticker (placeholderURL, description, out error);

				if (error != null)
					throw new Exception ($"Failed to create placeholder sticker: {error.LocalizedDescription}");

				return sticker;
			}
		}

		IceCreamStickerCache ()
		{
			var fileManager = NSFileManager.DefaultManager;
			cacheURL = Path.GetTempPath ();

			NSError error;
			fileManager.CreateDirectory (cacheURL, true, (NSFileAttributes) null, out error);
			if (error != null)
				throw new Exception ($"Unable to create cache URL: {cacheURL}");
		}

		public void GetSticker (IceCream iceCream, Action<MSSticker> completion)
		{
			if (iceCream.Base == null || iceCream.Scoops == null || iceCream.Topping == null)
				throw new Exception ("Stickers can only be created for completed ice creams");

			// Determine the URL for the sticker.
			var fileName = $"{iceCream.Base.RawValue}{iceCream.Scoops.RawValue}{iceCream.Topping.RawValue}.png";
			var url = Path.Combine (cacheURL, fileName);

			// Check if the sticker already exists at the URL
			if (!NSFileManager.DefaultManager.FileExists (url)) {

				// Create the sticker image and write it to disk.
				var image = iceCream.RenderSticker (false);
				var imageData = image?.AsPNG ();
				if (image == null || imageData == null)
					throw new Exception ("Unable to build image for ice cream");

				try {
					File.WriteAllBytes (url, imageData.ToArray ());
				} catch {
					throw new Exception ("Failed to write sticker image to cach");
				}
			}

			NSError error;
			var sticker = new MSSticker (new NSUrl ($"file://{url}"), "Ice Cream", out error);

			if (error != null)
				throw new Exception ($"Failed to write image to cache, error: {error.LocalizedDescription}");

			completion?.Invoke (sticker);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!disposedValue) {
				var fileManager = NSFileManager.DefaultManager;
				NSError error;
				fileManager.Remove (cacheURL, out error);

				if (error != null)
					throw new Exception ($"Unable to remove cache directory: {cacheURL}");

				disposedValue = true;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
		}
	}
}

