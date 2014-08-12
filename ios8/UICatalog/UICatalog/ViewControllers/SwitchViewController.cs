using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class SwitchViewController : UITableViewController
	{
		[Outlet]
		private UISwitch DefaultSwitch { get; set; }

		[Outlet]
		private UISwitch TintedSwitch { get; set; }

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

		private void ConfigureDefaultSwitch()
		{
			DefaultSwitch.SetState (true, animated: false);
			DefaultSwitch.ValueChanged += OnSwitchValueChange;
		}

		private void ConfigureTintedSwitch()
		{
			TintedSwitch.TintColor = ApplicationColors.Blue;
			TintedSwitch.OnTintColor = ApplicationColors.Green;
			TintedSwitch.ThumbTintColor = ApplicationColors.Purple;

			TintedSwitch.ValueChanged += OnSwitchValueChange;
		}

		private void OnSwitchValueChange(object sender, EventArgs e)
		{
			Console.WriteLine ("A switch changed its value: {0}.", sender);
		}
	}
}
