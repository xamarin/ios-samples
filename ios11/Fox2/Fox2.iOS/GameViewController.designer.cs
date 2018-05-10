// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Fox2
{
    [Register ("GameViewControllerIOS")]
    partial class GameViewControllerIOS
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SceneKit.SCNView gameView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (gameView != null) {
                gameView.Dispose ();
                gameView = null;
            }
        }
    }
}