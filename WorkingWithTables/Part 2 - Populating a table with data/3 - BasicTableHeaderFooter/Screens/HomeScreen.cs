using System;
using CoreGraphics;
using System.IO;
using System.Collections.Generic;
using UIKit;

namespace BasicTable {
	public class HomeScreen : UIViewController {
		UITableView table;

		public HomeScreen ()
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			table = new UITableView (View.Bounds, UITableViewStyle.Grouped);
			table.AutoresizingMask = UIViewAutoresizing.All;
			CreateTableItems();
			Add (table);
		}

		protected void CreateTableItems ()
		{
			List<TableItem> veges = new List<TableItem>();
	
			// Credit for test data to 
			// http://en.wikipedia.org/wiki/List_of_culinary_vegetables
			var lines = File.ReadLines("VegeData2.txt");
			foreach (var line in lines) {
				var vege = line.Split(',');
				veges.Add (new TableItem(vege[1]) {SubHeading=vege[0]} );
			}

			table.Source = new TableSource(veges, this);
		}
	}
}