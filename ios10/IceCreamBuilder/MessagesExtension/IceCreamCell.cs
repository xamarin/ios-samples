using System;

using UIKit;

namespace MessagesExtension {
	public partial class IceCreamCell : UICollectionViewCell {
		public new static string ReuseIdentifier = "IceCreamCell";

		public IceCream RepresentedIceCream { get; set; }

		public IceCreamCell (IntPtr handle) : base (handle)
		{
		}
	}
}
