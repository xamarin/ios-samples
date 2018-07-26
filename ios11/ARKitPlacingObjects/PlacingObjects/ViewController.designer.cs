// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PlacingObjects
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		public ARKit.ARSCNView SceneView { get; set; }

		[Outlet]
		public UIKit.UIButton AddObjectButton { get; set; }

		[Outlet]
		public UIKit.UIButton SettingsButton { get; set; }

		[Outlet]
		public UIKit.UIButton RestartExperienceButton { get; set; }

		[Outlet]
		public UIKit.UILabel MessageLabel { get; set; }

		[Outlet]
		public UIKit.UIView MessagePanel { get; set; }


		void ReleaseDesignerOutlets ()
		{
			if (SceneView != null) {
				SceneView.Dispose ();
				SceneView = null;
			}
		}
	}
}
