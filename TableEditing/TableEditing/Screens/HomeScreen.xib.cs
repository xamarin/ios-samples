using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using TableEditing.Code;

namespace TableEditing.Screens {
	public partial class HomeScreen : UIViewController {
		protected TableSource tableSource;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public HomeScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public HomeScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public HomeScreen () : base("HomeScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			btnDone.Enabled = false;

			CreateTableItems ();
			tblMain.Source = tableSource;
			
			//---- toggle the table's editing mode when the edit button is clicked
			btnEdit.Clicked += (s, e) => {
				tblMain.SetEditing (true, true);
				btnEdit.Enabled = false;
				btnDone.Enabled = true;
			};
			btnDone.Clicked += (sender, e) => {
				tblMain.SetEditing (false, true);
				btnEdit.Enabled = true;
				btnDone.Enabled = false;
			};
		}
		
		/// <summary>
		/// Creates a set of table items.
		/// </summary>
		protected void CreateTableItems ()
		{
			List<TableItemGroup> tableItems = new List<TableItemGroup> ();
			
			//---- declare vars
			TableItemGroup tableGroup;
			
			//---- Section 1
			tableGroup = new TableItemGroup() { Name = "Places" };
			tableGroup.Items.Add (new TableItem() { ImageName = "Images/Beach.png", Heading = "Fiji", SubHeading = "A nice beach" });
			tableGroup.Items.Add (new TableItem() { ImageName = "Images/Shanghai.png", Heading = "Beijing", SubHeading = "AKA Shanghai" });
			tableItems.Add (tableGroup);
			
			//---- Section 2
			tableGroup = new TableItemGroup() { Name = "Other" };
			tableGroup.Items.Add (new TableItem() { ImageName = "Images/Seeds.png", Heading = "Seedlings", SubHeading = "Tiny Plants" });
			tableGroup.Items.Add (new TableItem() { ImageName = "Images/Plants.png", Heading = "Plants", SubHeading = "Green plants" });
			tableItems.Add (tableGroup);
			
			tableSource = new TableSource(tableItems);
		}
	}
}