using System;
using CoreGraphics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
			table = new UITableView(View.Bounds); // defaults to Plain style
			table.AutoresizingMask = UIViewAutoresizing.All;
			
			// Credit for test data to 
			// http://en.wikipedia.org/wiki/List_of_culinary_vegetables
			var lines = File.ReadLines("VegeData.txt");
			List<string> veges = new List<string>();
			foreach (var l in lines) {
				veges.Add (l);
			}
			veges.Sort ((x,y) => {return x.CompareTo (y);});
			string[] tableItems = veges.ToArray();


			table.Source = new TableSource(tableItems,this);
			Add (table);
		}
	}
}