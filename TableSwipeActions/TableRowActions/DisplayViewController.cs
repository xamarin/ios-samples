using Foundation;
using System;
using UIKit;
using System.IO;
using System.Collections.Generic;

namespace TableRowActions
{
    public partial class DisplayViewController : UITableViewController
    {
        public DisplayViewController (IntPtr handle) : base (handle)
        {
        }

		List<string> words = new List<string>();
		bool hasViewedAlert = false;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var lines = File.ReadLines("word-list.txt");

			foreach (var l in lines)
			{
				words.Add(l);
			}
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return words.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			string item = words[indexPath.Row];

			UITableViewCell cell = tableView.DequeueReusableCell("displaycell");
			cell = cell ?? new UITableViewCell(UITableViewCellStyle.Default, "displaycell");

			cell.TextLabel.Text = item;

			return cell;
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			switch (editingStyle)
			{
				case UITableViewCellEditingStyle.Delete:
					// remove the item from the underlying data source
					words.RemoveAt(indexPath.Row);
					// delete the row from the table
					tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
					break;
				case UITableViewCellEditingStyle.None:
					Console.WriteLine("CommitEditingStyle:None called");
					break;
			}
		} 

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
		{
			return "I know this word.";
		}

		public override UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
		{
			var definitionAction = ContextualDefinitionAction(indexPath.Row);
			var flagAction = ContextualFlagAction(indexPath.Row);
			var leadingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { flagAction, definitionAction });

			leadingSwipe.PerformsFirstActionWithFullSwipe = false;
			return leadingSwipe;
		}

		public UIContextualAction ContextualDefinitionAction(int row)
		{
			string word = words[row];

			var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
																"Definition",
																(ReadLaterAction, view, success) => {
																	var def = new UIReferenceLibraryViewController(word);

																	var alertController = UIAlertController.Create("No Dictionary Installed", "To install a Dictionary, Select Definition again, click `Manage` on the next screen and select a dictionary to download", UIAlertControllerStyle.Alert);
																	alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

																	if (UIReferenceLibraryViewController.DictionaryHasDefinitionForTerm(word) || hasViewedAlert == true){
																		PresentViewController(def, true, null);
																		success(true);
																	}else{
																		PresentViewController(alertController, true, null);
																		hasViewedAlert = true;
																		success(false);
																	}
																});
			action.BackgroundColor = UIColor.Orange;
			return action;
		}

		public UIContextualAction ContextualFlagAction(int row)
		{
			var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
																	  "Flag",
																	  (FlagAction, view, success) => {
																			var alertController = UIAlertController.Create($"Report {words[row]}?", "", UIAlertControllerStyle.Alert);
																			alertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null)); 
																			alertController.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Destructive, null));
																			PresentViewController(alertController, true, null);

																		  success(true);
																	  });

			action.Image = UIImage.FromFile("feedback.png");
			action.BackgroundColor = UIColor.Blue;

			return action;
		}
    }
}