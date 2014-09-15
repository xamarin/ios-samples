using System;

using UIKit;
using Foundation;

using ListerKit;

namespace ListerToday
{
	[Register("CheckBoxCell")]
	public class CheckBoxCell : UITableViewCell
	{
		[Outlet("label")]
		public UILabel Label { get; private set; }

		[Outlet("checkBox")]
		public CheckBox CheckBox { get; private set; }

		public CheckBoxCell (IntPtr handle)
			: base(handle)
		{
		}

		public override void PrepareForReuse ()
		{
			TextLabel.Text = string.Empty;
			TextLabel.TextColor = UIColor.White;
			CheckBox.Checked = false;
			CheckBox.Hidden = false;
			CheckBox.TintColor = UIColor.Clear;
		}
	}
}

