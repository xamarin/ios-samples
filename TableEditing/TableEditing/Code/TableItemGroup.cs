using System;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace TableEditing.Code {
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
		List<TableItem> items = new List<TableItem> ();
	}
}