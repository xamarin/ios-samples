using System;
using UIKit;
using Foundation;

namespace AudioTapProcessor
{
	[Register ("SettingsViewController")]
	public class SettingsViewController : UITableViewController
	{
		[Outlet]
		UISwitch EnabledSwitch { get; set; }

		[Outlet]
		UISlider CenterFrequencySlider { get; set; }

		[Outlet]
		UISlider BandwidthSlider { get; set; }

		public MainViewController Controller { get; set; }

		public bool EnabledSwitchValue {
			get {
				ForceLoadView ();
				return EnabledSwitch.On;
			}
			set {
				ForceLoadView ();
				EnabledSwitch.On = value;
			}
		}

		public float CenterFrequencySliderValue {
			get {
				ForceLoadView ();
				return CenterFrequencySlider.Value;
			}
			set {
				ForceLoadView ();
				CenterFrequencySlider.Value = value;
			}
		}

		public float BandwidthSliderValue {
			get {
				ForceLoadView ();
				return BandwidthSlider.Value;
			}
			set {
				ForceLoadView ();
				BandwidthSlider.Value = value;
			}
		}

		public SettingsViewController (IntPtr handle)
			: base (handle)
		{
		}

		void ForceLoadView ()
		{
			if (EnabledSwitch == null || CenterFrequencySlider == null || BandwidthSlider == null)
				LoadView ();
		}

		#region Actions

		[Export ("updateEnabledSwitchValue:")]
		void UpdateEnabledSwitchValue (NSObject sender)
		{
			Controller.DidUpdateEnabledSwitchValue (EnabledSwitch.On);
		}

		[Export ("updateCenterFrequencySliderValue:")]
		void UpdateCenterFrequencySliderValue (NSObject sender)
		{
			Controller.DidUpdateCenterFrequencySliderValue (CenterFrequencySlider.Value);
		}

		[Export ("updateBandwidthSliderSliderValue:")]
		void UpdateBandwidthSliderSliderValue (NSObject sender)
		{
			Controller.DidUpdateBandwidthSliderValue (BandwidthSlider.Value);
		}

		#endregion
	}
}

