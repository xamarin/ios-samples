using System;
using System.Collections.Generic;
using Example_TableAndCellStyles.Code.CustomCells;
using MonoTouch.UIKit;
using Example_TableAndCellStyles.Code;
using MonoTouch.Foundation;
using System.IO;

namespace Example_TableAndCellStyles.Screens.iPhone.CustomCells
{
	public class CustomCell12TableScreen : UITableViewController
	{
		protected CustomCellTableSource tableSource;
		protected string cellType;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public CustomCell12TableScreen (IntPtr handle) : base(handle)
		{

		}

		[Export("initWithCoder:")]
		public CustomCell12TableScreen (NSCoder coder) : base(coder)
		{
		}

		/// <summary>
		/// You specify the table style in the constructor when using a UITableViewController
		/// </summary>
		public CustomCell12TableScreen ()
			: base (UITableViewStyle.Plain)
		{

		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Custom Cells";
			
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
			tGroup.Items.Add (new TableItem() { ImageName = "Images/Beach.png"
				, Heading = "Fiji", SubHeading = "A nice beach" });
			tGroup.Items.Add (new TableItem() { ImageName = "Images/Shanghai.png"
				, Heading = "Beijing", SubHeading = "AKA Shanghai" });
			tableItems.Add (tGroup);
			
			// Section 2
			tGroup = new TableItemGroup() { Name = "Other" };
			tGroup.Items.Add (new TableItem() { ImageName = "Images/Seeds.png"
				, Heading = "Seedlings", SubHeading = "Tiny Plants" });
			tGroup.Items.Add (new TableItem() { ImageName = "Images/Plants.png"
				, Heading = "Plants", SubHeading = "Green plants" });
			tableItems.Add (tGroup);
			
			// For custom cells, comment out the first and uncomment the second.
			tableSource = new CustomCellTableSource(tableItems);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		public class CustomCellTableSource : TableSource
		{
			// we need to keep a list of our cell controllers, since the table only has 
			// references to the cells, we'll link the two via a TickCount value
			protected Dictionary<int, CustomCellController2> cellControllers = new Dictionary<int, CustomCellController2>();
			
			public CustomCellTableSource (List<TableItemGroup> items) : base(items)
			{
				cellIdentifier = "MyCustomCell1";
			}
			
			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 85;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				// declare vars
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
				TableItem item = tableItems[indexPath.Section].Items[indexPath.Row];
				CustomCellController2 cellController = null;
				
				// if there are no cells to reuse, create a new one
				if (cell == null) { 
					cellController = new CustomCellController2();
					cell = cellController.Cell;
					cell.Tag = Environment.TickCount;
					cellControllers[cell.Tag] = cellController;
				} else // if we did get one, we also need to lookup the controller
				{
					cellController = cellControllers[cell.Tag];
				}
				
				// set the properties on the cell
				cellController.Heading = item.Heading;
				cellController.SubHeading = item.SubHeading;
				
				// if the item has a valid image
				if(!string.IsNullOrEmpty(item.ImageName)) {
					if(File.Exists(item.ImageName))
						cellController.Image = UIImage.FromBundle(item.ImageName);
				}
								
				return cell;				
			}
		}

	}
}

