using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreText;

namespace UICatalog
{
	public partial class ButtonViewController : UITableViewController
	{
		[Outlet]
		UIButton SystemTextButton { get; set; }

		[Outlet]
		UIButton SystemContactAddButton { get; set; }

		[Outlet]
		UIButton SystemDetailDisclosureButton { get; set; }

		[Outlet]
		UIButton ImageButton { get; set; }

		[Outlet]
		UIButton AttributedTextButton { get; set; }

		public ButtonViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			// All of the buttons are created in the storyboard, but configured below.
			ConfigureSystemTextButton ();
			ConfigureSystemContactAddButton ();
			ConfigureSystemDetailDisclosureButton ();
			ConfigureImageButton ();
			ConfigureAttributedTextSystemButton ();
		}

		private void ConfigureSystemTextButton()
		{
			SystemTextButton.SetTitle ("Button".Localize (), UIControlState.Normal);
			SystemTextButton.TouchUpInside += OnButtonClicked;
		}

		private void ConfigureSystemContactAddButton()
		{
			SystemContactAddButton.BackgroundColor = UIColor.Clear;
			SystemContactAddButton.TouchUpInside += OnButtonClicked;
		}

		private void ConfigureSystemDetailDisclosureButton()
		{
			SystemDetailDisclosureButton.BackgroundColor = UIColor.Clear;
			SystemDetailDisclosureButton.TouchUpInside += OnButtonClicked;
		}

		private void ConfigureImageButton()
		{
			// Remove the title text.
			ImageButton.SetTitle ("", UIControlState.Normal);
			ImageButton.TintColor = ApplicationColors.Purple;

			ImageButton.SetImage (UIImage.FromBundle ("x_icon"), UIControlState.Normal);

			// Add an accessibility label to the image.
			ImageButton.AccessibilityLabel = "X Button".Localize ();
			ImageButton.TouchUpInside += OnButtonClicked;
		}

		private void ConfigureAttributedTextSystemButton()
		{
			UIStringAttributes attribs = new UIStringAttributes {
				ForegroundColor = ApplicationColors.Blue,
				StrikethroughStyle = NSUnderlineStyle.Single,
			};
			var titleAttributes = new NSAttributedString ("Button".Localize(), attribs);
			AttributedTextButton.SetAttributedTitle (titleAttributes, UIControlState.Normal);

			var highlightedTitleAttributes = new UIStringAttributes {
				ForegroundColor = UIColor.Green,
				StrikethroughStyle = NSUnderlineStyle.Thick
			};
			var highlightedAttributedTitle = new NSAttributedString ("Button".Localize (), highlightedTitleAttributes);
			AttributedTextButton.SetAttributedTitle (highlightedAttributedTitle, UIControlState.Highlighted);

			AttributedTextButton.TouchUpInside += OnButtonClicked;
		}

		private void OnButtonClicked(object sender, EventArgs e)
		{
			Console.WriteLine ("A button was clicked. sender: {0}", sender);
		}
	}
}
