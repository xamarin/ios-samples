using System;
using Foundation;
using UIKit;
using AVFoundation;
using System.Diagnostics;

namespace Sound
{
    public partial class SoundViewController : UIViewController
    {
        // declarations
        AVAudioRecorder recorder;
        AVPlayer player;
        Stopwatch stopwatch = null;
        NSUrl audioFilePath = null;
        NSObject observer;

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public SoundViewController()
            : base(UserInterfaceIdiomIsPhone ? "SoundViewController_iPhone" : "SoundViewController_iPad", null)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            this.RecordingStatusLabel.Text = "";
            this.LengthOfRecordingLabel.Text = "";
			
            // start recording wireup
            this.StartRecordingButton.TouchUpInside += (sender, e) =>
            {
                Console.WriteLine("Begin Recording");

                var session = AVAudioSession.SharedInstance();

                NSError error = null;
                session.SetCategory(AVAudioSession.CategoryRecord, out error);
                if (error != null)
                {
                    Console.WriteLine(error);
                    return;
                }

                session.SetActive(true, out error);
                if (error != null)
                {
                    Console.WriteLine(error);
                    return;
                }
				
                if (!PrepareAudioRecording())
                {
                    RecordingStatusLabel.Text = "Error preparing";
                    return;
                }

                if (!recorder.Record())
                {
                    RecordingStatusLabel.Text = "Error preparing";
                    return;
                }
				
                this.stopwatch = new Stopwatch();
                this.stopwatch.Start();
                this.LengthOfRecordingLabel.Text = "";
                this.RecordingStatusLabel.Text = "Recording";
                this.StartRecordingButton.Enabled = false;
                this.StopRecordingButton.Enabled = true;
                this.PlayRecordedSoundButton.Enabled = false;
            };
			
            // stop recording wireup
            this.StopRecordingButton.TouchUpInside += (sender, e) =>
            {
                this.recorder.Stop();
				
                this.LengthOfRecordingLabel.Text = string.Format("{0:hh\\:mm\\:ss}", this.stopwatch.Elapsed);
                this.stopwatch.Stop();
                this.RecordingStatusLabel.Text = "";
                this.StartRecordingButton.Enabled = true;
                this.StopRecordingButton.Enabled = false;
                this.PlayRecordedSoundButton.Enabled = true;
            };
			
            observer = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, delegate (NSNotification n)
            {
                player.Dispose();
                player = null;
            });
			
            // play recorded sound wireup
            this.PlayRecordedSoundButton.TouchUpInside += (sender, e) =>
            {
                try
                {
                    Console.WriteLine("Playing Back Recording " + this.audioFilePath.ToString());

                    // The following line prevents the audio from stopping 
                    // when the device autolocks. will also make sure that it plays, even
                    // if the device is in mute
                    NSError error = null;
                    AVAudioSession.SharedInstance().SetCategory(AVAudioSession.CategoryPlayback, out error);
                    if (error != null)
                    {
                        throw new Exception(error.DebugDescription);
                    }
                    //AudioSession.Category = AudioSessionCategory.MediaPlayback;
					
                    this.player = new AVPlayer(this.audioFilePath);
                    this.player.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was a problem playing back audio: ");
                    Console.WriteLine(ex.Message);
                }
            };
        }

        public override void ViewDidUnload()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
			
            base.ViewDidUnload();
			
            // Clear any references to subviews of the main view in order to
            // allow the Garbage Collector to collect them sooner.
            //
            // e.g. myOutlet.Dispose (); myOutlet = null;
			
            ReleaseDesignerOutlets();
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }

        bool PrepareAudioRecording()
        {
            //Declare string for application temp path and tack on the file extension
            string fileName = string.Format("Myfile{0}.aac", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string tempRecording = NSBundle.MainBundle.BundlePath + "/../tmp/" + fileName;

            Console.WriteLine(tempRecording);
            this.audioFilePath = NSUrl.FromFilename(tempRecording);

            var audioSettings = new AudioSettings()
            {
                SampleRate = 44100.0f, 
                Format = AudioToolbox.AudioFormatType.MPEG4AAC,
                NumberChannels = 1,
                AudioQuality = AVAudioQuality.High
            };

            //Set recorder parameters
            NSError error;
            recorder = AVAudioRecorder.Create(this.audioFilePath, audioSettings, out error);
            if ((recorder == null) || (error != null))
            {
                Console.WriteLine(error);
                return false;
            }

            //Set Recorder to Prepare To Record
            if (!recorder.PrepareToRecord())
            {
                recorder.Dispose();
                recorder = null;
                return false;
            }

            recorder.FinishedRecording += delegate (object sender, AVStatusEventArgs e)
            {
                recorder.Dispose();
                recorder = null;
                Console.WriteLine("Done Recording (status: {0})", e.Status);
            };

            return true;
        }
    }
}