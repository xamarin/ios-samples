// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MoviePlaybackSample
{
	[Register ("MoviePlaybackSampleViewController")]
	partial class MoviePlaybackSampleViewController
	{
		[Outlet]
		UIKit.UIButton playMovieButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (playMovieButton != null) {
				playMovieButton.Dispose ();
				playMovieButton = null;
			}
		}
	}
}
