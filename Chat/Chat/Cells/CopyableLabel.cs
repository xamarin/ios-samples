using System;

using UIKit;
using Foundation;
using ObjCRuntime;

namespace Chat
{
	[Register("CopyableLabel")]
	public class CopyableLabel : UILabel
	{
		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}

		public CopyableLabel(IntPtr handle)
			: base(handle)
		{
		}

		public override bool CanPerform (Selector action, NSObject withSender)
		{
			return action.Name == "copy:";
		}

		public override void Copy (NSObject sender)
		{
			UIPasteboard.General.String = Text;
		}
	}
}

