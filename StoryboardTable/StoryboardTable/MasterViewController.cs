using System;
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace StoryboardTable
{
	public partial class MasterViewController : UITableViewController
	{
		List<Chore> chores;

		public MasterViewController (IntPtr handle) : base (handle)
		{
			Title = "ChoreBoard";

			// Custom initialization
			chores = new List<Chore> {
			new Chore() {Name="Groceries", Notes="Buy bread, cheese, apples", Done=false},
			new Chore() {Name="Devices", Notes="Buy Nexus, Galaxy, Droid", Done=false}
		};

		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "TaskSegue") { // set in Storyboard
				var navctlr = segue.DestinationViewController as TaskDetailViewController;
				if (navctlr != null) {
					var source = TableView.Source as RootTableSource;
					var rowPath = TableView.IndexPathForSelectedRow;
					var item = source.GetItem(rowPath.Row);
					navctlr.SetTask (this, item); // to be defined on the TaskDetailViewController
				}
			}
		}

		public void CreateTask ()
		{
			// first, add the task to the underlying data
			var newId = chores[chores.Count - 1].Id + 1;
			var newChore = new Chore(){Id=newId};
			chores.Add (newChore);

			// then open the detail view to edit it
			var detail = Storyboard.InstantiateViewController("detail") as TaskDetailViewController;
			detail.SetTask (this, newChore);
			NavigationController.PushViewController (detail, true);
		}

		public void SaveTask (Chore chore)
		{
			//var oldTask = chores.Find(t => t.Id == chore.Id);
			NavigationController.PopViewController(true);
		}

		public void DeleteTask (Chore chore)
		{
			var oldTask = chores.Find(t => t.Id == chore.Id);
			chores.Remove (oldTask);
			NavigationController.PopViewController(true);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.

			/*AddButton.Clicked += (sender, e) => {
				CreateTask ();
			};*/
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// bind every time, to reflect deletion in the Detail view
			TableView.Source = new RootTableSource(chores.ToArray ());
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		//To create a New Task
		partial void AddButton_Activated (UIBarButtonItem sender)
		{
			CreateTask ();
		}
		#endregion

//		class DataSource : UITableViewSource
//		{
//			static readonly NSString CellIdentifier = new NSString ("Cell");
//			readonly List<object> objects = new List<object> ();
//			readonly MasterViewController controller;
//
//			public DataSource (MasterViewController controller)
//			{
//				this.controller = controller;
//			}
//
//			public IList<object> Objects {
//				get { return objects; }
//			}
//
//
//			// Customize the number of sections in the table view.
//			public override int NumberOfSections (UITableView tableView)
//			{
//				return 1;
//			}
//
//			public override int RowsInSection (UITableView tableview, int section)
//			{
//				return objects.Count;
//			}
//
//			// Customize the appearance of table view cells.
//			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
//			{
//				var cell = (UITableViewCell)tableView.DequeueReusableCell (CellIdentifier, indexPath);
//
//				cell.TextLabel.Text = objects [indexPath.Row].ToString ();
//
//				return cell;
//			}
//
//			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
//			{
//				// Return false if you do not want the specified item to be editable.
//				return true;
//			}
//
//			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
//			{
//				if (editingStyle == UITableViewCellEditingStyle.Delete) {
//					// Delete the row from the data source.
//					objects.RemoveAt (indexPath.Row);
//					controller.TableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
//				} else if (editingStyle == UITableViewCellEditingStyle.Insert) {
//					// Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
//				}
//			}
//
//			/*
//			// Override to support rearranging the table view.
//			public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
//			{
//			}
//			*/
//
//			/*
//			// Override to support conditional rearranging of the table view.
//			public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
//			{
//				// Return false if you do not want the item to be re-orderable.
//				return true;
//			}
//			*/
//		}

	}
}

