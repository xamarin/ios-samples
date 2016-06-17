// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace tvTable
{
	[Register ("AttractionViewController")]
	partial class AttractionViewController
	{
		[Outlet]
		UIKit.UIImageView AttractionImage { get; set; }

		[Outlet]
		UIKit.UIImageView BackgroundImage { get; set; }

		[Outlet]
		UIKit.UILabel City { get; set; }

		[Outlet]
		UIKit.UIImageView IsDirections { get; set; }

		[Outlet]
		UIKit.UIImageView IsFavorite { get; set; }

		[Outlet]
		UIKit.UIImageView IsFlighBooked { get; set; }

		[Outlet]
		UIKit.UILabel SubTitle { get; set; }

		[Outlet]
		UIKit.UILabel Title { get; set; }

		[Action ("BookFlight:")]
		partial void BookFlight (Foundation.NSObject sender);

		[Action ("GetDirections:")]
		partial void GetDirections (Foundation.NSObject sender);

		[Action ("MarkFavorite:")]
		partial void MarkFavorite (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AttractionImage != null) {
				AttractionImage.Dispose ();
				AttractionImage = null;
			}

			if (BackgroundImage != null) {
				BackgroundImage.Dispose ();
				BackgroundImage = null;
			}

			if (City != null) {
				City.Dispose ();
				City = null;
			}

			if (IsFavorite != null) {
				IsFavorite.Dispose ();
				IsFavorite = null;
			}

			if (IsFlighBooked != null) {
				IsFlighBooked.Dispose ();
				IsFlighBooked = null;
			}

			if (IsDirections != null) {
				IsDirections.Dispose ();
				IsDirections = null;
			}

			if (SubTitle != null) {
				SubTitle.Dispose ();
				SubTitle = null;
			}

			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}
		}
	}
}
