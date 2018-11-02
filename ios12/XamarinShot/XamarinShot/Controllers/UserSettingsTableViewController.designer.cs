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
    [Register ("UserSettingsTableViewController")]
    partial class UserSettingsTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel appVersionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider effectsVolume { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider musicVolume { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField playerNameTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel selectedLevelLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch spectatorSwitch { get; set; }

        [Action ("doneTapped:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void doneTapped (UIKit.UIBarButtonItem sender);

        [Action ("effectVolumeChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void effectVolumeChanged (UIKit.UISlider sender);

        [Action ("effectVolumeReleased:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void effectVolumeReleased (UIKit.UISlider sender);

        [Action ("musicVolumeChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void musicVolumeChanged (UIKit.UISlider sender);

        [Action ("spectatorChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void spectatorChanged (UIKit.UISwitch sender);

        void ReleaseDesignerOutlets ()
        {
            if (appVersionLabel != null) {
                appVersionLabel.Dispose ();
                appVersionLabel = null;
            }

            if (effectsVolume != null) {
                effectsVolume.Dispose ();
                effectsVolume = null;
            }

            if (musicVolume != null) {
                musicVolume.Dispose ();
                musicVolume = null;
            }

            if (playerNameTextField != null) {
                playerNameTextField.Dispose ();
                playerNameTextField = null;
            }

            if (selectedLevelLabel != null) {
                selectedLevelLabel.Dispose ();
                selectedLevelLabel = null;
            }

            if (spectatorSwitch != null) {
                spectatorSwitch.Dispose ();
                spectatorSwitch = null;
            }
        }
    }
}