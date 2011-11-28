using System;
using System.Collections.Generic;

namespace Example_TableParts
{
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class TableItemGroup
	{
		public string Name { get; set; }
	
		public string Footer { get; set; }
	
		public List<string> Items
		{
			get { return items; }
			set { items = value; }
		}
		protected List<string> items = new List<string> ();
	}
}

