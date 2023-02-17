using FontList.Models;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace FontList {
	public partial class FontsViewController : UITableViewController {
		private const string CellIdentifier = "FontCellView";
		private const string FontDetailsSegueIdentifier = "fontDetailsSegue";

		private readonly List<FontFamilyItem> items = new List<FontFamilyItem> ();

		private FontItem selectedItem;

		public FontsViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			foreach (var fontFamily in UIFont.FamilyNames) {
				// create a nav group
				var group = new FontFamilyItem (fontFamily);

				var fontNames = UIFont.FontNamesForFamilyName (fontFamily);
				if (fontNames.Any ()) {
					foreach (var fontName in fontNames) {
						var font = UIFont.FromName (fontName, UIFont.LabelFontSize);
						if (font != null) {
							group.Items.Add (new FontItem { Name = fontName, Font = font });
						}
					}
				} else {
					var font = UIFont.FromName (fontFamily, UIFont.LabelFontSize);
					group.Items.Add (new FontItem { Name = fontFamily, Font = font });
				}

				items.Add (group);
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (!string.IsNullOrEmpty (segue?.Identifier) && segue.Identifier == FontDetailsSegueIdentifier) {
				if (segue.DestinationViewController is FontDetailsViewController controller) {
					controller.FontItem = selectedItem;
					selectedItem = null;
				}
			}
		}

		#region table delegate

		public override nint NumberOfSections (UITableView tableView)
		{
			return items.Count;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return items [(int) section].Items.Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return items [(int) section].Name;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var item = items [indexPath.Section].Items [indexPath.Row];

			var cell = tableView.DequeueReusableCell (CellIdentifier);
			cell.TextLabel.Text = item.Name;
			cell.TextLabel.Font = item.Font;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			selectedItem = items [indexPath.Section].Items [indexPath.Row];
			PerformSegue (FontDetailsSegueIdentifier, this);
		}

		#endregion
	}
}
