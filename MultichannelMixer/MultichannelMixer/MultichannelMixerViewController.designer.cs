// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MultichannelMixer
{
	[Register ("MultichannelMixerViewController")]
	partial class MultichannelMixerViewController
	{
		[Outlet]
		MonoTouch.UIKit.UISwitch bus0switch { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider bus0VolumeSlider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch bus1switch { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider bus1VolumeSlider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider outputVolumeSlider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton startButton { get; set; }

		[Action ("doSomethingAction:")]
		partial void doSomethingAction (MonoTouch.Foundation.NSObject sender);

		[Action ("enableInput:")]
		partial void enableInput (MonoTouch.Foundation.NSObject sender);

		[Action ("setInputVolume:")]
		partial void setInputVolume (MonoTouch.Foundation.NSObject sender);

		[Action ("setOutputVolume:")]
		partial void setOutputVolume (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (bus0switch != null) {
				bus0switch.Dispose ();
				bus0switch = null;
			}

			if (bus0VolumeSlider != null) {
				bus0VolumeSlider.Dispose ();
				bus0VolumeSlider = null;
			}

			if (bus1switch != null) {
				bus1switch.Dispose ();
				bus1switch = null;
			}

			if (bus1VolumeSlider != null) {
				bus1VolumeSlider.Dispose ();
				bus1VolumeSlider = null;
			}

			if (outputVolumeSlider != null) {
				outputVolumeSlider.Dispose ();
				outputVolumeSlider = null;
			}

			if (startButton != null) {
				startButton.Dispose ();
				startButton = null;
			}
		}
	}
}
