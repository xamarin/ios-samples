using System;
using System.Drawing;
using Foundation;
using UIKit;
using ObjCRuntime;

namespace StoryboardSample
{
	public partial class BaseController : UIViewController
	{
		// provide access to the sidebar controller to all inheriting controllers
		protected SidebarNavigation.SidebarController? SidebarController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate)?.RootViewController?.SidebarController;
			} 
		}

		// provide access to the navigation controller to all inheriting controllers
		protected NavController NavController { 
			get {
				if (UIApplication.SharedApplication.Delegate is AppDelegate appDelegate && appDelegate.RootViewController is not null) {
					return appDelegate.RootViewController.NavController;
				} else {
					Console.WriteLine ("NavController failed");
					throw new NotSupportedException ("unable to get root view controller.");
				}
			} 
		}

		// provide access to the storyboard to all inheriting controllers
		public override UIStoryboard Storyboard { 
			get {
				if (UIApplication.SharedApplication.Delegate is AppDelegate appDelegate && appDelegate.RootViewController is not null) {
					return appDelegate.RootViewController.Storyboard;
				} else {
					Console.WriteLine ("Storyboard failed.");
					throw new NotSupportedException ("unable to get storyboard.");
				}
			} 
		}

		public BaseController (NativeHandle handle) : base (handle)
		{
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationItem.SetRightBarButtonItem (
				new UIBarButtonItem (UIImage.FromBundle ("threelines"),
					UIBarButtonItemStyle.Plain,
					ButtonHandler), true);
		}

		void ButtonHandler (object? sender, EventArgs e) {
			if (SidebarController is not null) {
				SidebarController.ToggleMenu ();
			}
		}
	}
}

