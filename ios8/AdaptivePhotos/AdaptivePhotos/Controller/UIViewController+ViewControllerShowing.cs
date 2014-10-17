using System;
using UIKit;
using Foundation;
using ObjCRuntime;
using System.Reflection;

namespace AdaptivePhotos
{
	public class CustomTableViewController : UITableViewController
	{
		public CustomTableViewController (UITableViewStyle style) : base (style)
		{
		}

		public bool WillShowingViewControllerPushWithSender ()
		{
			var selector = new Selector ("WillShowingViewControllerPushWithSender");
			var target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("WillShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}

		public bool WillShowingDetailViewControllerPushWithSender ()
		{
			var selector = new Selector ("WillShowingDetailViewControllerPushWithSender");
			var target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("WillShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}

		public virtual Photo ContainedPhoto (Photo photo)
		{
			return null;
		}

		public virtual bool ContainsPhoto (Photo photo)
		{
			return false;
		}

		public Photo CurrentVisibleDetailPhotoWithSender ()
		{
			var selector = new Selector ("CurrentVisibleDetailPhotoWithSender");
			UIViewController target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("CurrentVisibleDetailPhotoWithSender");
				return (Photo)method.Invoke (target, new object[] { });
			} else {
				return null;
			}
		}
	}

	public class CustomViewController : UIViewController
	{
		public bool WillShowingViewControllerPushWithSender ()
		{
			var selector = new Selector ("WillShowingViewControllerPushWithSender");
			var target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("WillShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}

		public bool WillShowingDetailViewControllerPushWithSender ()
		{
			var selector = new Selector ("WillShowingDetailViewControllerPushWithSender");
			var target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("WillShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}

		public virtual Photo ContainedPhoto (Photo photo)
		{
			return null;
		}

		public virtual bool ContainsPhoto (Photo photo)
		{
			return false;
		}
	}

	public class CustomNavigationController : UINavigationController
	{
		public CustomNavigationController (UIViewController viewController) : base (viewController)
		{
		}

		[Export ("WillShowingViewControllerPushWithSender")]
		public bool WillShowingDetailViewControllerPushWithSender ()
		{
			return true;
		}
	}

	public class CustomSplitViewController : UISplitViewController
	{
		[Export ("WillShowingViewControllerPushWithSender")]
		public bool WillShowingViewControllerPushWithSender ()
		{
			return false;
		}

		[Export ("WillShowingDetailViewControllerPushWithSender")]
		public bool WillShowingDetailViewControllerPushWithSender ()
		{
			if (this.Collapsed) {
				var target = this.ViewControllers [this.ViewControllers.Length - 1];
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("WillShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}
	}
}

