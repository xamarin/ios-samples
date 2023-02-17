using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas {
	public partial class TextFieldTableViewCell : FormFieldTableViewCell {
		[Outlet]
		public UITextField TextField { get; set; }

		public TextInput TextInput { get; set; }

		public TextFieldTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public TextFieldTableViewCell (NSCoder coder)
			: base (coder)
		{

		}
	}
}
