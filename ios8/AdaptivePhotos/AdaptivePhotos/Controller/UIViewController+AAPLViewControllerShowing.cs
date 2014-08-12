using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Reflection;

namespace AdaptivePhotos
{
	public class CustomTableViewController : UITableViewController
	{
		public CustomTableViewController (UITableViewStyle style) : base (style)
		{
		}

		public bool Aapl_willShowingViewControllerPushWithSender ()
		{
			var selector = new Selector ("Aapl_willShowingViewControllerPushWithSender");
			var target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("Aapl_willShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}

		public bool Aapl_willShowingDetailViewControllerPushWithSender ()
		{
			var selector = new Selector ("Aapl_willShowingDetailViewControllerPushWithSender");
			var target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("Aapl_willShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}

		public virtual AAPLPhoto Aapl_containedPhoto (AAPLPhoto photo)
		{
			return null;
		}

		public virtual bool Aapl_containsPhoto (AAPLPhoto photo)
		{
			return false;
		}

		public AAPLPhoto Aapl_currentVisibleDetailPhotoWithSender ()
		{
			var selector = new Selector ("Aapl_currentVisibleDetailPhotoWithSender");
			UIViewController target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("Aapl_currentVisibleDetailPhotoWithSender");
				return (AAPLPhoto)method.Invoke (target, new object[] { });
			} else {
				return null;
			}
		}
	}

	public class CustomViewController : UIViewController
	{
		public bool Aapl_willShowingViewControllerPushWithSender ()
		{
			var selector = new Selector ("Aapl_willShowingViewControllerPushWithSender");
			var target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("Aapl_willShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}

		public bool Aapl_willShowingDetailViewControllerPushWithSender ()
		{
			var selector = new Selector ("Aapl_willShowingDetailViewControllerPushWithSender");
			var target = this.GetTargetViewControllerForAction (selector, this);

			if (target != null) {
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("Aapl_willShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}

		public virtual AAPLPhoto Aapl_containedPhoto (AAPLPhoto photo)
		{
			return null;
		}

		public virtual bool Aapl_containsPhoto (AAPLPhoto photo)
		{
			return false;
		}
	}

	public class CustomNavigationController : UINavigationController
	{
		public CustomNavigationController (UIViewController viewController) : base (viewController)
		{
		}

		[Export ("Aapl_willShowingViewControllerPushWithSender")]
		public bool Aapl_willShowingDetailViewControllerPushWithSender ()
		{
			return true;
		}
	}

	public class CustomSplitViewController : UISplitViewController
	{
		[Export ("Aapl_willShowingViewControllerPushWithSender")]
		public bool Aapl_willShowingViewControllerPushWithSender ()
		{
			return false;
		}

		[Export ("Aapl_willShowingDetailViewControllerPushWithSender")]
		public bool Aapl_willShowingDetailViewControllerPushWithSender ()
		{
			if (this.Collapsed) {
				var target = this.ViewControllers [this.ViewControllers.Length - 1];
				var type = target.GetType ();
				MethodInfo method = type.GetMethod ("Aapl_willShowingDetailViewControllerPushWithSender");
				return (bool)method.Invoke (target, new object[] { });
			} else {
				return false;
			}
		}
	}
}

