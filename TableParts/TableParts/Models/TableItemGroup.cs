using System;
using System.Collections.Generic;

namespace TableParts.Models {
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class TableItemGroup {
		public string Name { get; set; }

		public string Footer { get; set; }

		public List<string> Items { get; private set; } = new List<string> ();
	}
}
