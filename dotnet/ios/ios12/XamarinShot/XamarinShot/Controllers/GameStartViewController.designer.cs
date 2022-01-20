// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace XamarinShot
{
    [Register ("GameStartViewController")]
    partial class GameStartViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton backButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIVisualEffectView blurView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView browserContainerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton hostButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton joinButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel nearbyGamesLabel { get; set; }

        [Action ("backButtonPressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void backButtonPressed (UIKit.UIButton sender);

        [Action ("joinButtonPressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void joinButtonPressed (UIKit.UIButton sender);

        [Action ("settingsPressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void settingsPressed (UIKit.UIButton sender);

        [Action ("startGamePressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void startGamePressed (UIKit.UIButton sender);

        [Action ("startSoloGamePressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void startSoloGamePressed (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (backButton is not null) {
                backButton.Dispose ();
                backButton = null;
            }

            if (blurView is not null) {
                blurView.Dispose ();
                blurView = null;
            }

            if (browserContainerView is not null) {
                browserContainerView.Dispose ();
                browserContainerView = null;
            }

            if (hostButton is not null) {
                hostButton.Dispose ();
                hostButton = null;
            }

            if (joinButton is not null) {
                joinButton.Dispose ();
                joinButton = null;
            }

            if (nearbyGamesLabel is not null) {
                nearbyGamesLabel.Dispose ();
                nearbyGamesLabel = null;
            }
        }
    }
}