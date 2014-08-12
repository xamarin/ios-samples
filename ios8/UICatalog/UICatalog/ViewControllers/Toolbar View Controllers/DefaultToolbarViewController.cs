using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class DefaultToolbarViewController : UIViewController
	{
		[Outlet]
		private UIToolbar Toolbar { get; set; }

		#region Lazy initialization

		private UIBarButtonItem TrashBarButtonItem {
			get {
				return new UIBarButtonItem (UIBarButtonSystemItem.Trash, OnBarButtonItemClicked);
			}
		}

		private UIBarButtonItem FlexibleSpaceBarButtonItem {
			get {
				return new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
			}
		}

		private UIBarButtonItem CustomTitleBarButtonItem {
			get {
				return new UIBarButtonItem ("Action".Localize (), UIBarButtonItemStyle.Plain, OnBarButtonItemClicked);
			}
		}

		#endregion

		public DefaultToolbarViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureToolbar ();
		}

		private void ConfigureToolbar()
		{
			var toolbarButtonItems = new UIBarButtonItem[] {
				TrashBarButtonItem,
				FlexibleSpaceBarButtonItem,
				CustomTitleBarButtonItem
			};

			Toolbar.SetItems (toolbarButtonItems, animated: true);
		}

		private void OnBarButtonItemClicked(object sender, EventArgs e)
		{
			Console.WriteLine ("A bar button item on the default toolbar was clicked: {0}.", sender);
		}
	}
}
