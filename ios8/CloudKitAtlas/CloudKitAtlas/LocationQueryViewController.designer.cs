// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace CloudKitAtlas
{
	[Register ("LocationQueryViewController")]
	partial class LocationQueryViewController
	{
		[Outlet]
		MapKit.MKMapView map { get; set; }

		[Action ("QueryRecords:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void QueryRecords (UIBarButtonItem sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
