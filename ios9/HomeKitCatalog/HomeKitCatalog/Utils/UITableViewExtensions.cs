using UIKit;

namespace HomeKitCatalog
{
	public static class UITableViewExtensions
	{
		public static void SetBackgroundMessage (this UITableView tableView, string msg)
		{
			if (string.IsNullOrWhiteSpace (msg)) {
				tableView.BackgroundView = null;
				tableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			} else {

				var messageLbl = new UILabel {
					Text = msg,
					Font = UIFont.PreferredBody,
					TextColor = UIColor.LightGray,
					TextAlignment = UITextAlignment.Center
				};
				messageLbl.SizeToFit ();

				tableView.BackgroundView = messageLbl;
				tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			}
		}
	}
}