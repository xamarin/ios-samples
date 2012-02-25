using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;
using System.Diagnostics;
using System.IO;
using MonoTouch.AudioToolbox;

namespace Sound
{
	public partial class SoundViewController : UIViewController
	{
		// declarations
		AVAudioRecorder recorder;
		AVPlayer player;
		NSDictionary settings;
		Stopwatch stopwatch = null;
		NSUrl audioFilePath = null;
		
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SoundViewController ()
			: base (UserInterfaceIdiomIsPhone ? "SoundViewController_iPhone" : "SoundViewController_iPad", null)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.RecordingStatusLabel.Text = "";
			this.LengthOfRecordingLabel.Text = "";
			
			// start recording wireup
			this.StartRecordingButton.TouchUpInside += (sender, e) => {
				Console.WriteLine("Begin Recording");
				
				this.PrepareAudioRecording();
				this.recorder.Record();
				
				this.stopwatch = new Stopwatch();
				this.stopwatch.Start();
				this.LengthOfRecordingLabel.Text = "";
				this.RecordingStatusLabel.Text = "Recording";
				this.StartRecordingButton.Enabled = false;
				this.StopRecordingButton.Enabled = true;
				this.PlayRecordedSoundButton.Enabled = false;
			};
			
			// stop recording wireup
			this.StopRecordingButton.TouchUpInside += (sender, e) => {
				this.recorder.Stop();
				
				recorder.FinishedRecording += delegate {
					recorder.Dispose();
					Console.WriteLine("Done Recording");
				};
				
				this.LengthOfRecordingLabel.Text = string.Format("{0:hh\\:mm\\:ss}", this.stopwatch.Elapsed);
				this.stopwatch.Stop();
				this.RecordingStatusLabel.Text = "";
				this.StartRecordingButton.Enabled = true;
				this.StopRecordingButton.Enabled = false;
				this.PlayRecordedSoundButton.Enabled = true;
			};
			
			// play recorded sound wireup
			this.PlayRecordedSoundButton.TouchUpInside += (sender, e) => {
				try {
					
					Console.WriteLine("Playing Back Recording " + this.audioFilePath.ToString());

					// The following line prevents the audio from stopping 
					// when the device autolocks. will also make sure that it plays, even
					// if the device is in mute
					AudioSession.Category = AudioSessionCategory.MediaPlayback;
					AudioSession.RoutingOverride = AudioSessionRoutingOverride.Speaker;
					
					this.player = new AVPlayer (this.audioFilePath);
					this.player.Play();
										
				} catch (Exception ex) {
					Console.WriteLine("There was a problem playing back audio: ");
					Console.WriteLine(ex.Message);
				}

			};
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		protected void PrepareAudioRecording()
		{
            //Declare string for application temp path and tack on the file extension
			string fileName = string.Format ("Myfile{0}.aac", DateTime.Now.ToString ("yyyyMMddHHmmss"));
			string tempRecording = NSBundle.MainBundle.BundlePath + "/../tmp/" + fileName;
			           
            Console.WriteLine(tempRecording);
			this.audioFilePath = NSUrl.FromFilename(tempRecording);
			
 			//set up the NSObject Array of values that will be combined with the keys to make the NSDictionary
			NSObject[] values = new NSObject[]
            {    
                NSNumber.FromFloat(44100.0f),
                NSNumber.FromInt32((int)MonoTouch.AudioToolbox.AudioFormatType.MPEG4AAC),
                NSNumber.FromInt32(1),
                NSNumber.FromInt32((int)AVAudioQuality.High)
            };
			//Set up the NSObject Array of keys that will be combined with the values to make the NSDictionary
            NSObject[] keys = new NSObject[]
            {
                AVAudioSettings.AVSampleRateKey,
                AVAudioSettings.AVFormatIDKey,
                AVAudioSettings.AVNumberOfChannelsKey,
                AVAudioSettings.AVEncoderAudioQualityKey
            };			
			//Set Settings with the Values and Keys to create the NSDictionary
			settings = NSDictionary.FromObjectsAndKeys (values, keys);
			
			//Set recorder parameters
			NSError error;
			recorder = AVAudioRecorder.ToUrl(this.audioFilePath, settings, out error);
			if (recorder == null){
				Console.WriteLine (error);
				return;
			}
			
			//Set Recorder to Prepare To Record
			recorder.PrepareToRecord(); 
			
		}
	}
}

