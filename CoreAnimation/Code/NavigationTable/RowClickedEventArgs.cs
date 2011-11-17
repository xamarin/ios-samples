using System;
namespace Example_CoreAnimation.Code.NavigationTable
{
	public class RowClickedEventArgs : EventArgs
	{
		public NavItem Item { get; set; }
		
		public RowClickedEventArgs(NavItem item) : base()
		{ this.Item = item; }
	}
}

