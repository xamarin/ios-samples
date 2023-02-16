using Foundation;
using System;
using UIKit;

namespace UICatalog {
	public partial class SwitchesViewController : UITableViewController {
		public SwitchesViewController (IntPtr handle) : base (handle) { }

		partial void DefaultValueChanged (NSObject sender)
		{
			Console.WriteLine ($"A 'default' switch changed its value: {defaultSwitch.On}.");
		}

		partial void TintedValueChanged (NSObject sender)
		{
			Console.WriteLine ($"A 'tinted' switch changed its value: {tintedSwitch.On}.");
		}
	}
}
