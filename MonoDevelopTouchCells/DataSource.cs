using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonoDevelopTouchCells
{
	
	public class DataSource : UITableViewDataSource {
		private List<Item> Data { get; set; }
		
		public DataSource (IEnumerable<Item> data)
		{
			this.Data = new List<Item>(data);
		}
		
		public override int RowsInSection (UITableView tableView, int section)
		{
			return this.Data.Count;
		}
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			string customCellID = "MyCellID" + indexPath.Section + indexPath.Row;
			
			CustomCell cell = (CustomCell)tableView.DequeueReusableCell(customCellID);
			
			if (cell == null)
			{
				cell = new CustomCell(UITableViewCellStyle.Default, customCellID);
				
				Item item = this.Data[indexPath.Row];
				cell.Title = item.Title;
				cell.Checked = item.Checked;
			}
			
			return cell;
		}

	}

}
