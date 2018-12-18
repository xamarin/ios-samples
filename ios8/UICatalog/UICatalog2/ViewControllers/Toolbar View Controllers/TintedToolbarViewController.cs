using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("TintedToolbarViewController")]
	public class TintedToolbarViewController : UIViewController
	{
		[Outlet]
		UIToolbar Toolbar { get; set; }

		#region UIBarButtonItem Creation and Configuration

		UIBarButtonItem RefreshBarButtonItem {
			get {
				return new UIBarButtonItem (UIBarButtonSystemItem.Refresh, OnBarButtonItemClicked);
			}
		}

		UIBarButtonItem FlexibleSpaceBarButtonItem {
			get {
				// Note that there's no target/action since this represents empty space.
				return new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace, null);
			}
		}

		UIBarButtonItem ActionBarButtonItem {
			get {
				return new UIBarButtonItem (UIBarButtonSystemItem.Action, OnBarButtonItemClicked);
			}
		}

		#endregion

		public TintedToolbarViewController (IntPtr handle)
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
			// See the UIBarStyle enum for more styles, including UIBarStyle.Default.
			Toolbar.BarStyle = UIBarStyle.Black;
			Toolbar.Translucent = true;

			Toolbar.TintColor = ApplicationColors.Green;
			Toolbar.BackgroundColor = ApplicationColors.Blue;

			var toolbarButtonItems = new [] {
				RefreshBarButtonItem,
				FlexibleSpaceBarButtonItem,
				ActionBarButtonItem
			};
			Toolbar.SetItems (toolbarButtonItems, true);
		}

		static void OnBarButtonItemClicked (object sender, EventArgs e)
		{
			Console.WriteLine ("A bar button item on the default toolbar was clicked: {0}.", sender);
		}
	}
}