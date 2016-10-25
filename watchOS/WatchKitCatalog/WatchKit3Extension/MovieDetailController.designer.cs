// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Watchkit2Extension
{
	[Register ("MovieDetailController")]
	partial class MovieDetailController
	{
		[Outlet]
		WatchKit.WKInterfaceInlineMovie InlineMovie { get; set; }

		[Outlet]
		WatchKit.WKInterfaceMovie Movie { get; set; }

		[Outlet]
		WatchKit.WKTapGestureRecognizer TapGestureRecognizer { get; set; }

		[Action ("InlineMovieTapped:")]
		partial void InlineMovieTapped (Foundation.NSObject sender);

	
		
		void ReleaseDesignerOutlets ()
		{
			if (Movie != null) {
				Movie.Dispose ();
				Movie = null;
			}

			if (InlineMovie != null) {
				InlineMovie.Dispose ();
				InlineMovie = null;
			}

			if (TapGestureRecognizer != null) {
				TapGestureRecognizer.Dispose ();
				TapGestureRecognizer = null;
			}
		}
	}
}
