using System;

using UIKit;
using Foundation;

namespace Chat
{
	public partial class OutgoingCell : UITableViewCell
	{
		public override UILabel TextLabel {
			get {
				return MessageText;
			}
		}

		public OutgoingCell (IntPtr handle)
			: base(handle)
		{
		}
	}
}

