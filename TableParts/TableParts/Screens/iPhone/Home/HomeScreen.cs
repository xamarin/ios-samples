using System;
using UIKit;
using Foundation;
using System.Collections.Generic;

namespace Example_TableParts.Screens.iPhone.Home
{
	public class HomeScreen : UITableViewController
	{
		protected TableSource tableSource;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public HomeScreen (IntPtr handle) : base(handle)
		{

		}

		[Export("initWithCoder:")]
		public HomeScreen (NSCoder coder) : base(coder)
		{
		}

		public HomeScreen () : base (UITableViewStyle.Grouped)
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.CreateTableItems ();
			this.TableView.Source = tableSource;

		}

		// Creates a set of table items.
		protected void CreateTableItems ()
		{
			List<TableItemGroup> tableItems = new List<TableItemGroup> ();
			
			// declare vars
			TableItemGroup tGroup;
			
			// Section 1
			tGroup = new TableItemGroup() { Name = "Section 0 Header", Footer = "Section 0 Footer" };
			tGroup.Items.Add ("Row 0");
			tGroup.Items.Add ("Row 1");
			tGroup.Items.Add ("Row 2");
			tableItems.Add (tGroup);
			
			// Section 2
			tGroup = new TableItemGroup() { Name = "Section 1 Header", Footer = "Section 1 Footer" };
			tGroup.Items.Add ("Row 0");
			tGroup.Items.Add ("Row 1");
			tGroup.Items.Add ("Row 2");
			tableItems.Add (tGroup);
			
			// Section 3
			tGroup = new TableItemGroup() { Name = "Section 2 Header", Footer = "Section 2 Footer" };
			tGroup.Items.Add ("Row 0");
			tGroup.Items.Add ("Row 1");
			tGroup.Items.Add ("Row 2");
			tableItems.Add (tGroup);
			
			tableSource = new TableSource(tableItems);
		}
	}
}

