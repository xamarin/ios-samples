using System;

using Foundation;
using UIKit;

namespace ApplicationShortcuts
{
	public partial class ShortcutDetailViewController : UITableViewController, IUITextFieldDelegate, IUIPickerViewDataSource, IUIPickerViewDelegate
	{
		static readonly string[] pickerItems = { "Compose", "Play", "Pause", "Add", "Location", "Search", "Share" };

		[Outlet ("titleTextField")]
		public UITextField TitleTextField { get; set; }

		[Outlet ("subtitleTextField")]
		public UITextField SubtitleTextField { get; set; }

		[Outlet ("pickerView")]
		public UIPickerView PickerView { get; set; }

		[Outlet ("doneButton")]
		public UIBarButtonItem DoneButton { get; set; }

		// Used to share information between this controller and its parent.
		public UIApplicationShortcutItem ShortcutItem { get; set; }

		// The observer token for the `UITextFieldDidChangeNotification`.
		NSObject textFieldObserverToken;

		public ShortcutDetailViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Initialize the UI to reflect the values of the `shortcutItem`.
			var selectedShortcutItem = ShortcutItem;
			if (selectedShortcutItem == null)
				throw new InvalidProgramException ("The `selectedShortcutItem` was not set.");

			Title = selectedShortcutItem.LocalizedTitle;

			TitleTextField.Text = selectedShortcutItem.LocalizedTitle;
			SubtitleTextField.Text = selectedShortcutItem.LocalizedSubtitle;

			// Extract the raw value representing the icon from the userInfo dictionary, if provided.
			var userInfo = selectedShortcutItem.UserInfo;
			if (userInfo == null)
				return;

			var rawNumber = userInfo [AppDelegate.ApplicationShortcutUserInfoIconKey] as NSNumber;
			if (rawNumber == null)
				return;

			int row = rawNumber.Int32Value;

			// Select the matching row in the picker for the icon type.
			UIApplicationShortcutIconType iconType = IconTypeForSelectedRow (row);

			// The `iconType` returned may not align to the `iconRawValue` so use the `iconType`'s `rawValue`.
			PickerView.Select ((int)iconType, 0, false);

			textFieldObserverToken = UITextField.Notifications.ObserveTextFieldTextDidChange ((s, e) => {
				// You cannot dismiss the view controller without a valid shortcut title.
				DoneButton.Enabled = !string.IsNullOrEmpty (TitleTextField.Text);
			});
		}

		// Constructs a UIApplicationShortcutIconType based on the integer result from our picker.
		static UIApplicationShortcutIconType IconTypeForSelectedRow (int row)
		{
			var value = (UIApplicationShortcutIconType)row;
			bool isDefined = Enum.IsDefined (typeof(UIApplicationShortcutIconType), value);
			return isDefined ? value : UIApplicationShortcutIconType.Compose;
		}

		#region UITextFieldDelegate

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder ();
			return true;
		}

		#endregion

		#region UIPickerViewDataSource

		public nint GetComponentCount (UIPickerView pickerView)
		{
			return 1;
		}

		public nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			return pickerItems.Length;
		}

		[Export ("pickerView:titleForRow:forComponent:")]
		public string GetTitle (UIPickerView pickerView, nint row, nint component)
		{
			return pickerItems [(int)row];
		}

		#endregion

		#region UIStoryboardSegue Handling

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var selectedShortcutItem = ShortcutItem;
			if (selectedShortcutItem == null)
				throw new InvalidProgramException ("The `selectedShortcutItem` was not set.");

			if (segue.Identifier == "ShortcutDetailUpdated") {
				// In the updated case, create a shortcut item to represent the final state of the view controller.
				UIApplicationShortcutIconType iconType = IconTypeForSelectedRow ((int)PickerView.SelectedRowInComponent (0));
				var icon = UIApplicationShortcutIcon.FromType (iconType);

				var userInfo = new NSDictionary<NSString, NSObject> (AppDelegate.ApplicationShortcutUserInfoIconKey, new NSNumber (PickerView.SelectedRowInComponent (0)));
				ShortcutItem = new UIApplicationShortcutItem (selectedShortcutItem.Type, TitleTextField.Text ?? string.Empty, SubtitleTextField.Text, icon, userInfo);
			}
		}

		#endregion

		protected override void Dispose (bool disposing)
		{
			var token = textFieldObserverToken;
			if (token != null)
				token.Dispose ();

			base.Dispose (disposing);
		}
	}
}