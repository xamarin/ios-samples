using System;

using UIKit;
using Foundation;

using ListerKit;
using System.Text;

namespace Lister
{
	[Register ("NewDocumentController")]
	public class NewDocumentController : UIViewController, IUITextFieldDelegate
	{
		[Outlet ("grayButton")]
		UIButton GrayButton { get; set; }

		[Outlet ("blueButton")]
		UIButton BlueButton { get; set; }

		[Outlet ("greenButton")]
		UIButton GreenButton { get; set; }

		[Outlet ("yellowButton")]
		UIButton YellowButton { get; set; }

		[Outlet ("orangeButton")]
		UIButton OrangeButton { get; set; }

		[Outlet ("redButton")]
		UIButton RedButton { get; set; }

		[Outlet ("saveButton")]
		UIBarButtonItem SaveButton { get; set; }

		[Outlet ("toolbar")]
		UIToolbar Toolbar { get; set; }

		[Outlet ("titleLabel")]
		UILabel TitleLabel { get; set; }

		[Outlet ("nameField")]
		UITextField NameField { get; set; }

		UIButton selectedButton;
		ListColor selectedColor;
		string selectedTitle;

		public ListsController ListsController { get; set; }

		public NewDocumentController (IntPtr handle)
			: base (handle)
		{
		}

		#region UITextFieldDelegate

		[Export ("textField:shouldChangeCharactersInRange:replacementString:")]
		public bool ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString)
		{
			var sb = new StringBuilder (textField.Text);
			sb.Remove ((int)range.Location, (int)range.Length);
			sb.Insert ((int)range.Location, replacementString);
			UpdateForProposedListName (sb.ToString ());

			return true;
		}

		[Export ("textFieldDidEndEditing:")]
		public void EditingEnded (UITextField textField)
		{
			UpdateForProposedListName (textField.Text);
		}

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder ();
			return true;
		}

		#endregion

		#region IBActions

		[Export ("pickColor:")]
		public void PickColor (UIButton sender)
		{
			// The user is choosing a color, resign first responder on the text field, if necessary.
			if (NameField.IsFirstResponder)
				NameField.ResignFirstResponder ();

			// Use the button's tag to determine the color.
			selectedColor = (ListColor)(int)sender.Tag;

			// Clear out the previously selected button's border.
			if (selectedButton != null)
				selectedButton.Layer.BorderWidth = 0;

			sender.Layer.BorderWidth = 5f;
			sender.Layer.BorderColor = UIColor.LightGray.CGColor;
			selectedButton = sender;

			TitleLabel.TextColor = AppColors.ColorFrom (selectedColor);
			Toolbar.TintColor = AppColors.ColorFrom (selectedColor);
		}

		[Export ("saveAction:")]
		public void SaveAction (NSObject sender)
		{
			List list = new List {
				Color = selectedColor
			};

			ListsController.CreateListInfoForList (list, selectedTitle);
			DismissViewController (true, null);
		}

		[Export ("cancelAction:")]
		public void CancelAction (NSObject sender)
		{
			DismissViewController (true, null);
		}

		#endregion

		#region Touch Handling

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			var touch = (UITouch)evt.AllTouches.AnyObject;

			// The user has tapped outside the text field, resign first responder, if necessary.
			if (NameField.IsFirstResponder && touch.View != NameField)
				NameField.ResignFirstResponder ();
		}

		#endregion

		void UpdateForProposedListName (string name)
		{
			if (ListsController.CanCreateListInfoWithName (name)) {
				SaveButton.Enabled = true;
				selectedTitle = name;
			} else {
				SaveButton.Enabled = false;
			}
		}
	}
}