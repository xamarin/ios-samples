﻿// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BackgroundExecution
{
    [Register ("HomeScreen")]
    partial class HomeScreen
    {
        [Outlet]
        UIButton BtnStartLongRunningTask { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnStartLongRunningTask { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnStartLongRunningTask != null) {
                btnStartLongRunningTask.Dispose ();
                btnStartLongRunningTask = null;
            }
        }
    }
}