using System;
using Foundation;
using UIKit;

namespace ScanningAndDetecting3DObjects {
	internal class ViewControllerNavigationBar {
		UINavigationBar navigationBar;
		UIBarButtonItem backButton;
		UIBarButtonItem startOverButton;

		internal ViewControllerNavigationBar (UINavigationBar navigationBar, EventHandler previousButtonTapped, EventHandler restartButtonTapped)
		{
			this.navigationBar = navigationBar;

			backButton = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Plain, previousButtonTapped);
			startOverButton = new UIBarButtonItem ("Restart", UIBarButtonItemStyle.Plain, restartButtonTapped);

			var navigationItem = new UINavigationItem ("Start");
			navigationItem.LeftBarButtonItem = backButton;
			navigationItem.RightBarButtonItem = startOverButton;
			navigationBar.Items = new [] { navigationItem };

			navigationBar.SetBackgroundImage (new UIImage (), UIBarMetrics.Default);
			navigationBar.ShadowImage = new UIImage ();
			navigationBar.Translucent = true;
		}

		internal void SetNavigationBarTitle (string title)
		{
			var navItem = navigationBar.Items [0];
			if (navItem == null) {
				return;
			}
			navItem.Title = title;
		}

		internal void ShowBackButton (bool show)
		{
			var navItem = navigationBar.Items [0];
			if (navItem == null) {
				return;
			}
			if (show) {
				navItem.LeftBarButtonItem = backButton;
			} else {
				navItem.LeftBarButtonItem?.Dispose ();
				navItem.LeftBarButtonItem = null;
			}
		}
	}
}
