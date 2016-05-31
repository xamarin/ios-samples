using System;

using AudioUnit;
using CoreAudioKit;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace FilterDemoFramework {
	public partial class FilterDemoViewController : AUViewController, IFilterViewDelegate, IAUAudioUnitFactory {
		static readonly NSString cutoffKey = (NSString)"cutoff";
		static readonly NSString resonanceKey = (NSString)"resonance";

		AUParameter cutoffParameter;
		AUParameter resonanceParameter;
		AUParameterObserverToken parameterObserverToken;

		AUv3FilterDemo audioUnit;
		public AUv3FilterDemo AudioUnit {
			get {
				return audioUnit;
			}
			set {
				audioUnit = value;
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (IsViewLoaded)
						ConnectViewWithAU ();
				});
			}
		}

		[Export("initWithCoder:")]
		public FilterDemoViewController (NSCoder coder) : base(coder)
		{
		}

		[Export ("initWithNibName:bundle:")]
		public FilterDemoViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}

		void UpdateFilterViewFrequencyAndMagnitudes ()
		{
			var au = AudioUnit;
			if (au == null)
				return;

			var frequencies = filterView.GetFrequencyData ();
			var magnitudes = au.GetMagnitudes (frequencies);
			filterView.SetMagnitudes (magnitudes);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			filterView.Delegate = this;

			if (AudioUnit == null)
				return;

			ConnectViewWithAU ();
		}

		public void ResonanceChanged (FilterView filterView, float resonance)
		{
			resonanceParameter.SetValue (resonance, parameterObserverToken);
			resonanceLabel.Text = resonanceParameter.GetString (resonance);
			UpdateFilterViewFrequencyAndMagnitudes ();
		}

		public void FrequencyChanged (FilterView filterView, float frequency)
		{
			cutoffParameter.SetValue (frequency, parameterObserverToken);
			frequencyLabel.Text = cutoffParameter.GetString (frequency);
			UpdateFilterViewFrequencyAndMagnitudes ();
		}

		public void DataChanged (FilterView filterView)
		{
			UpdateFilterViewFrequencyAndMagnitudes ();
		}

		[Export ("createAudioUnitWithComponentDescription:error:")]
		public AUAudioUnit CreateAudioUnit (AudioComponentDescription desc, out NSError error)
		{
			AudioUnit = new AUv3FilterDemo (desc, 0, out error);
			return AudioUnit;
		}

		[Export("beginRequestWithExtensionContext:")]
		public void BeginRequestWithExtensionContext (NSExtensionContext context)
		{
		}

		void ConnectViewWithAU ()
		{
			var au = AudioUnit;
			if (au == null)
				return;
			
			var paramTree = au.ParameterTree;
			if (paramTree == null)
				return;

			cutoffParameter = (AUParameter)paramTree.ValueForKey (cutoffKey);
			resonanceParameter = (AUParameter)paramTree.ValueForKey (resonanceKey);
		
			parameterObserverToken = paramTree.CreateTokenByAddingParameterObserver ((address, value) =>
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (address == cutoffParameter.Address) {
						filterView.Frequency = value;
						frequencyLabel.Text = cutoffParameter.GetString (null);
					} else if (address == resonanceParameter.Address) {
						filterView.Resonance = value;
						resonanceLabel.Text = resonanceParameter.GetString (null);
					}

					UpdateFilterViewFrequencyAndMagnitudes ();
				})
			);

			filterView.Frequency = cutoffParameter.Value;
			filterView.Resonance = resonanceParameter.Value;
		}
	}
}
