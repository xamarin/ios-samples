using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using UIKit;

namespace BasicTable
{
	public class TableDelegate : UITableViewDelegate
	{
		#region Constructors
		public TableDelegate ()
		{
		}

		public TableDelegate (IntPtr handle) : base (handle)
		{
		}

		public TableDelegate (NSObjectFlag t) : base (t)
		{
		}
			
		#endregion

		#region Override Methods
		/// <summary>
		/// Override the default behavior when editing a row in the table and define a set of 
		/// custom actions.
		/// </summary>
		/// <returns>The actions for row.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="indexPath">Index path.</param>
		public override UITableViewRowAction[] EditActionsForRow (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewRowAction hiButton = UITableViewRowAction.Create (
				UITableViewRowActionStyle.Default, 
				"Hi",
				delegate {
					Console.WriteLine ("Hello World!");
				});
			return new UITableViewRowAction[] { hiButton };
		}
		#endregion
	}
}

