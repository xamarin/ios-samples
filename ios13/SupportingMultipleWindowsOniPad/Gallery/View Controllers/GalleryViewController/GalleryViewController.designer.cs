// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Gallery
{
	[Register ("GalleryViewController")]
	partial class GalleryViewController
	{
		[Outlet]
		UIKit.UICollectionView galleryCollectionView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (galleryCollectionView != null) {
				galleryCollectionView.Dispose ();
				galleryCollectionView = null;
			}
		}
	}
}
