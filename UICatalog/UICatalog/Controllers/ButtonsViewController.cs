using Foundation;
using System;
using UIKit;

namespace UICatalog {
	public partial class ButtonsViewController : UITableViewController {
		public ButtonsViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureAttributedTextSystemButton ();
		}

		private void ConfigureAttributedTextSystemButton ()
		{
			var attributes = new UIStringAttributes {
				ForegroundColor = UIColor.Blue,
				StrikethroughStyle = NSUnderlineStyle.Single,
			};

			var titleAttributes = new NSAttributedString ("Button", attributes);
			attributedButton.SetAttributedTitle (titleAttributes, UIControlState.Normal);

			var highlightedTitleAttributes = new UIStringAttributes {
				ForegroundColor = UIColor.Green,
				StrikethroughStyle = NSUnderlineStyle.Thick
			};

			var highlightedAttributedTitle = new NSAttributedString ("Button", highlightedTitleAttributes);
			attributedButton.SetAttributedTitle (highlightedAttributedTitle, UIControlState.Highlighted);
		}

		partial void AttributedButtonTouched (NSObject sender)
		{
			Console.WriteLine ("'Attributed' button was clicked");
		}

		partial void ImageButtonTouched (NSObject sender)
		{
			Console.WriteLine ("'Image' button was clicked");
		}

		partial void SystemButtonTouched (NSObject sender)
		{
			Console.WriteLine ("'System' button was clicked");
		}

		partial void SystemDetailsButtonTouched (NSObject sender)
		{
			Console.WriteLine ("'System Details' button was clicked");
		}

		partial void SystemContactButtonTouched (NSObject sender)
		{
			Console.WriteLine ("'System Contact' button was clicked");
		}
	}
}
