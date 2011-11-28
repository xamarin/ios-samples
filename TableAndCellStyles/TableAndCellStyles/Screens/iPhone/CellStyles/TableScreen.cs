using System;
using Example_TableAndCellStyles.Code;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace Example_TableAndCellStyles.Screens.iPhone.CellStyles
{
	public class TableScreen : UITableViewController
	{
		protected TableSource tableSource;
		protected UITableViewCellStyle cellStyle;
		protected UITableViewCellAccessory cellAccessory;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public TableScreen (IntPtr handle) : base(handle)
		{

		}

		[Export("initWithCoder:")]
		public TableScreen (NSCoder coder) : base(coder)
		{
		}

		/// <summary>
		/// You specify the table style in the constructor when using a UITableViewController
		/// </summary>
		public TableScreen (UITableViewStyle tableStyle, UITableViewCellStyle cellStyle
			, UITableViewCellAccessory cellAccessory)
			: base (tableStyle)
		{
			this.cellStyle = cellStyle;
			this.cellAccessory = cellAccessory;
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = cellStyle.ToString () + " Style";
			
			CreateTableItems ();
			TableView.Source = tableSource;
			
		}

		/// <summary>
		/// Creates a set of table items.
		/// </summary>
		protected void CreateTableItems ()
		{
			List<TableItemGroup> tableItems = new List<TableItemGroup> ();
			
			// declare vars
			TableItemGroup tGroup;
			
			// Section 1
			tGroup = new TableItemGroup() { Name = "Places" };
			tGroup.Items.Add (new TableItem() { CellStyle = cellStyle, CellAccessory = cellAccessory
				, ImageName = "Images/Beach.png", Heading = "Fiji", SubHeading = "A nice beach" });
			tGroup.Items.Add (new TableItem() { CellStyle = cellStyle, CellAccessory = cellAccessory
				, ImageName = "Images/Shanghai.png", Heading = "Beijing", SubHeading = "AKA Shanghai" });
			tableItems.Add (tGroup);
			
			// Section 2
			tGroup = new TableItemGroup() { Name = "Other" };
			tGroup.Items.Add (new TableItem() { CellStyle = cellStyle, CellAccessory = cellAccessory
				, ImageName = "Images/Seeds.png", Heading = "Seedlings", SubHeading = "Tiny Plants" });
			tGroup.Items.Add (new TableItem() { CellStyle = cellStyle, CellAccessory = cellAccessory
				, ImageName = "Images/Plants.png", Heading = "Plants", SubHeading = "Green plants" });
			tableItems.Add (tGroup);
			
			tableSource = new TableSource(tableItems);
		}
	}
}

