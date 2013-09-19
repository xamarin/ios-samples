using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TextKitDemo
{
	public class TextViewController : UIViewController
	{
		public DemoModel model;

		public TextViewController (IntPtr handle) : base (handle)
		{
		}

		public TextViewController () : base ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UIApplication.Notifications.ObserveContentSizeCategoryChanged (delegate {
				PreferredContentSizeChanged ();
			});
		}

		public virtual void PreferredContentSizeChanged ()
		{
		}
	}
}

