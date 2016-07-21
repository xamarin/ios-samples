using System;

using UIKit;

namespace MessagesExtension {
	public partial class IceCreamView : UIStackView {

		IceCream iceCream;
		public IceCream IceCream {
			get {
				return iceCream;
			}
			set {
				iceCream = value;

				// Remove any existing arranged subviews.
				foreach (var view in ArrangedSubviews)
					RemoveArrangedSubview (view);

				// Do nothing more if the `iceCream` property is nil.
				if (iceCream == null)
					return;

				// Add a `UIImageView` for each of the ice cream's valid parts.
				IceCreamPart[] iceCreamParts = {
					iceCream.Topping,
					iceCream.Scoops,
					iceCream.Base
				};

				foreach (var iceCreamPart in iceCreamParts) {
					if (iceCreamPart == null)
						continue;

					var imageView = new UIImageView (iceCreamPart.Image) {
						ContentMode = UIViewContentMode.ScaleAspectFit
					};
					AddArrangedSubview (imageView);
				}
			}
		}

		public IceCreamView (IntPtr handle) : base (handle)
		{
		}
	}
}

