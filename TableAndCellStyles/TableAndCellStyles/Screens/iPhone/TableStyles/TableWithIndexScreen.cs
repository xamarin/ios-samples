using System;
using System.Collections.Generic;
using Example_TableAndCellStyles.Code;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_TableAndCellStyles.Screens.iPhone.TableStyles
{
	public class TableWithIndexScreen : UITableViewController
	{
		protected TableSourceWithIndex tableSource;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public TableWithIndexScreen (IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public TableWithIndexScreen (NSCoder coder) : base(coder)
		{
		}

		/// <summary>
		/// You specify the table style in the constructor when using a UITableViewController
		/// </summary>
		public TableWithIndexScreen (UITableViewStyle tableStyle) : base (tableStyle)
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = "Table with Index";
			
			CreateTableItems ();
			TableView.Source = tableSource;
			
		}

		/// <summary>
		/// Creates a set of table items.
		/// </summary>
		protected void CreateTableItems ()
		{
			List<TableItemGroup> tableItems = new List<TableItemGroup> ();
			Dictionary<string, int> indexSectionMap = new Dictionary<string, int>();
			
			// declare vars
			TableItemGroup tGroup;
			
			// Section
			tGroup = new TableItemGroup() { Name = "A"};
			tGroup.Items.Add (new TableItem("Apple"));
			tGroup.Items.Add (new TableItem("Artichoke"));
			tableItems.Add (tGroup);
			indexSectionMap["A"] = 0;
			
			// Section
			tGroup = new TableItemGroup() { Name = "B"};
			tGroup.Items.Add (new TableItem("Banana"));
			tGroup.Items.Add (new TableItem("Berries"));
			tableItems.Add (tGroup);
			indexSectionMap["B"] = 1;
			
			// Section
			tGroup = new TableItemGroup() { Name = "C"};
			tGroup.Items.Add (new TableItem("Cucumber"));
			tGroup.Items.Add (new TableItem("Cantalopes"));
			tableItems.Add (tGroup);
			indexSectionMap["C"] = 2;
			
			// Section
			tGroup = new TableItemGroup() { Name = "D"};
			tGroup.Items.Add (new TableItem("Daikon"));
			tableItems.Add (tGroup);
			indexSectionMap["D"] = 3;
			
			// Section
			tGroup = new TableItemGroup() { Name = "E"};
			tGroup.Items.Add (new TableItem("Eggplant"));
			tGroup.Items.Add (new TableItem("Elderberry"));
			tableItems.Add (tGroup);
			indexSectionMap["E"] = 4;
			
			// Section
			tGroup = new TableItemGroup() { Name = "F"};
			tGroup.Items.Add (new TableItem("Fig"));
			tableItems.Add (tGroup);
			indexSectionMap["F"] = 5;
			
			// Section
			tGroup = new TableItemGroup() { Name = "G"};
			tGroup.Items.Add (new TableItem("Grape"));
			tGroup.Items.Add (new TableItem("Guava"));
			tableItems.Add (tGroup);
			indexSectionMap["G"] = 6;
			
			// Section
			tGroup = new TableItemGroup() { Name = "H"};
			tGroup.Items.Add (new TableItem("Honeydew"));
			tGroup.Items.Add (new TableItem("Huckleberry"));
			tableItems.Add (tGroup);
			indexSectionMap["H"] = 7;
			
			// Section
			tGroup = new TableItemGroup() { Name = "I"};
			tGroup.Items.Add (new TableItem("Indian Fig"));
			tableItems.Add (tGroup);
			indexSectionMap["I"] = 8;
			
			// Section
			tGroup = new TableItemGroup() { Name = "J"};
			tGroup.Items.Add (new TableItem("Jackfruit"));
			tableItems.Add (tGroup);
			indexSectionMap["J"] = 9;
			
			// Section
			tGroup = new TableItemGroup() { Name = "K"};
			tGroup.Items.Add (new TableItem("Kiwi"));
			tGroup.Items.Add (new TableItem("Kumquat"));
			tableItems.Add (tGroup);
			indexSectionMap["K"] = 10;
			
			
			indexSectionMap["L"] = 10;
			indexSectionMap["M"] = 10;
			indexSectionMap["N"] = 10;
			indexSectionMap["O"] = 10;
			indexSectionMap["P"] = 10;
			indexSectionMap["Q"] = 10;
			indexSectionMap["R"] = 10;
			indexSectionMap["S"] = 10;
			indexSectionMap["T"] = 10;
			indexSectionMap["U"] = 10;
			indexSectionMap["V"] = 10;
			indexSectionMap["W"] = 10;
			indexSectionMap["X"] = 10;
			indexSectionMap["Y"] = 10;
			indexSectionMap["Z"] = 10;
	
			// For custom cells, comment out the first and uncomment the second.
			tableSource = new TableSourceWithIndex(tableItems, indexSectionMap);
		}
	}
}

