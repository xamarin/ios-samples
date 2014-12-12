using UIKit;
using Foundation;

namespace PhotoFilterExtension
{
	[Register ("PhotoEditingViewController")]
	partial class PhotoEditingViewController
	{
		[Outlet("collectionView")]
		private UICollectionView CollectionView { get; set; }

		[Outlet("filterPreviewView")]
		private UIImageView FilterPreviewView { get; set; }

		[Outlet("backgroundImageView")]
		private UIImageView BackgroundImageView { get; set; }

		void ReleaseDesignerOutlets ()
		{
		}
	}
}

