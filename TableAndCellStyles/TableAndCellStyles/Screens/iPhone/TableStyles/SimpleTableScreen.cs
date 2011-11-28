using System;
using System.Collections.Generic;
using Example_TableAndCellStyles.Code;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_TableAndCellStyles.Screens.iPhone.TableStyles
{
	public class SimpleTableScreen : UITableViewController
	{
		protected TableSource tableSource;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public SimpleTableScreen (IntPtr handle) : base(handle)
		{

		}

		[Export("initWithCoder:")]
		public SimpleTableScreen (NSCoder coder) : base(coder)
		{
		}

		/// <summary>
		/// You specify the table style in the constructor when using a UITableViewController
		/// </summary>
		public SimpleTableScreen (UITableViewStyle tableStyle) : base (tableStyle)
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if(TableView.Style == UITableViewStyle.Grouped)
				Title = "Grouped Table";
			else if (TableView.Style == UITableViewStyle.Plain)
				Title = "Plain Table";
			
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
			tGroup = new TableItemGroup() { Name = "Section 0 Header", Footer = "Section 0 Footer" };
			tGroup.Items.Add (new TableItem("Row 0"));
			tGroup.Items.Add (new TableItem("Row 1"));
			tGroup.Items.Add (new TableItem("Row 2"));
			tableItems.Add (tGroup);
			
			// Section 2
			tGroup = new TableItemGroup() { Name = "Section 1 Header", Footer = "Section 1 Footer" };
			tGroup.Items.Add (new TableItem("Row 0"));
			tGroup.Items.Add (new TableItem("Row 1"));
			tGroup.Items.Add (new TableItem("Row 2"));
			tableItems.Add (tGroup);
			
			// Section 3
			tGroup = new TableItemGroup() { Name = "Section 2 Header", Footer = "Section 2 Footer" };
			tGroup.Items.Add (new TableItem("Row 0"));
			tGroup.Items.Add (new TableItem("Row 1"));
			tGroup.Items.Add (new TableItem("Row 2"));
			tableItems.Add (tGroup);
			
			// For custom cells, comment out the first and uncomment the second.
			tableSource = new TableSource (tableItems);
		}
	}
}

