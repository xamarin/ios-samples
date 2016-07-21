// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MessagesExtension
{
	[Register ("BuildIceCreamViewController")]
	partial class BuildIceCreamViewController
	{
		[Outlet]
		UIKit.UICollectionView CollectionView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CollectionViewHeightConstraint { get; set; }

		[Outlet]
		MessagesExtension.IceCreamView IceCreamView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint IceCreamViewHeightConstraint { get; set; }

		[Outlet]
		UIKit.UILabel PromtLabel { get; set; }

		[Action ("DidTapSelect:")]
		partial void DidTapSelect (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CollectionView != null) {
				CollectionView.Dispose ();
				CollectionView = null;
			}

			if (CollectionViewHeightConstraint != null) {
				CollectionViewHeightConstraint.Dispose ();
				CollectionViewHeightConstraint = null;
			}

			if (IceCreamView != null) {
				IceCreamView.Dispose ();
				IceCreamView = null;
			}

			if (IceCreamViewHeightConstraint != null) {
				IceCreamViewHeightConstraint.Dispose ();
				IceCreamViewHeightConstraint = null;
			}

			if (PromtLabel != null) {
				PromtLabel.Dispose ();
				PromtLabel = null;
			}
		}
	}
}
