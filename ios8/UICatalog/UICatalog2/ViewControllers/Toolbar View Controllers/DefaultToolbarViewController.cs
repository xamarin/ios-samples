using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("DefaultToolbarViewController")]
	public class DefaultToolbarViewController : UIViewController
	{
		[Outlet]
		UIToolbar Toolbar { get; set; }

		#region Lazy initialization

		UIBarButtonItem TrashBarButtonItem {
			get {
				return new UIBarButtonItem (UIBarButtonSystemItem.Trash, OnBarButtonItemClicked);
			}
		}

		UIBarButtonItem FlexibleSpaceBarButtonItem {
			get {
				return new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
			}
		}

		UIBarButtonItem CustomTitleBarButtonItem {
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

		void ConfigureToolbar ()
		{
			var toolbarButtonItems = new [] {
				TrashBarButtonItem,
				FlexibleSpaceBarButtonItem,
				CustomTitleBarButtonItem
			};

			Toolbar.SetItems (toolbarButtonItems, true);
		}

		static void OnBarButtonItemClicked (object sender, EventArgs e)
		{
			Console.WriteLine ("A bar button item on the default toolbar was clicked: {0}.", sender);
		}
	}
}