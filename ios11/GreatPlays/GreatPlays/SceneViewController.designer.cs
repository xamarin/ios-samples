// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace GreatPlays
{
    [Register ("SceneViewController")]
    partial class SceneViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView sceneText { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (sceneText != null) {
                sceneText.Dispose ();
                sceneText = null;
            }
        }
    }
}