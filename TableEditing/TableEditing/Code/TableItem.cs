using System;
using UIKit;

namespace TableEditing.Code {
	/// <summary>
	/// Represents our item in the table
	/// </summary>
	public class TableItem
	{
		public string Heading { get; set; }
		
		public string SubHeading { get; set; }
		
		public string ImageName { get; set; }
		
		public UITableViewCellStyle CellStyle {
			get { return cellStyle; }
			set { cellStyle = value; }
		}
		UITableViewCellStyle cellStyle = UITableViewCellStyle.Default;
		
		public UITableViewCellAccessory CellAccessory {
			get { return cellAccessory; }
			set { cellAccessory = value; }
		}
		UITableViewCellAccessory cellAccessory = UITableViewCellAccessory.None;

		public TableItem () { }
		
		public TableItem (string heading)
		{ this.Heading = heading; }
	}
}