using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MediaPlayer
{
	public partial class MyStreamingMovieViewController : UIViewController {
		public MyStreamingMovieViewController (IntPtr handle) : base (handle) 
		{ }
		
		public override void ViewDidLoad ()
		{
			movieURLTextField.ShouldReturn = delegate {
				movieURLTextField.ResignFirstResponder ();
				return true;
			};
		}

		partial void playMovieButtonPressed (UIButton sender)
		{
			try {
				var url = new NSUrl (movieURLTextField.Text);
				var appDelegate = UIApplication.SharedApplication.Delegate as MoviePlayerAppDelegate;
				
				appDelegate.InitAndPlayMovie (url);
			} catch {
				// Ignore errors in the movieUrl
			}
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

	}	
}