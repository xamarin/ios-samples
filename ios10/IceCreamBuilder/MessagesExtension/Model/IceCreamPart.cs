using System;

using Foundation;
using UIKit;

namespace MessagesExtension {
	public class IceCreamPart : IQueryItemRepresentable {
		public string RawValue { get; set; }

		public UIImage Image {
			get {
				var imageName = RawValue;
				var image = UIImage.FromBundle (imageName);

				if (image == null)
					throw new Exception ($"Unable to find image named {imageName}");

				return image;
			}
		}

		UIImage stickerImage;
		public UIImage StickerImage {
			get {
				stickerImage = stickerImage ?? new UIImage ($"{RawValue}_sticker");
				return stickerImage;
			}
		}

		public NSUrlQueryItem QueryItem {
			get {
				return new NSUrlQueryItem (QueryItemKey, RawValue);
			}
		}

		public virtual string QueryItemKey { get; }

		public IceCreamPart (string rawValue)
		{
			RawValue = rawValue;
		}
	}
}

