using System;
using System.Collections.Generic;

namespace FontList.Code {
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class NavItemGroup
	{
		public string Name { get; set; }

		public string Footer { get; set; }

		public List<NavItem> Items { get; set; }

		public NavItemGroup ()
		{
			Items = new List<NavItem> ();
		}

		public NavItemGroup (string name)
		{
			Name = name;
			Items = new List<NavItem> ();
		}
	}
}
