using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace StoryboardTables1 {
	public partial class ItemViewController : UITableViewController {
		List<Chores> chores;

		public ItemViewController (IntPtr handle) : base (handle)
		{
			chores = new List<Chores> {
				new Chores {Name="Groceries", Notes="Buy bread, cheese, apples", Done=false},
				new Chores {Name="Devices", Notes="Buy Nexus, Galaxy, Droid", Done=false}
			};
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "TaskSegue") { // set in Storyboard
				var navctlr = segue.DestinationViewController as TaskDetailViewController;
				if (navctlr != null) {
					var source = TableView.Source as RootTableSource;
					var rowPath = TableView.IndexPathForSelectedRow;
					var item = source.GetItem (rowPath.Row);
					navctlr.SetTask (this, item); // to be defined on the TaskDetailViewController
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			addButton.Clicked += (sender, e) => CreateTask ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			TableView.Source = new RootTableSource (chores.ToArray ());
		}

		public void SaveTask (Chores chore)
		{
			var oldTask = chores.Find (t => t.Id == chore.Id);
			NavigationController.PopViewController (true);
		}

		public void DeleteTask (Chores chore)
		{
			var oldTask = chores.Find (t => t.Id == chore.Id);
			chores.Remove (oldTask);
			NavigationController.PopViewController (true);
		}

		public void CreateTask ()
		{
			// first, add the task to the underlying data
			var newId = chores [chores.Count - 1].Id + 1;
			var newChore = new Chores { Id = newId };
			chores.Add (newChore);

			// then open the detail view to edit it
			var detail = Storyboard.InstantiateViewController ("detail") as TaskDetailViewController;
			detail.SetTask (this, newChore);
			NavigationController.PushViewController (detail, true);
		}
	}
}
