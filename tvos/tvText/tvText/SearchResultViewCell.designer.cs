// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace tvText
{
    [Register ("SearchResultViewCell")]
    partial class SearchResultViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView Picture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Title { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Picture != null) {
                Picture.Dispose ();
                Picture = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}