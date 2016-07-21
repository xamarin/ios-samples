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

		public UIImage StickerImage {
			get {
				var image = new UIImage ($"{RawValue}_sticker");

				if (image == null)
					throw new Exception ($"Unable to find sticker image");

				return image;
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

