using System;
using System.Collections.Generic;

namespace CoreAnimationExample
{
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class NavItemGroup
	{
		List<NavItem> items = new List<NavItem> ();

		public string Name { get; set; }

		public string Footer { get; set; }

		public List<NavItem> Items {
			get { return  items; }
			set { items = value; }
		}

		public NavItemGroup ()
		{
		}

		public NavItemGroup (string name)
		{
			Name = name;
		}
	}
}
