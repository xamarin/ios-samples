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
    [Register ("MediaItemTableViewCell")]
    partial class MediaItemTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BtnAddToPlaylist { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BtnPlayItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImgAssetCoverArt { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LblArtist { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LblTitle { get; set; }

        [Action ("BtnAddToPlaylist_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnAddToPlaylist_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnPlayItem_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnPlayItem_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BtnAddToPlaylist != null) {
                BtnAddToPlaylist.Dispose ();
                BtnAddToPlaylist = null;
            }

            if (BtnPlayItem != null) {
                BtnPlayItem.Dispose ();
                BtnPlayItem = null;
            }

            if (ImgAssetCoverArt != null) {
                ImgAssetCoverArt.Dispose ();
                ImgAssetCoverArt = null;
            }

            if (LblArtist != null) {
                LblArtist.Dispose ();
                LblArtist = null;
            }

            if (LblTitle != null) {
                LblTitle.Dispose ();
                LblTitle = null;
            }
        }
    }
}