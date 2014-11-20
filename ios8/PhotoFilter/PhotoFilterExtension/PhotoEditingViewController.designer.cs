using UIKit;
using Foundation;

namespace PhotoFilterExtension
{
	[Register ("PhotoEditingViewController")]
	partial class PhotoEditingViewController
	{
		[Outlet("collectionView")]
		UICollectionView CollectionView { get; set; }

		[Outlet("filterPreviewView")]
		UIImageView FilterPreviewView { get; set; }

		[Outlet("backgroundImageView")]
		UIImageView BackgroundImageView { get; set; }

		void ReleaseDesignerOutlets ()
		{
		}
	}
}

