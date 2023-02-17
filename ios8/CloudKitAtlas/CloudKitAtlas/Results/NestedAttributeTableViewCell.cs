using System;
using Foundation;

namespace CloudKitAtlas {
	// TODO: why do we need this class?
	public partial class NestedAttributeTableViewCell : AttributeTableViewCell {
		public NestedAttributeTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public NestedAttributeTableViewCell (NSCoder coder)
			: base (coder)
		{

		}
	}
}
