using System;

using UIKit;
using Speech;
using AVFoundation;
using Foundation;

namespace SpeakToMe
{
	public partial class ViewController : UIViewController, ISFSpeechRecognizerDelegate
	{
		readonly SFSpeechRecognizer speechRecognizer = new SFSpeechRecognizer (new NSLocale ("en-US"));
		readonly AVAudioEngine audioEngine = new AVAudioEngine ();
		SFSpeechAudioBufferRecognitionRequest recognitionRequest;
		SFSpeechRecognitionTask recognitionTask;

		[Outlet ("textView")]
		UITextView textView { get; set; }

		[Outlet ("recordButton")]
		UIButton recordButton { get; set; }

		protected ViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Disable the record buttons until authorization has been granted.
			recordButton.Enabled = false;
		}

		public override void ViewDidAppear (bool animated)
		{
			speechRecognizer.Delegate = this;
			SFSpeechRecognizer.RequestAuthorization (authStatus => {
				// The callback may not be called on the main thread. Add an
				// operation to the main queue to update the record button's state.
				NSOperationQueue.MainQueue.AddOperation (() => {
					switch (authStatus) {
					case SFSpeechRecognizerAuthorizationStatus.Authorized:
						recordButton.Enabled = true;
						break;

					case SFSpeechRecognizerAuthorizationStatus.Denied:
						recordButton.Enabled = false;
						recordButton.SetTitle ("User denied access to speech recognition", UIControlState.Disabled);
						break;

					case SFSpeechRecognizerAuthorizationStatus.Restricted:
						recordButton.Enabled = false;
						recordButton.SetTitle ("Speech recognition restricted on this device", UIControlState.Disabled);
						break;

					case SFSpeechRecognizerAuthorizationStatus.NotDetermined:
						recordButton.Enabled = false;
						recordButton.SetTitle ("Speech recognition not yet authorized", UIControlState.Disabled);
						break;
					}
				});
			});
		}

		void StartRecording ()
		{
			// Cancel the previous task if it's running.
			recognitionTask?.Cancel ();
			recognitionTask = null;

			var audioSession = AVAudioSession.SharedInstance ();
			NSError err;
			err = audioSession.SetCategory (AVAudioSessionCategory.Record);
			audioSession.SetMode (AVAudioSession.ModeMeasurement, out err); // TODO: request overload
			err = audioSession.SetActive (true, AVAudioSessionSetActiveOptions.NotifyOthersOnDeactivation);

			// Configure request so that results are returned before audio recording is finished
			recognitionRequest = new SFSpeechAudioBufferRecognitionRequest {
				ShouldReportPartialResults = true
			};

			var inputNode = audioEngine.InputNode;
			if (inputNode == null)
				throw new InvalidProgramException ("Audio engine has no input node");

			// A recognition task represents a speech recognition session.
			// We keep a reference to the task so that it can be cancelled.
			recognitionTask = speechRecognizer.GetRecognitionTask (recognitionRequest, (result, error) => {
				var isFinal = false;
				if (result != null) {
					textView.Text = result.BestTranscription.FormattedString;
					isFinal = result.Final;
				}

				if (error != null || isFinal) {
					audioEngine.Stop ();
					inputNode.RemoveTapOnBus (0);
					recognitionRequest = null;
					recognitionTask = null;
					recordButton.Enabled = true;
					recordButton.SetTitle ("Start Recording", UIControlState.Normal);
				}
			});

			var recordingFormat = inputNode.GetBusOutputFormat (0);
			inputNode.InstallTapOnBus (0, 1024, recordingFormat, (buffer, when) => {
				recognitionRequest?.Append (buffer);
			});

			audioEngine.Prepare ();
			audioEngine.StartAndReturnError (out err);
			textView.Text = "(Go ahead, I'm listening)";
		}

		#region ISFSpeechRecognizerDelegate

		[Export ("speechRecognizer:availabilityDidChange:")]
		public void AvailabilityDidChange (SFSpeechRecognizer speechRecognizer, bool available)
		{
			if (available) {
				recordButton.Enabled = true;
				recordButton.SetTitle ("Start Recording", UIControlState.Normal);
			} else {
				recordButton.Enabled = false;
				recordButton.SetTitle ("Recognition not available", UIControlState.Disabled);
			}
		}

		#endregion

		#region Interface Builder actions

		[Action ("recordButtonTapped")]
		void RecordButtonTapped ()
		{
			if (audioEngine.Running) {
				audioEngine.Stop ();
				recognitionRequest?.EndAudio ();
				recordButton.Enabled = false;
				recordButton.SetTitle ("Stopping", UIControlState.Disabled);
			} else {
				StartRecording ();
				recordButton.SetTitle ("Stop recording", UIControlState.Normal);
			}
		}

		#endregion
	}
}
