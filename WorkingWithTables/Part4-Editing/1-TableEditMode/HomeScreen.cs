using System;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace BasicTable {
	public class HomeScreen : UIViewController {
		UITableView table;
		TableSource tableSource;
		TableDelegate tableDelegate;

		public HomeScreen ()
		{	
		}
		
		UIBarButtonItem edit, done, @add;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			table = new UITableView(View.Bounds); // defaults to Plain style
			table.AutoresizingMask = UIViewAutoresizing.All;
			List<TableItem> tableItems = new List<TableItem>();
			
			// credit for images and content
			// http://en.wikipedia.org/wiki/List_of_culinary_vegetables
			tableItems.Add (new TableItem("Vegetables") { SubHeading="65 items", ImageName="Vegetables.jpg"});
			tableItems.Add (new TableItem("Fruits") { SubHeading="17 items", ImageName="Fruits.jpg"});
			tableItems.Add (new TableItem("Flower Buds") { SubHeading="5 items", ImageName="Flower Buds.jpg"});
			tableItems.Add (new TableItem("Legumes") { SubHeading="33 items", ImageName="Legumes.jpg"});
			tableItems.Add (new TableItem("Bulbs") { SubHeading="18 items", ImageName="Bulbs.jpg"});
			tableItems.Add (new TableItem("Tubers") { SubHeading="43 items", ImageName="Tubers.jpg"});
			tableSource = new TableSource(tableItems, this);
			table.Source = tableSource;

			// To replace the standard edit button with a "Hi" button, uncomment out
			// these lines
			 tableDelegate = new TableDelegate ();
			 table.Delegate = tableDelegate;

			
			@add = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s,e)=>{
				tableItems.Insert (0, new TableItem ("(inserted)") { SubHeading="0 items"}); // default new row
				table.InsertRows (new NSIndexPath[] { NSIndexPath.FromRowSection(0,0) }, UITableViewRowAnimation.Fade);
			});

			done = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s,e)=>{
				table.SetEditing (false, true);
				NavigationItem.LeftBarButtonItem = @add;
				NavigationItem.RightBarButtonItem = edit;
			});

			edit = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (s,e)=>{
				if (table.Editing)
					table.SetEditing (false, true); // if we've half-swiped a row

				table.SetEditing (true, true);
				NavigationItem.LeftBarButtonItem = null;
				NavigationItem.RightBarButtonItem = done;
				
			});

			NavigationItem.RightBarButtonItem = edit;
			NavigationItem.LeftBarButtonItem = @add;


			
			Add (table);
		}
	}
}