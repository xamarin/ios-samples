﻿/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A view controller that contains an image view to show a particular photo.
*/

using System;
using Foundation;
using UIKit;

namespace Gallery {
	public partial class PhotoDetailViewController : UIViewController {
		public Photo Photo { get; set; }

		protected PhotoDetailViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (Photo != null)
				imageView.Image = UIImage.FromFile (Photo.Name);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			View.Window.WindowScene.UserActivity = Photo?.OpenDetailUserActivity ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			View.Window.WindowScene.UserActivity = null;
		}

		public static PhotoDetailViewController LoadFromStoryboard ()
		{
			var storyboard = UIStoryboard.FromName ("Main", NSBundle.MainBundle);
			return storyboard.InstantiateViewController (nameof (PhotoDetailViewController)) as PhotoDetailViewController;
		}
	}
}

