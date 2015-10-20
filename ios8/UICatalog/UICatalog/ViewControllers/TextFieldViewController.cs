using System;
using System.Drawing;

using Foundation;
using UIKit;
using CoreGraphics;

namespace UICatalog
{
	[Register ("TextFieldViewController")]
	public class TextFieldViewController : UITableViewController
	{
		[Outlet]
		UITextField TextField { get; set; }

		[Outlet]
		UITextField TintedTextField { get; set; }

		[Outlet]
		UITextField SecureTextField { get; set; }

		[Outlet]
		UITextField SpecificKeyboardTextField { get; set; }

		[Outlet]
		UITextField CustomTextField { get; set; }

		public TextFieldViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureTextField ();
			ConfigureTintedTextField ();
			ConfigureSecureTextField ();
			ConfigureSpecificKeyboardTextField ();
			ConfigureCustomTextField ();
		}

		void ConfigureTextField ()
		{
			TextField.Placeholder = "Placeholder text".Localize ();
			TextField.AutocorrectionType = UITextAutocorrectionType.Yes;
			TextField.ReturnKeyType = UIReturnKeyType.Done;
			TextField.ClearButtonMode = UITextFieldViewMode.Never;
			TextField.AccessibilityIdentifier = "DEFAULT UITextField".Localize ();
		}

		void ConfigureTintedTextField ()
		{
			TintedTextField.TintColor = ApplicationColors.Blue;
			TintedTextField.TextColor = ApplicationColors.Green;

			TintedTextField.Placeholder = "Placeholder text".Localize ();
			TintedTextField.ReturnKeyType = UIReturnKeyType.Done;
			TintedTextField.ClearButtonMode = UITextFieldViewMode.Never;
			TintedTextField.AccessibilityIdentifier = "TINTED UITextField".Localize ();
		}

		void ConfigureSecureTextField ()
		{
			SecureTextField.SecureTextEntry = true;
			SecureTextField.Placeholder = "Placeholder text".Localize ();
			SecureTextField.ReturnKeyType = UIReturnKeyType.Done;
			SecureTextField.ClearButtonMode = UITextFieldViewMode.Always;
			SecureTextField.AccessibilityIdentifier = "SECURE UITextField".Localize ();
		}

		// There are many different types of keyboards that you may choose to use.
		// The different types of keyboards are defined in the UITextInputTraits interface.
		// This example shows how to display a keyboard to help enter email addresses.
		void ConfigureSpecificKeyboardTextField ()
		{
			SpecificKeyboardTextField.KeyboardType = UIKeyboardType.EmailAddress;
			SpecificKeyboardTextField.Placeholder = "Placeholder text".Localize ();
			SpecificKeyboardTextField.ReturnKeyType = UIReturnKeyType.Done;
			SpecificKeyboardTextField.AccessibilityIdentifier = "SPECIFIC KEYBOARD UITextField".Localize ();
		}

		void ConfigureCustomTextField ()
		{
			// Text fields with custom image backgrounds must have no border.
			CustomTextField.BorderStyle = UITextBorderStyle.None;

			CustomTextField.Background = UIImage.FromBundle ("text_field_background");

			// Create a purple button that, when selected, turns the custom text field's text color
			// to purple.
			var purpleImage = UIImage.FromBundle ("text_field_purple_right_view");
			var purpleImageButton = new UIButton (UIButtonType.Custom);
			purpleImageButton.Bounds = new CGRect (PointF.Empty, purpleImage.Size);
			purpleImageButton.ImageEdgeInsets = new UIEdgeInsets (top: 0, left: 0, bottom: 0, right: 5);
			purpleImageButton.SetImage (purpleImage, UIControlState.Normal);
			purpleImageButton.TouchUpInside += OnCustomTextFieldPurpleButtonClicked;
			CustomTextField.RightView = purpleImageButton;
			CustomTextField.RightViewMode = UITextFieldViewMode.Always;

			// Add an empty view as the left view to ensure inset between the text and the bounding rectangle.
			var leftPaddingView = new UIView (new RectangleF (0, 0, 10, 0));
			leftPaddingView.BackgroundColor = UIColor.Clear;
			CustomTextField.LeftView = leftPaddingView;
			CustomTextField.LeftViewMode = UITextFieldViewMode.Always;

			CustomTextField.Placeholder = "Placeholder text".Localize ();
			CustomTextField.AutocorrectionType = UITextAutocorrectionType.No;
			CustomTextField.ReturnKeyType = UIReturnKeyType.Done;
			CustomTextField.AccessibilityIdentifier = "CUSTOM UITextField".Localize ();
		}

		void OnCustomTextFieldPurpleButtonClicked (object sender, EventArgs e)
		{
			CustomTextField.TextColor = ApplicationColors.Purple;
			Console.WriteLine ("The custom text field's purple right view button was clicked.");
		}

		#region UITextFieldDelegate

		[Export ("textFieldShouldReturn:")]
		bool textFieldShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder ();
			return true;
		}

		#endregion
	}
}
