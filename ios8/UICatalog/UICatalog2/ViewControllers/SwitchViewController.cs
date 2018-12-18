using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("SwitchViewController")]
	public class SwitchViewController : UITableViewController
	{
		[Outlet]
		UISwitch DefaultSwitch { get; set; }

		[Outlet]
		UISwitch TintedSwitch { get; set; }

		public SwitchViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureDefaultSwitch ();
			ConfigureTintedSwitch ();
		}

		void ConfigureDefaultSwitch ()
		{
			DefaultSwitch.SetState (true, false);
			DefaultSwitch.ValueChanged += OnSwitchValueChange;
		}

		void ConfigureTintedSwitch ()
		{
			TintedSwitch.TintColor = ApplicationColors.Blue;
			TintedSwitch.OnTintColor = ApplicationColors.Green;
			TintedSwitch.ThumbTintColor = ApplicationColors.Purple;

			TintedSwitch.ValueChanged += OnSwitchValueChange;
		}

		void OnSwitchValueChange (object sender, EventArgs e)
		{
			Console.WriteLine ("A switch changed its value: {0}.", sender);
		}
	}
}
