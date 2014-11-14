using UIKit;
using System;
using Foundation;
using MediaPlayer;

namespace MoviePlaybackSample
{
    public partial class MoviePlaybackSampleViewController : UIViewController
    {
        //class level declarations
        MPMoviePlayerController mp = new MPMoviePlayerController();

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public MoviePlaybackSampleViewController()
            : base(UserInterfaceIdiomIsPhone ? "MoviePlaybackSampleViewController_iPhone" : "MoviePlaybackSampleViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
			
            base.ViewDidLoad();

            //Button Events
            playMovieButton.TouchUpInside += delegate
            {
			
                try
                {
                    //Set already instantiated MPMoviePlayerController to playback from Movies/file.m4v
                    mp = new MPMoviePlayerController(NSUrl.FromFilename("Movies/file.m4v"));
					
                    //enable AirPlay
                    mp.AllowsAirPlay = true;
					
                    //Add the MPMoviePlayerController View
                    this.View.AddSubview(mp.View);
					
                    //set the view to be full screen and show animated
                    mp.SetFullscreen(true, true);

                    //Disable the pinch-to-zoom gesture
                    mp.ControlStyle = MPMovieControlStyle.Fullscreen;
			
                    //MPMoviePlayer must be set to PrepareToPlay before playback
                    mp.PrepareToPlay();
					
                    //Play Movie
                    mp.Play();
                }
                catch
                {
                    Console.WriteLine("There was a problem playing back Video");
                }		
				
            };
						
        }

        [Obsolete]
        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
        }

        [Obsolete]
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            if (UserInterfaceIdiomIsPhone)
            {
                return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
            }
            else
            {
                return true;
            }
        }
    }
}
