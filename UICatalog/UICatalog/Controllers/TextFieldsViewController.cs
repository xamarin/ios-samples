using CoreGraphics;
using Foundation;
using System;
using System.Drawing;
using UIKit;

namespace UICatalog {
	public partial class TextFieldsViewController : UITableViewController, IUITextFieldDelegate {
		public TextFieldsViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.ConfigureCustomTextField ();
		}

		private void ConfigureCustomTextField ()
		{
			// Text fields with custom image backgrounds must have no border.
			customTextField.BorderStyle = UITextBorderStyle.None;
			customTextField.Background = UIImage.FromBundle ("text_field_background");

			// Create a purple button that, when selected, turns the custom text field's text color to purple.
			var purpleImage = UIImage.FromBundle ("text_field_purple_right_view");
			var purpleImageButton = new UIButton (UIButtonType.Custom);
			purpleImageButton.Bounds = new CGRect (PointF.Empty, purpleImage.Size);
			purpleImageButton.ImageEdgeInsets = new UIEdgeInsets (0, 0, 0, 5);
			purpleImageButton.SetImage (purpleImage, UIControlState.Normal);
			purpleImageButton.TouchUpInside += (sender, e) => {
				customTextField.TextColor = UIColor.Purple;
				Console.WriteLine ("The custom text field's purple right view button was clicked.");
			};

			customTextField.RightView = purpleImageButton;
			customTextField.RightViewMode = UITextFieldViewMode.Always;

			// Add an empty view as the left view to ensure inset between the text and the bounding rectangle.
			var leftPaddingView = new UIView (new RectangleF (0, 0, 10, 0));
			leftPaddingView.BackgroundColor = UIColor.Clear;
			customTextField.LeftView = leftPaddingView;
			customTextField.LeftViewMode = UITextFieldViewMode.Always;
		}

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder ();
			return true;
		}
	}
}
