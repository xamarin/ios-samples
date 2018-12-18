using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("CustomToolbarViewController")]
	public class CustomToolbarViewController : UIViewController
	{
		[Outlet]
		UIToolbar Toolbar { get; set; }

		#region UIBarButtonItem Creation and Configuration

		UIBarButtonItem CustomImageBarButtonItem {
			get {
				var customImageBarButtonItem = new UIBarButtonItem (UIImage.FromBundle ("tools_icon"), UIBarButtonItemStyle.Plain, OnBarButtonItemClicked);
				customImageBarButtonItem.TintColor = ApplicationColors.Purple;

				return customImageBarButtonItem;
			}
		}

		UIBarButtonItem FlexibleSpaceBarButtonItem {
			get {
				// Note that there's no target/action since this represents empty space.
				return new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace, null);
			}
		}

		UIBarButtonItem CustomBarButtonItem {
			get {
				var barButtonItem = new UIBarButtonItem ("Button".Localize (), UIBarButtonItemStyle.Plain, OnBarButtonItemClicked);

				barButtonItem.SetBackgroundImage (UIImage.FromBundle ("WhiteButton"), UIControlState.Normal, UIBarMetrics.Default);

				var attributes = new UITextAttributes {
					TextColor = ApplicationColors.Purple
				};
				barButtonItem.SetTitleTextAttributes (attributes, UIControlState.Normal);

				return barButtonItem;
			}
		}

		#endregion

		public CustomToolbarViewController (IntPtr handle)
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
			Toolbar.SetBackgroundImage (UIImage.FromBundle ("toolbar_background"), UIToolbarPosition.Bottom, UIBarMetrics.Default);

			var toolbarButtonItems = new [] {
				CustomImageBarButtonItem,
				FlexibleSpaceBarButtonItem,
				CustomBarButtonItem
			};
			Toolbar.SetItems (toolbarButtonItems, true);
		}

		static void OnBarButtonItemClicked (object sender, EventArgs e)
		{
			Console.WriteLine ("A bar button item on the default toolbar was clicked: {0}.", sender);
		}
	}
}
