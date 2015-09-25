using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace ApplicationShortcuts
{
	public partial class ShortcutsTableViewController : UITableViewController
	{
		// Pre-defined shortcuts; retrieved from the Info.plist, lazily.
		List<UIApplicationShortcutItem> staticShortcuts;

		List<UIApplicationShortcutItem> StaticShortcuts {
			get {
				if (staticShortcuts != null)
					return staticShortcuts;

				staticShortcuts = new List<UIApplicationShortcutItem> ();

				var infoDictionary = NSBundle.MainBundle.InfoDictionary;
				if (infoDictionary == null)
					return staticShortcuts;

				// Obtain the `UIApplicationShortcutItems` array from the Info.plist. If unavailable, there are no static shortcuts.
				var array = infoDictionary ["UIApplicationShortcutItems"] as NSArray;
				if (array == null)
					return staticShortcuts;

				Dictionary<string, NSObject>[] shortcuts = ParseShortcuts (array);

				foreach (var shortcut in  shortcuts) {
					// The `UIApplicationShortcutItemType` and `UIApplicationShortcutItemTitle` keys are required to successfully create a `UIApplicationShortcutItem`.
					NSObject rawShortcutType;
					NSObject rawShortcutTitle;
					if (!shortcut.TryGetValue ("UIApplicationShortcutItemType", out rawShortcutType) ||
						!shortcut.TryGetValue ("UIApplicationShortcutItemTitle", out rawShortcutTitle))
						continue;

					var shortcutType = rawShortcutType as NSString;
					var shortcutTitle = rawShortcutTitle as NSString;
					if (shortcutType == null || shortcutTitle == null)
						continue;

					// The `UIApplicationShortcutItemSubtitle` key is optional and doesn't need to be unwrapped.
					NSObject rawShortcutSubtitle;
					NSString shortcutSubtitle = null;
					if (shortcut.TryGetValue ("UIApplicationShortcutItemSubtitle", out rawShortcutSubtitle))
						shortcutSubtitle = rawShortcutSubtitle as NSString;

					staticShortcuts.Add (new UIApplicationShortcutItem (shortcutType, shortcutTitle, shortcutSubtitle, null, null));
				}

				return staticShortcuts;
			}
		}

		// Shortcuts defined by the application and modifiable based on application state.
		UIApplicationShortcutItem[] dynamicShortcuts;

		UIApplicationShortcutItem[] DynamicShortcuts {
			get {
				if (dynamicShortcuts == null)
					dynamicShortcuts = UIApplication.SharedApplication.ShortcutItems ?? new UIApplicationShortcutItem[0];

				return dynamicShortcuts;
			}
		}


		public ShortcutsTableViewController (IntPtr handle) : base (handle)
		{
		}

		static Dictionary<string, NSObject>[] ParseShortcuts (NSArray items)
		{
			var count = (int)items.Count;
			var result = new Dictionary<string, NSObject>[count];
			for (int i = 0; i < count; i++) {
				var nDict = items.GetItem<NSDictionary> ((nuint)i);
				var dict = result [i] = new Dictionary<string, NSObject> ();
				foreach (var kvp in nDict)
					dict [(NSString)kvp.Key] = kvp.Value;
			}

			return result;
		}

		#region UITableViewDataSource

		public override nint NumberOfSections (UITableView tableView)
		{
			return 2;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return section == 0 ? "Static" : "Dynamic";
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return section == 0 ? StaticShortcuts.Count : DynamicShortcuts.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("CellID", indexPath);

			UIApplicationShortcutItem shortcut;

			if (indexPath.Section == 0) {
				// Static shortcuts (cannot be edited).
				shortcut = staticShortcuts [indexPath.Row];
				cell.Accessory = UITableViewCellAccessory.None;
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			} else {
				// Dynamic shortcuts.
				shortcut = dynamicShortcuts [indexPath.Row];
			}

			cell.TextLabel.Text = shortcut.LocalizedTitle;
			cell.DetailTextLabel.Text = shortcut.LocalizedSubtitle;

			return cell;
		}

		#endregion

		#region Navigation

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			// Supply the `shortcutItem` matching the selected row from the data source.
			if (segue.Identifier != "ShowShortcutDetail")
				return;

			var indexPath = TableView.IndexPathForSelectedRow;
			var controller = segue.DestinationViewController as ShortcutDetailViewController;
			if (controller != null && indexPath != null)
				controller.ShortcutItem = DynamicShortcuts [indexPath.Row];
		}

		public override bool ShouldPerformSegue (string segueIdentifier, NSObject sender)
		{
			// Block navigating to detail view controller for static shortcuts (which are not editable).
			var selectedIndexPath = TableView.IndexPathForSelectedRow;
			if (selectedIndexPath == null)
				return false;

			return selectedIndexPath.Section > 0;
		}

		#endregion

		#region Actions

		// Unwind segue action called when the user taps 'Done' after navigating to the detail controller.
		[Export ("done:")]
		void Done (UIStoryboardSegue sender)
		{
			// Obtain the edited shortcut from our source view controller.
			var sourceViewController = sender.SourceViewController as ShortcutDetailViewController;
			if (sourceViewController == null)
				return;

			var selected = TableView.IndexPathForSelectedRow;
			if (selected == null)
				return;

			var updatedShortcutItem = sourceViewController.ShortcutItem;
			if (updatedShortcutItem == null)
				return;

			// Update our data source.
			DynamicShortcuts [selected.Row] = updatedShortcutItem;

			// Update the application's `shortcutItems`.
			UIApplication.SharedApplication.ShortcutItems = DynamicShortcuts;

			TableView.ReloadRows (new []{ selected }, UITableViewRowAnimation.Automatic);
		}

		// Unwind segue action called when the user taps 'Cancel' after navigating to the detail controller.
		[Export ("cancel:")]
		void cancel (UIStoryboardSegue sender)
		{
		}

		#endregion
	}
}