// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("DataItemCollectionViewCell")]
	partial class DataItemCollectionViewCell
	{
		[Outlet]
		public UIKit.UIImageView ImageView { get; private set; }

		[Outlet]
		public UIKit.UILabel Label { get; private set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

			if (Label != null) {
				Label.Dispose ();
				Label = null;
			}
		}
	}
}
