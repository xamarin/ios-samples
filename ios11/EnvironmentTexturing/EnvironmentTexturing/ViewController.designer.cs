// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace EnvironmentTexturing
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		ARKit.ARSCNView sceneView { get; set; }

		[Outlet]
		UIKit.UILabel sessionInfoLabel { get; set; }

		[Outlet]
		UIKit.UIVisualEffectView sessionInfoView { get; set; }

		[Outlet]
		UIKit.UISegmentedControl textureModeSelectionControl { get; set; }

		[Action ("ChangeTextureMode:")]
		partial void ChangeTextureMode (UIKit.UISegmentedControl sender);

		[Action ("DidPan:")]
		partial void DidPan (UIKit.UIPanGestureRecognizer gesture);

		[Action ("DidScale:")]
		partial void DidScale (UIKit.UIPinchGestureRecognizer gesture);

		[Action ("DidTap:")]
		partial void DidTap (UIKit.UITapGestureRecognizer gesture);

		[Action ("RestartExperience:")]
		partial void RestartExperience (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (sceneView != null) {
				sceneView.Dispose ();
				sceneView = null;
			}

			if (sessionInfoLabel != null) {
				sessionInfoLabel.Dispose ();
				sessionInfoLabel = null;
			}

			if (sessionInfoView != null) {
				sessionInfoView.Dispose ();
				sessionInfoView = null;
			}

			if (textureModeSelectionControl != null) {
				textureModeSelectionControl.Dispose ();
				textureModeSelectionControl = null;
			}
		}
	}
}
