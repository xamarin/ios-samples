using System;

using Foundation;
using UIKit;

namespace MessagesExtension {
	public partial class CompletedIceCreamViewController : UIViewController {
		public static readonly string StoryboardIdentifier = "CompletedIceCreamViewController";

		public IceCream IceCream { get; set; }

		public CompletedIceCreamViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			if (IceCream == null)
				throw new Exception ("No ice cream has been set");

			base.ViewDidLoad ();

			// Update the sticker view
			var cache = IceCreamStickerCache.Cache;
			StickerView.Sticker = cache.PlaceholderSticker;

			cache.GetSticker (IceCream, (sticker) => {
				NSOperationQueue.MainQueue.AddOperation (() => {
					if (IsViewLoaded)
						StickerView.Sticker = sticker;
				});
			});
		}
	}
}
