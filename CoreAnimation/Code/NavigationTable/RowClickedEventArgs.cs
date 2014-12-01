using System;

namespace CoreAnimationExample
{
	public class RowClickedEventArgs : EventArgs
	{
		public NavItem Item { get; set; }

		public RowClickedEventArgs (NavItem item) : base ()
		{
			Item = item;
		}
	}
}

