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
		private DialogViewController settingsController = null;
		private SettingsDialog settingsDialog = null;
		private Settings settings = null;
		
		// media browsing
		private DialogViewController browseController = null;
		private MediaBrowserDialog browseDialog = null;
		
		// capture
		private CaptureManager captureManager = null;
		private bool isCapturing = false;
		private DateTime captureStartTime;
		private int nextImageIndex = 1;
			
		// log message
		private UITextView textView = null;
		private ConcurrentQueue<string> messages = new ConcurrentQueue<string>();
		private int textViewXOffset;
		private int textViewYOffset;
		private int textViewWidth;
		private int textViewHeight;

		// image previewer
		private UIImageView imageView = null;
		private DateTime lastImageWriteTime = DateTime.MinValue;
		
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
			{
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} 
			else 
			{
				return true;
			}
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// load the saved (or default) settings
			settings = new Settings();
			settings.Load();
			
			setupButtons();
			setupTextView();
			setupImageView();
			layoutViews();
			
			// subscribe to the popped controller event
			((RootViewController)NavigationController).ViewControllerPopped += handleViewControllerPopped;
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			layoutViews();
		}
		
		private void layoutViews()
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
			scrollMessageViewToEnd();
		}
		
		private bool capturing
		{
			get
			{
				return (captureManager != null && captureManager.IsCapturing);
			}
		}
		
		void handleViewControllerPopped (object sender, RootViewController.ViewControllerPoppedEventArgs e)
		{
			if ( e.Controller == settingsController )
			{
				harvestSettings();
				layoutViews();
			}
			else if ( e.Controller == browseController )
			{
				try
				{
					// unsubscribe browser events
					browseDialog.MovieFileSelected -= new EventHandler<FileSelectedEventArgs>( handleMediaFileSelected );
					browseDialog.ImageFileSelected -= new EventHandler<FileSelectedEventArgs>( handleImageFileSelected );
				}
				catch
				{
				}
			}
		}
		
		private void updateUI()
		{
			this.buttonSettings.Enabled = !isCapturing;
			this.buttonBrowse.Enabled = !isCapturing;
			this.buttonStartStop.Enabled = !buttonHandlerInProgress;
		}
		
		private void setupButtons()
		{
			buttonSettings.Clicked += (s,e) =>
			{
				editSettings();
				updateUI ();
			};
			
			buttonStartStop.Clicked += (s,e) =>
			{
				startStop();
				updateUI();
			};
			
			buttonBrowse.Clicked += (s,e) =>
			{
				browse();
				updateUI();
			};
		}
		
		private void setupTextView()
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

		private void setupImageView()
		{
			if ( imageView == null )
			{
				imageView = new UIImageView ( new CGRect(1,1,1,1) );
				imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				this.View.AddSubview (imageView);				
			}
		}
		
		private bool buttonHandlerInProgress = false;
		private void startStop()
		{
			try
			{
				if ( buttonHandlerInProgress == true )
				{
					return;
				}
				buttonHandlerInProgress = true;
				
				// the layout call is to handle the possibility that settings changed and the image capture view may now
				// have different visibility
				layoutViews();
				startStopCapture();
			}
			finally
			{
				buttonHandlerInProgress = false;
			}
			updateUI();
		}
		
		private void startStopCapture()
		{
			if ( isCapturing == false)
			{
				try
				{
					capture();
					if ( isCapturing)
					{
						buttonStartStop.Title = "Stop";
					}
				}
				catch (Exception ex)
				{
					logMessage( string.Format ("Failed to start capture: {0}", ErrorHandling.GetExceptionDetailedText(ex)) );
				}
			}
			else
			{
				stopCapture();
				buttonStartStop.Title = "Start";
			}
		}
		
		private void browse()
		{
			browseDialog = new MediaBrowserDialog( Settings.ImageDataPath, Settings.VideoDataPath );
			browseDialog.MovieFileSelected += new EventHandler<FileSelectedEventArgs>( handleMediaFileSelected );
			browseDialog.ImageFileSelected += new EventHandler<FileSelectedEventArgs>( handleImageFileSelected );
			browseController = new DialogViewController (browseDialog.Menu, true);
			NavigationController.PushViewController (browseController, true);				
		}
		
		private void editSettings()
		{
			settingsDialog = new SettingsDialog( settings );
			settingsController = new DialogViewController (settingsDialog.Menu, true);
			NavigationController.PushViewController (settingsController, true);				
			settingsDialog.EnforceDependencies(); // must be called after view controller push
		}
		
		private void harvestSettings()
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
		
		
		private void capture()
		{
			// stop capture if it is in progress
			stopCapture();
			
			// make sure there is something configured for capture before starting up
			if ( !settings.AudioCaptureEnabled && !settings.VideoCaptureEnabled && !settings.ImageCaptureEnabled )
			{
				logMessage ("No capture devices enabled.  Enable one or more capture types in settings then retry");
				isCapturing = false;
				return;
			}

			// reset variables
			captureStartTime = DateTime.Now;
			nextImageIndex = 1;
			
			// create new capture manager and subscribe events
			logMessage("Creating and initializing capture graph");
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
			captureManager.MovieSegmentCaptured += new EventHandler<MovieSegmentCapturedEventArgs>( handleMovieSegmentCaptured );
			captureManager.CaptureError += new EventHandler<CaptureErrorEventArgs>( handleCaptureError );
			captureManager.ImageCaptured += new EventHandler<ImageCaptureEventArgs>( handleImageCaptured );
			
			string errorMessage = null;
			logMessage("starting capture ...");
			if ( captureManager.StartCapture( out errorMessage ) == false )
			{
				logMessage( errorMessage );
				return;
			}
			if ( settings.VideoCaptureEnabled || settings.ImageCaptureEnabled)
			{
				logMessage ( string.Format("capture started at {0} resolution", settings.CaptureResolution));
			}
			if ( settings.AudioCaptureEnabled )
			{
				logMessage ( string.Format("Capturing audio"));
			}
			if ( settings.VideoCaptureEnabled )
			{
				logMessage ( string.Format("Capturing video"));
			}
			if ( settings.ImageCaptureEnabled )
			{
				logMessage ( string.Format("Capturing image samples"));
			}
			
			isCapturing = true;
		}

		private void stopCapture()
		{
			if ( captureManager != null )
			{
				logMessage("stopping capture...");
				try
				{
					captureManager.StopCapture();
					
					// unsubscribe events
					captureManager.MovieSegmentCaptured -= new EventHandler<MovieSegmentCapturedEventArgs>( handleMovieSegmentCaptured );
					captureManager.CaptureError -= new EventHandler<CaptureErrorEventArgs>( handleCaptureError );
					captureManager.ImageCaptured -= new EventHandler<ImageCaptureEventArgs>( handleImageCaptured );
					captureManager = null;
				}
				catch
				{
				}
				logMessage("capture stopped");
			}
			isCapturing = false;
		}
		
		private void handleMediaFileSelected( object sender, FileSelectedEventArgs args )
		{
			NavigationController.PopToViewController(this,true);

			try
			{
				MPMoviePlayerController player = new MPMoviePlayerController();
				player = new MPMoviePlayerController (NSUrl.FromFilename( args.File ));
				player.AllowsAirPlay = true;
				this.View.AddSubview(player.View);
				player.SetFullscreen(true, true);
				player.PrepareToPlay();
				player.Play();
			}
			catch ( Exception ex )
			{
				string message = string.Format ("Error during playback of {0}: {1}", Path.GetFileName(args.File), ErrorHandling.GetExceptionDetailedText(ex) );
				logMessage( message );
			}		
		}
		
		private void handleImageFileSelected( object sender, FileSelectedEventArgs args )
		{
			NavigationController.PopToViewController(this,true);
			UIImage image = UIImage.FromFile( args.File );
			this.imageView.Image = image;
		}
		
		private void handleMovieSegmentCaptured( object sender, MovieSegmentCapturedEventArgs args )
		{
			logMessage( string.Format("Media file captured to '{0}'", Path.GetFileName(args.File)) );
		}
		
		private void handleCaptureError( object sender, CaptureErrorEventArgs args )
		{
			logMessage( args.ErrorMessage );
		}
		
		private void handleImageCaptured( object sender, ImageCaptureEventArgs args )
		{
			imageView.InvokeOnMainThread (delegate {
				
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
							logMessage("Captured image saved to photos library");
						}
						else if ( settings.SaveCapturedImagesToMyDocuments )
						{
							string partialPath = getNextImageFileName();
							string fullPath = Path.Combine( Settings.ImageDataPath, partialPath );
							byte[] bytes = args.Image.AsJPEG().ToArray();
							
							// create directory
							string directory = Path.GetDirectoryName( fullPath );
							if ( Directory.Exists( directory ) == false )
							{
								Directory.CreateDirectory( directory );
							}
							// delete file if it exists already.  this is unlikely but possible if the user starts, stops, and starts the capture 
							// quickly enough to cause the same DateTome-based folder name to be regenerated
							if ( File.Exists(fullPath) )
							{
								File.Delete( fullPath );
							}
							
							// write the image to the file
							File.WriteAllBytes(  fullPath, bytes );
							logMessage(string.Format ("Captured image saved to '{0}'", partialPath));
						}

						// update the last capture time
						lastImageWriteTime = DateTime.Now;
					}
				}
			});
		}
		
		private string getNextImageFileName()
		{
			string directory = captureStartTime.ToString().Replace(":","-").Replace ("/","-").Replace (" ","-").Replace ("\\","-");
			string fileName = string.Format ("image_{0}", nextImageIndex++);
			return Path.Combine( directory, fileName );
		}
			
		// this is a poor man's message logger.  This really should be in a table view but this is just a sample app so ...
		private void logMessage( string message )
		{
			DateTime time = DateTime.Now;
			string timeText = string.Format ("{0}:{1}:{2}.{3}", time.Hour, time.Minute.ToString().PadLeft(2,'0'), time.Second.ToString().PadLeft(2,'0'), time.Millisecond);
			string timeStampedMessage = string.Format ("{0}: {1}\r\n", timeText , message.TrimEnd());
			messages.Enqueue( timeStampedMessage );
			
			StringBuilder sb = new StringBuilder();
			string [] messageArray = messages.ToArray();
			foreach ( string m in messageArray )
			{
				sb.Append(m);
			}
			string text = sb.ToString();
			
			InvokeOnMainThread(delegate{
				textView.Text = text;
				scrollMessageViewToEnd();
			});
			
		}
		
		// this method makes sure that the most recently added text is exactly at the bottom of the text view
		private void scrollMessageViewToEnd()
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
					{
						break;
					}
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

