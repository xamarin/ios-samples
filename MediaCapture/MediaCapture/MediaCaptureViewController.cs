//
// how to capture still images, video and audio using iOS AVFoundation and the AVCAptureSession
//
// This sample handles all of the low-level AVFoundation and capture graph setup required to capture and save media.  This code also exposes the
// capture, configuration and notification capabilities in a more '.Netish' way of programming.  The client code will not need to deal with threads, delegate classes
// buffer management, or objective-C data types but instead will create .NET objects and handle standard .NET events.  The underlying iOS concepts and classes are detailed in
// the iOS developer online help (TP40010188-CH5-SW2).
//
// https://developer.apple.com/library/mac/#documentation/AudioVideo/Conceptual/AVFoundationPG/Articles/04_MediaCapture.html#//apple_ref/doc/uid/TP40010188-CH5-SW2
//
// Enhancements, suggestions and bug reports can be sent to steve.millar@infinitekdev.com
//
using System;
using System.Linq;
using CoreGraphics;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using Foundation;
using UIKit;
using MonoTouch.Dialog;
using MediaPlayer;

namespace MediaCapture
{
	public partial class MediaCaptureViewController : UIViewController
	{
		// settings
		DialogViewController settingsController = null;
		SettingsDialog settingsDialog = null;
		Settings settings = null;

		// media browsing
		DialogViewController browseController = null;
		MediaBrowserDialog browseDialog = null;

		// capture
		CaptureManager captureManager = null;
		bool isCapturing = false;
		DateTime captureStartTime;
		int nextImageIndex = 1;

		// log message
		UITextView textView = null;
		ConcurrentQueue<string> messages = new ConcurrentQueue<string>();
		int textViewXOffset;
		int textViewYOffset;
		int textViewWidth;
		int textViewHeight;

		// image previewer
		UIImageView imageView = null;
		DateTime lastImageWriteTime = DateTime.MinValue;

		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MediaCaptureViewController () : base (UserInterfaceIdiomIsPhone ? "MediaCaptureViewController_iPhone" : "MediaCaptureViewController_iPad", null) {}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload();
			ReleaseDesignerOutlets();
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			if (UserInterfaceIdiomIsPhone)
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			else
				return true;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// load the saved (or default) settings
			settings = new Settings();
			settings.Load();

			SetupButtons();
			SetupTextView();
			SetupImageView();
			LayoutViews();

			// subscribe to the popped controller event
			((RootViewController)NavigationController).ViewControllerPopped += HandleViewControllerPopped;
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			LayoutViews();
		}

		void LayoutViews()
		{
			// calculate the working area for the view(s)
			CGRect workingRect = this.View.Frame;
			workingRect.Height = workingRect.Height - mainToolBar.Frame.Height;

			// figure out padding and sizes
			double maxDimensionPercent = .80;
			double paddingPercent =  settings.ImageCaptureEnabled ? ((1 - maxDimensionPercent) / 3) : ((1 - maxDimensionPercent) / 2);

			bool isPortrait = UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown;
			double heightPercentage;
			if ( isPortrait )
			{
				// lay out the text view on top
				heightPercentage = settings.ImageCaptureEnabled ? maxDimensionPercent/2 : maxDimensionPercent;
				double widthPercentage = maxDimensionPercent;
				textViewWidth = (int)(workingRect.Width * widthPercentage);
				textViewHeight = (int)(workingRect.Height * heightPercentage);
				textViewXOffset = (int)(workingRect.Width * paddingPercent);
				textViewYOffset = (int)(workingRect.Height * paddingPercent);
				textView.Frame = new CGRect( textViewXOffset, textViewYOffset, textViewWidth, textViewHeight );

				// lay out image view (if visible) on the botom
				if ( settings.ImageCaptureEnabled == true )
				{
					int width = (int)(workingRect.Width * widthPercentage);
					int height = (int)(workingRect.Height * heightPercentage);
					int xOffset = (int)(workingRect.Width * paddingPercent);
					int yOffset = (2 * textViewYOffset) + textViewHeight;
					imageView.Frame = new CGRect( xOffset, yOffset, width, height );
					this.imageView.Hidden = false;
				}
				else
				{
					imageView.Frame = new CGRect( 0,0,0,0 );
					this.imageView.Hidden = true;
				}
			}
			else
			{
				// lay out the text view to the left
				heightPercentage = maxDimensionPercent;
				double textWidthPercentage = settings.ImageCaptureEnabled ? maxDimensionPercent * .667 : maxDimensionPercent;
				double imageWidthPercentage = settings.ImageCaptureEnabled ? maxDimensionPercent * .333 : 0.0;
				textViewWidth = (int)(workingRect.Width * textWidthPercentage);
				textViewHeight = (int)(workingRect.Height * heightPercentage);
				textViewXOffset = (int)(workingRect.Width * paddingPercent);
				textViewYOffset = (int)(workingRect.Height * paddingPercent);
				textView.Frame = new CGRect( textViewXOffset, textViewYOffset, textViewWidth, textViewHeight );

				// lay out image view (if visible) to the right
				if ( settings.ImageCaptureEnabled == true )
				{
					int width = (int)(workingRect.Width * imageWidthPercentage);
					int height = (int)(workingRect.Height * heightPercentage);
					int xOffset = (2 * textViewXOffset) + textViewWidth;
					int yOffset = (int)(workingRect.Height * paddingPercent);
					imageView.Frame = new CGRect( xOffset, yOffset, width, height );
					this.imageView.Hidden = false;
				}
				else
				{
					imageView.Frame = new CGRect( 0,0,0,0 );
					this.imageView.Hidden = true;
				}
			}

			// scroll the message text to show mopst recent messages at bottom
			ScrollMessageViewToEnd();
		}

		bool Capturing
		{
			get
			{
				return (captureManager != null && captureManager.IsCapturing);
			}
		}

		void HandleViewControllerPopped (object sender, RootViewController.ViewControllerPoppedEventArgs e)
		{
			if ( e.Controller == settingsController )
			{
				HarvestSettings();
				LayoutViews();
			}
			else if ( e.Controller == browseController )
			{
				try
				{
					// unsubscribe browser events
					browseDialog.MovieFileSelected -= new EventHandler<FileSelectedEventArgs>( handleMediaFileSelected );
					browseDialog.ImageFileSelected -= new EventHandler<FileSelectedEventArgs>( HandleImageFileSelected );
				}
				catch
				{
				}
			}
		}

		void UpdateUI()
		{
			this.buttonSettings.Enabled = !isCapturing;
			this.buttonBrowse.Enabled = !isCapturing;
			this.buttonStartStop.Enabled = !buttonHandlerInProgress;
		}

		void SetupButtons()
		{
			buttonSettings.Clicked += (s,e) =>
			{
				EditSettings();
				UpdateUI ();
			};

			buttonStartStop.Clicked += (s,e) =>
			{
				StartStop();
				UpdateUI();
			};

			buttonBrowse.Clicked += (s,e) =>
			{
				Browse();
				UpdateUI();
			};
		}

		void SetupTextView()
		{
			if ( textView == null )
			{
				textView = new UITextView ( new CGRect(1,1,1,1) );
				textView.Font = UIFont.FromName("Courier", 16);
				textView.ScrollEnabled = true;
				textView.Editable = false;
				textView.BackgroundColor = UIColor.FromRGB(240,240,240);
				this.View.AddSubview (textView);
			}
		}

		void SetupImageView()
		{
			if ( imageView == null )
			{
				imageView = new UIImageView ( new CGRect(1,1,1,1) );
				imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				this.View.AddSubview (imageView);
			}
		}

		bool buttonHandlerInProgress = false;
		void StartStop()
		{
			try
			{
				if (buttonHandlerInProgress)
					return;
				buttonHandlerInProgress = true;

				// the layout call is to handle the possibility that settings changed and the image capture view may now
				// have different visibility
				LayoutViews();
				startStopCapture();
			}
			finally
			{
				buttonHandlerInProgress = false;
			}
			UpdateUI();
		}

		private void startStopCapture()
		{
			if (isCapturing == false)
			{
				try
				{
					Capture();
					if ( isCapturing)
						buttonStartStop.Title = "Stop";
				}
				catch (Exception ex)
				{
					LogMessage( string.Format ("Failed to start capture: {0}", ErrorHandling.GetExceptionDetailedText(ex)) );
				}
			}
			else
			{
				StopCapture();
				buttonStartStop.Title = "Start";
			}
		}

		void Browse()
		{
			browseDialog = new MediaBrowserDialog( Settings.ImageDataPath, Settings.VideoDataPath );
			browseDialog.MovieFileSelected += new EventHandler<FileSelectedEventArgs>( handleMediaFileSelected );
			browseDialog.ImageFileSelected += new EventHandler<FileSelectedEventArgs>( HandleImageFileSelected );
			browseController = new DialogViewController (browseDialog.Menu, true);
			NavigationController.PushViewController (browseController, true);
		}

		void EditSettings()
		{
			settingsDialog = new SettingsDialog( settings );
			settingsController = new DialogViewController (settingsDialog.Menu, true);
			NavigationController.PushViewController (settingsController, true);
			settingsDialog.EnforceDependencies(); // must be called after view controller push
		}

		void HarvestSettings()
		{
			settings = settingsDialog.ResultSettings;
			settings.Save();

			// warn user if the image save is turned on so they don't accidentally fill up their photo library with junk
			if ( settings.ImageCaptureEnabled && settings.SaveCapturedImagesToPhotoLibrary )
			{
				string message = String.Format("You have enabled still image capture and saving to the photo library.  This will save a captured image to your photo library once every {0} seconds", settings.ImageSaveIntervalInSeconds);
				Utilities.ShowMessage( "Warning", message );
			}
		}

		private void Capture()
		{
			// stop capture if it is in progress
			StopCapture();

			// make sure there is something configured for capture before starting up
			if ( !settings.AudioCaptureEnabled && !settings.VideoCaptureEnabled && !settings.ImageCaptureEnabled )
			{
				LogMessage ("No capture devices enabled.  Enable one or more capture types in settings then retry");
				isCapturing = false;
				return;
			}

			// reset variables
			captureStartTime = DateTime.Now;
			nextImageIndex = 1;

			// create new capture manager and subscribe events
			LogMessage("Creating and initializing capture graph");
			captureManager = new CaptureManager(
				settings.CaptureResolution,
				settings.ImageCaptureEnabled,
				settings.AudioCaptureEnabled,
				settings.VideoCaptureEnabled,
				settings.Camera,
				Settings.VideoDataPath,
				settings.MaxMovieDurationInSeconds * 1000,
				settings.AutoRecordNextMovie );

			// subscribe events
			captureManager.MovieSegmentCaptured += new EventHandler<MovieSegmentCapturedEventArgs>( HandleMovieSegmentCaptured );
			captureManager.CaptureError += new EventHandler<CaptureErrorEventArgs>( HandleCaptureError );
			captureManager.ImageCaptured += new EventHandler<ImageCaptureEventArgs>( HandleImageCaptured );

			string errorMessage = null;
			LogMessage("starting capture ...");
			if ( captureManager.StartCapture( out errorMessage ) == false )
			{
				LogMessage( errorMessage );
				return;
			}
			if ( settings.VideoCaptureEnabled || settings.ImageCaptureEnabled)
				LogMessage ( string.Format("capture started at {0} resolution", settings.CaptureResolution));
			if ( settings.AudioCaptureEnabled )
				LogMessage ( string.Format("Capturing audio"));
			if ( settings.VideoCaptureEnabled )
				LogMessage ( string.Format("Capturing video"));
			if ( settings.ImageCaptureEnabled )
				LogMessage ( string.Format("Capturing image samples"));

			isCapturing = true;
		}

		void StopCapture()
		{
			if ( captureManager != null )
			{
				LogMessage("stopping capture...");
				try
				{
					captureManager.StopCapture();

					// unsubscribe events
					captureManager.MovieSegmentCaptured -= new EventHandler<MovieSegmentCapturedEventArgs>( HandleMovieSegmentCaptured );
					captureManager.CaptureError -= new EventHandler<CaptureErrorEventArgs>( HandleCaptureError );
					captureManager.ImageCaptured -= new EventHandler<ImageCaptureEventArgs>( HandleImageCaptured );
					captureManager = null;
				}
				catch
				{
				}
				LogMessage("capture stopped");
			}
			isCapturing = false;
		}

		private void handleMediaFileSelected( object sender, FileSelectedEventArgs args )
		{
			NavigationController.PopToViewController(this,true);

			try
			{
				var player = new MPMoviePlayerController();
				player = new MPMoviePlayerController (NSUrl.FromFilename( args.File ));
				player.AllowsAirPlay = true;
				View.AddSubview(player.View);
				player.SetFullscreen(true, true);
				player.PrepareToPlay();
				player.Play();
			}
			catch ( Exception ex )
			{
				string message = string.Format ("Error during playback of {0}: {1}", Path.GetFileName(args.File), ErrorHandling.GetExceptionDetailedText(ex) );
				LogMessage( message );
			}
		}

		void HandleImageFileSelected( object sender, FileSelectedEventArgs args )
		{
			NavigationController.PopToViewController(this,true);
			UIImage image = UIImage.FromFile( args.File );
			imageView.Image = image;
		}

		void HandleMovieSegmentCaptured( object sender, MovieSegmentCapturedEventArgs args )
		{
			LogMessage( string.Format("Media file captured to '{0}'", Path.GetFileName(args.File)) );
		}

		void HandleCaptureError( object sender, CaptureErrorEventArgs args )
		{
			LogMessage( args.ErrorMessage );
		}

		void HandleImageCaptured( object sender, ImageCaptureEventArgs args )
		{
			imageView.InvokeOnMainThread (() => {
				// render image in preview pane
				imageView.Image = args.Image;

				// conditionally save the image
				bool saveImage = settings.SaveCapturedImagesToPhotoLibrary || settings.SaveCapturedImagesToMyDocuments;
				if ( saveImage )
				{
					// see if it is time to save anothter one
					TimeSpan elapsedTimeSinceLastImageWrite = DateTime.Now.Subtract( lastImageWriteTime );
					if ( elapsedTimeSinceLastImageWrite >= TimeSpan.FromSeconds(settings.ImageSaveIntervalInSeconds) )
					{
						// determine the target save location and save it
						if ( settings.SaveCapturedImagesToPhotoLibrary )
						{
							args.Image.SaveToPhotosAlbum(null);
							LogMessage("Captured image saved to photos library");
						}
						else if ( settings.SaveCapturedImagesToMyDocuments )
						{
							string partialPath = GetNextImageFileName();
							string fullPath = Path.Combine( Settings.ImageDataPath, partialPath );
							byte[] bytes = args.Image.AsJPEG().ToArray();

							// create directory
							string directory = Path.GetDirectoryName( fullPath );
							if ( Directory.Exists( directory ) == false )
								Directory.CreateDirectory( directory );
							// delete file if it exists already.  this is unlikely but possible if the user starts, stops, and starts the capture
							// quickly enough to cause the same DateTome-based folder name to be regenerated
							if ( File.Exists(fullPath) )
								File.Delete( fullPath );

							// write the image to the file
							File.WriteAllBytes(  fullPath, bytes );
							LogMessage(string.Format ("Captured image saved to '{0}'", partialPath));
						}

						// update the last capture time
						lastImageWriteTime = DateTime.Now;
					}
				}
			});
		}

		string GetNextImageFileName()
		{
			string directory = captureStartTime.ToString().Replace(":","-").Replace ("/","-").Replace (" ","-").Replace ("\\","-");
			string fileName = string.Format ("image_{0}", nextImageIndex++);
			return Path.Combine( directory, fileName );
		}

		// this is a poor man's message logger.  This really should be in a table view but this is just a sample app so ...
		void LogMessage(string message )
		{
			DateTime time = DateTime.Now;
			string timeText = string.Format ("{0}:{1}:{2}.{3}", time.Hour, time.Minute.ToString().PadLeft(2,'0'), time.Second.ToString().PadLeft(2,'0'), time.Millisecond);
			string timeStampedMessage = string.Format ("{0}: {1}\r\n", timeText , message.TrimEnd());
			messages.Enqueue( timeStampedMessage );

			StringBuilder sb = new StringBuilder();
			string [] messageArray = messages.ToArray();
			foreach ( string m in messageArray )
				sb.Append(m);
			string text = sb.ToString();

			InvokeOnMainThread(() => {
				textView.Text = text;
				ScrollMessageViewToEnd();
			});
		}

		// this method makes sure that the most recently added text is exactly at the bottom of the text view
		void ScrollMessageViewToEnd()
		{
			try
			{
				// find the number of characters between the end of the text and the previous newline
				string text = textView.Text.TrimEnd();
				int index = text.Length - 1;
				while (true)
				{
					char c = text[index];
					if ( c == '\r' || c == '\n' )
						break;
					index--;
				}
				textView.ScrollRangeToVisible( new NSRange( index + 1, 1 ) );
			}
			catch
			{
			}
		}
	}
}