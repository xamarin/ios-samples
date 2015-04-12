using System;

using UIKit;
using Foundation;

namespace Chat
{
	public partial class LeftCell : UITableViewCell
	{
		public override UILabel TextLabel {
			get {
				return MessageText;
			}
		}

		public LeftCell (IntPtr handle)
			: base(handle)
		{
		}
	}
}