using System;

using Foundation;
using UIKit;

namespace MultichannelMixer
{
	public partial class MultichannelMixerViewController : UIViewController
	{
		public MultichannelMixerViewController (IntPtr handle) : base (handle)
		{
			Mixer = new MultichannelMixerController ();
		}

		public MultichannelMixerController Mixer { get; set; }

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var greenImage = new UIImage ("green_button.png").StretchableImage (12, 0);
			var redImage = new UIImage ("red_button.png").StretchableImage (12, 0);

			startButton.SetBackgroundImage (greenImage, UIControlState.Normal);
			startButton.SetBackgroundImage (redImage, UIControlState.Selected);

			SetUIDefaults ();
		}

		#endregion

		void SetUIDefaults ()
		{
			// set the mixers values according to the UI state
			Mixer.EnableInput (0, true);
			Mixer.EnableInput (1, true);
			Mixer.SetInputVolume (0, bus0VolumeSlider.Value);
			Mixer.SetInputVolume (1, bus1VolumeSlider.Value);
			Mixer.SetOutputVolume (outputVolumeSlider.Value);
		}

		partial void doSomethingAction (NSObject sender)
		{
			if (Mixer.IsPlaying) {
				Mixer.Stop ();
				startButton.Selected = false;
			} else {
				Mixer.Start ();
				startButton.Selected = true;
			}
		}

		partial void setInputVolume (NSObject sender)
		{
			var slider = (UISlider) sender;
			Mixer.SetInputVolume ((int)slider.Tag, slider.Value);
		}

		partial void setOutputVolume (NSObject sender)
		{
			var slider = (UISlider) sender;
			Mixer.SetOutputVolume (slider.Value);
		}

		partial void enableInput (NSObject sender)
		{
			var ctrl = (UISwitch) sender;

			switch (ctrl.Tag) {
			case 0:
				bus0VolumeSlider.Enabled = ctrl.On;
				break;
			case 1:
				bus1VolumeSlider.Enabled = ctrl.On;
				break;
			}

			Mixer.EnableInput ((int)ctrl.Tag, ctrl.On);
		}

		// called if we've been interrupted and if we're playing, stop
		public void StopForInterruption ()
		{
			if (Mixer.IsPlaying) {
				Mixer.Stop ();
				startButton.Selected = false;
			}
		}
	}
}

