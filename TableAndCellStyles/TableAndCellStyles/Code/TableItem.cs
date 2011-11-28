using System;
using MonoTouch.UIKit;
namespace Example_TableAndCellStyles.Code
{
	/// <summary>
	/// Represents our item in the table
	/// </summary>
	public class TableItem
	{
		public string Heading { get; set; }
		
		public string SubHeading { get; set; }
		
		public string ImageName { get; set; }
		
		public UITableViewCellStyle CellStyle
		{
			get { return cellStyle; }
			set { cellStyle = value; }
		}
		protected UITableViewCellStyle cellStyle = UITableViewCellStyle.Default;
		
		public UITableViewCellAccessory CellAccessory
		{
			get { return cellAccessory; }
			set { cellAccessory = value; }
		}
		protected UITableViewCellAccessory cellAccessory = UITableViewCellAccessory.None;

		public TableItem () { }
		
		public TableItem (string heading)
		{ this.Heading = heading; }
	}
}

