using System;
using System.Linq;
using System.Collections.Generic;
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

				// Do nothing more if the `iceCream` property is null.
				if (iceCream == null)
					return;

				// Add a `UIImageView` for each of the ice cream's valid parts.
				var views = Parts ().Where (part => part != null)
									.Select (part => new UIImageView (part.Image) {
										ContentMode = UIViewContentMode.ScaleAspectFit
									});

				foreach (var v in views)
					AddArrangedSubview (v);
			}
		}

		IEnumerable<IceCreamPart> Parts ()
		{
			yield return iceCream.Topping;
			yield return iceCream.Scoops;
			yield return iceCream.Base;
		}

		public IceCreamView (IntPtr handle) : base (handle)
		{
		}
	}
}
