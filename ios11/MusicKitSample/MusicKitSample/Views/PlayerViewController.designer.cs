// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace MusicKitSample
{
    [Register ("PlayerViewController")]
    partial class PlayerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BtnPlayPause { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BtnSkipToNext { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BtnSkipToPrevious { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImgCurrentArtwork { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LblCurrentAlbum { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LblCurrentArtist { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LblCurrentTitle { get; set; }

        [Action ("BtnPlayPause_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnPlayPause_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnSkipToNext_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSkipToNext_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnSkipToPrevious_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSkipToPrevious_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BtnPlayPause != null) {
                BtnPlayPause.Dispose ();
                BtnPlayPause = null;
            }

            if (BtnSkipToNext != null) {
                BtnSkipToNext.Dispose ();
                BtnSkipToNext = null;
            }

            if (BtnSkipToPrevious != null) {
                BtnSkipToPrevious.Dispose ();
                BtnSkipToPrevious = null;
            }

            if (ImgCurrentArtwork != null) {
                ImgCurrentArtwork.Dispose ();
                ImgCurrentArtwork = null;
            }

            if (LblCurrentAlbum != null) {
                LblCurrentAlbum.Dispose ();
                LblCurrentAlbum = null;
            }

            if (LblCurrentArtist != null) {
                LblCurrentArtist.Dispose ();
                LblCurrentArtist = null;
            }

            if (LblCurrentTitle != null) {
                LblCurrentTitle.Dispose ();
                LblCurrentTitle = null;
            }
        }
    }
}