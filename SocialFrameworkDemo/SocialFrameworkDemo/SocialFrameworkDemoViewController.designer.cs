// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace SocialFrameworkDemo
{
	[Register ("SocialFrameworkDemoViewController")]
	partial class SocialFrameworkDemoViewController
	{
		[Outlet]
		UIKit.UIButton twitterRequestButton { get; set; }

		[Outlet]
		UIKit.UIButton facebookRequestButton { get; set; }

		[Outlet]
		UIKit.UIButton twitterButton { get; set; }

		[Outlet]
		UIKit.UIButton facebookButton { get; set; }

		[Outlet]
		UIKit.UITextView resultsTextView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (twitterRequestButton != null) {
				twitterRequestButton.Dispose ();
				twitterRequestButton = null;
			}

			if (facebookRequestButton != null) {
				facebookRequestButton.Dispose ();
				facebookRequestButton = null;
			}

			if (twitterButton != null) {
				twitterButton.Dispose ();
				twitterButton = null;
			}

			if (facebookButton != null) {
				facebookButton.Dispose ();
				facebookButton = null;
			}

			if (resultsTextView != null) {
				resultsTextView.Dispose ();
				resultsTextView = null;
			}
		}
	}
}
