using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class CustomToolbarViewController : UIViewController
	{
		[Outlet]
		private UIToolbar Toolbar { get; set; }

		#region UIBarButtonItem Creation and Configuration

		private UIBarButtonItem customImageBarButtonItem {
			get {
				var customImageBarButtonItem = new UIBarButtonItem (UIImage.FromBundle ("tools_icon"), UIBarButtonItemStyle.Plain, OnBarButtonItemClicked);
				customImageBarButtonItem.TintColor = ApplicationColors.Purple;

				return customImageBarButtonItem;
			}
		}

		private UIBarButtonItem FlexibleSpaceBarButtonItem {
			get {
				// Note that there's no target/action since this represents empty space.
				return new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace, null);
			}
		}

		private UIBarButtonItem CustomBarButtonItem {
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

		private void ConfigureToolbar()
		{
			Toolbar.SetBackgroundImage (UIImage.FromBundle ("toolbar_background"), UIToolbarPosition.Bottom, UIBarMetrics.Default);

			var toolbarButtonItems = new UIBarButtonItem[] {
				customImageBarButtonItem,
				FlexibleSpaceBarButtonItem,
				CustomBarButtonItem
			};
			Toolbar.SetItems (toolbarButtonItems, animated: true);
		}

		private void OnBarButtonItemClicked(object sender, EventArgs e)
		{
			Console.WriteLine ("A bar button item on the default toolbar was clicked: {0}.", sender);
		}
	}
}
