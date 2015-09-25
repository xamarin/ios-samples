// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ViewControllerPreview
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet]
		UIKit.UILabel DetailDescriptionLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DetailDescriptionLabel != null) {
				DetailDescriptionLabel.Dispose ();
				DetailDescriptionLabel = null;
			}
		}
	}
}
