// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace StackView
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIStackView RatingView { get; set; }

		[Action ("DecreaseRating:")]
		partial void DecreaseRating (Foundation.NSObject sender);

		[Action ("IncreaseRating:")]
		partial void IncreaseRating (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (RatingView != null) {
				RatingView.Dispose ();
				RatingView = null;
			}
		}
	}
}
