// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace HomeKitCatalog
{
	[Register ("FavoritesViewController")]
	partial class FavoritesViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem EditButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (EditButton != null) {
				EditButton.Dispose ();
				EditButton = null;
			}
		}
	}
}
