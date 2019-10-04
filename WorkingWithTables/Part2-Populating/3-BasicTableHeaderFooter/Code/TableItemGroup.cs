using System;
using System.Collections.Generic;
using UIKit;

namespace BasicTable {
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class TableItemGroup {
		public string Name { get; set; }
	
		public string Footer { get; set; }
	
		public List<TableItem> Items
		{
			get { return items; }
			set { items = value; }
		}
		protected List<TableItem> items = new List<TableItem> ();
	}
}