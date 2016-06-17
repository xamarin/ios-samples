// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MusicMotion
{
	[Register ("SongViewController")]
	partial class SongViewController
	{
		[Outlet]
		UIKit.UIImageView AlbumView { get; set; }

		[Outlet]
		UIKit.UITableView SongTableView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (AlbumView != null) {
				AlbumView.Dispose ();
				AlbumView = null;
			}

			if (SongTableView != null) {
				SongTableView.Dispose ();
				SongTableView = null;
			}
		}
	}
}
