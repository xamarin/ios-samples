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

namespace SocialFrameworkDemo
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PostToFacebook { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RequestFacebookTimeline { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RequestTwitterTimeline { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView Results { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SendTweet { get; set; }

        [Action ("SendTweet_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendTweet_TouchUpInside (UIKit.UIButton sender);

        [Action ("RequestTwitterTimeline_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RequestTwitterTimeline_TouchUpInside (UIKit.UIButton sender);

        [Action ("PostToFacebook_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PostToFacebook_TouchUpInside (UIKit.UIButton sender);

        [Action ("RequestFacebookTimeline_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RequestFacebookTimeline_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (PostToFacebook != null) {
                PostToFacebook.Dispose ();
                PostToFacebook = null;
            }

            if (RequestFacebookTimeline != null) {
                RequestFacebookTimeline.Dispose ();
                RequestFacebookTimeline = null;
            }

            if (RequestTwitterTimeline != null) {
                RequestTwitterTimeline.Dispose ();
                RequestTwitterTimeline = null;
            }

            if (Results != null) {
                Results.Dispose ();
                Results = null;
            }

            if (SendTweet != null) {
                SendTweet.Dispose ();
                SendTweet = null;
            }
        }
    }
}