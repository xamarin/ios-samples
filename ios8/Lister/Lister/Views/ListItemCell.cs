using System;

using UIKit;
using Foundation;

using ListerKit;

namespace Lister
{
	[Register("ListItemCell")]
	public class ListItemCell : UITableViewCell
	{
		[Outlet("checkBox")]
		public CheckBox CheckBox { get; set; }

		[Outlet("textField")]
		public UITextField TextField { get; set; }

		bool complete;
		public bool Completed {
			get {
				return complete;
			}
			set {
				complete = value;
				TextField.Enabled = !complete;
				CheckBox.Checked = complete;

				TextField.TextColor = complete ? UIColor.LightGray : UIColor.DarkTextColor;
			}
		}

		public ListItemCell(IntPtr handle)
			: base(handle)
		{

		}

		public override void PrepareForReuse ()
		{
			TextField.Text = string.Empty;
			TextField.TextColor = UIColor.DarkTextColor;
			TextField.Enabled = true;
			CheckBox.Checked = false;
			CheckBox.Hidden = false;
		}
	}
}

