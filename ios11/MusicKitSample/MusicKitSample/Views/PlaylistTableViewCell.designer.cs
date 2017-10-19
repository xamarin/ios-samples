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
    [Register ("PlaylistTableViewCell")]
    partial class PlaylistTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImgAssetCoverArt { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LblAlbum { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LblArtist { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LblTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ImgAssetCoverArt != null) {
                ImgAssetCoverArt.Dispose ();
                ImgAssetCoverArt = null;
            }

            if (LblAlbum != null) {
                LblAlbum.Dispose ();
                LblAlbum = null;
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