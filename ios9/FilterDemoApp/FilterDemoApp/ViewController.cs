using System;

using AudioUnit;
using CoreFoundation;
using FilterDemoFramework;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace FilterDemoApp {
	public partial class ViewController : UIViewController {
		SimplePlayEngine playEngine;
		AUParameter cutoffParameter;
		AUParameter resonanceParameter;
		FilterDemoViewController filterDemoViewController;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var componentDescription = new AudioComponentDescription {
				ComponentType = AudioComponentType.Effect,
				ComponentSubType = 0x666c7472,
				ComponentManufacturer = (AudioComponentManufacturerType)0x44656d6f,
				ComponentFlags = 0,
				ComponentFlagsMask = 0
			};

			AUAudioUnit.RegisterSubclass (
				new Class (typeof(AUv3FilterDemo)),
				componentDescription,
				"Local FilterDemo",
				int.MaxValue
			);

			EmbedPlugInView ();

			playEngine = new SimplePlayEngine ();

			playEngine.SelectEffectWithComponentDescription (componentDescription, ConnectParametersToControls);

			playEngine.AudioUnit.TransportStateBlock = (ref AUHostTransportStateFlags transportStateFlags, ref double currentSamplePosition, ref double cycleStartBeatPosition, ref double cycleEndBeatPosition) => {
				transportStateFlags = AUHostTransportStateFlags.Recording;

				currentSamplePosition = 10.0;
				cycleStartBeatPosition = 0.0;
				cycleEndBeatPosition = 20.0;

				return true;
			};
		}

		void EmbedPlugInView ()
		{
			var storyboard = UIStoryboard.FromName ("MainInterface", NSBundle.MainBundle);
			filterDemoViewController = storyboard.InstantiateInitialViewController () as FilterDemoViewController;

			var view = filterDemoViewController.View;

			if (view == null)
				return;

			AddChildViewController (filterDemoViewController);
			view.Frame = auContainerView.Bounds;

			auContainerView.AddSubview (view);
			filterDemoViewController.DidMoveToParentViewController (this);
		}

		partial void changedCutoff (UISlider sender)
		{
			if (sender != cutoffSlider)
				return;

			cutoffParameter.Value = cutoffSlider.Value;
		}

		partial void changedResonance (UISlider sender)
		{
			if (sender != resonanceSlider)
				return;

			resonanceParameter.Value = resonanceSlider.Value;
		}

		partial void togglePlay (UIButton sender)
		{
			var isPlaying = playEngine.TogglePlay ();
			var titleText = isPlaying ? "Stop" : "Play";
			playButton.SetTitle (titleText, UIControlState.Normal);
		}

		void ConnectParametersToControls ()
		{
			var parameterTree = playEngine.AudioUnit?.ParameterTree;
			if (parameterTree == null)
				return;

			var audioUnit = playEngine.AudioUnit as AUv3FilterDemo;
			filterDemoViewController.AudioUnit = audioUnit;

			cutoffParameter = (AUParameter)parameterTree.ValueForKey ((NSString)"cutoff");
			resonanceParameter = (AUParameter)parameterTree.ValueForKey ((NSString)"resonance");

			parameterTree.CreateTokenByAddingParameterObserver ((address, value) => DispatchQueue.MainQueue.DispatchAsync (() => {
				if (address == cutoffParameter.Address)
					UpdateCutoff ();
				else if (address == resonanceParameter.Address)
					UpdateResonance ();
			}));

			UpdateCutoff ();
			UpdateResonance ();
		}

		void UpdateCutoff ()
		{
			cutoffTextField.Text = cutoffParameter.GetString (null);
			cutoffSlider.Value = cutoffParameter.Value;
		}

		void UpdateResonance ()
		{
			resonanceTextField.Text = resonanceParameter.GetString (null);
			resonanceSlider.Value = resonanceParameter.Value;
		}
	}
}

