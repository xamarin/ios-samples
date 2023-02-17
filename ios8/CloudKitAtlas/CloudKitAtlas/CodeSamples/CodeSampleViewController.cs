using System;
using System.Linq;

using UIKit;
using Foundation;
using CoreLocation;
using CoreAnimation;
using CoreGraphics;

using static CloudKitAtlas.NullableExtensions;
using System.Diagnostics.Contracts;

namespace CloudKitAtlas {
	public partial class CodeSampleViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource,
	IUIScrollViewDelegate, IUITextFieldDelegate, ICLLocationManagerDelegate, IUIImagePickerControllerDelegate,
	IUINavigationControllerDelegate, IUIPickerViewDelegate, IUIPickerViewDataSource {
		CLLocationManager LocationManager { get; } = new CLLocationManager ();
		UIImagePickerController ImagePickerController { get; } = new UIImagePickerController ();

		[Outlet]
		public NSLayoutConstraint PickerHeightConstraint { get; set; }

		[Outlet]
		public UIPickerView PickerView { get; set; }

		[Outlet]
		public TableView TableView { get; set; }

		[Outlet]
		public UILabel ClassName { get; set; }

		[Outlet]
		public UILabel MethodName { get; set; }

		[Outlet]
		public UIBarButtonItem RunButton { get; set; }

		[Outlet]
		UILabel CodeSampleDescription { get; set; }

		int? selectedLocationCellIndex;
		int? selectedImageCellIndex;
		int? selectedSelectionCellIndex;

		public string GroupTitle { get; set; }
		public CodeSample SelectedCodeSample { get; set; }

		public CodeSampleViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public CodeSampleViewController (NSCoder coder)
			: base (coder)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var codeSample = SelectedCodeSample;
			if (codeSample != null) {
				ClassName.Text = $"Class: {codeSample.ClassName}";
				MethodName.Text = codeSample.MethodName;
				CodeSampleDescription.Text = codeSample.Description;
			}

			var groupTitle = GroupTitle;
			if (!string.IsNullOrWhiteSpace (groupTitle))
				NavigationItem.Title = groupTitle;
			NavigationItem.HidesBackButton = NavigationController.ViewControllers [0].NavigationItem.HidesBackButton;

			var border = new CALayer {
				BackgroundColor = new UIColor (0.91f, 0.91f, 0.91f, 1).CGColor,
				Frame = new CGRect (0, 0, TableView.Bounds.Width, 1)
			};
			TableView.Layer.AddSublayer (border);

			LocationManager.Delegate = this;

			ImagePickerController.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			ImagePickerController.Delegate = this;

			PickerView.Delegate = this;
			PickerView.DataSource = this;

			ValidateInputs ();
		}

		void ValidateInputs ()
		{
			var codeSample = SelectedCodeSample;
			RunButton.Enabled = (codeSample == null) || codeSample.Inputs.All (input => input.IsValid);
		}

		#region Table view data source

		[Export ("numberOfSectionsInTableView:")]
		public nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public nint RowsInSection (UITableView tableView, nint section)
		{
			var codeSample = SelectedCodeSample;
			var cnt = codeSample == null ? 0 : codeSample.Inputs.Count (cs => !cs.IsHidden);
			return cnt;
		}

		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var codeSample = SelectedCodeSample;
			if (codeSample != null) {
				var inputs = codeSample.Inputs.Where (i => !i.IsHidden).ToArray ();
				var input = inputs [indexPath.Row];

				var textInput = input as TextInput;
				if (textInput != null) {
					var textCell = tableView.DequeueReusableCell ("TextFieldCell", indexPath) as TextFieldTableViewCell;
					if (textInput != null && textCell != null) {
						textCell.TextInput = textInput;
						textCell.FieldLabel.Text = textInput.Label;
						textCell.TextField.Text = textInput.Value;

						if (textInput.Type == TextInputType.Email)
							textCell.TextField.KeyboardType = UIKeyboardType.EmailAddress;
						textCell.TextField.Delegate = this;

						if (indexPath.Row == 0)
							textCell.TextField.BecomeFirstResponder ();

						return textCell;
					}
				}

				var locationInput = input as LocationInput;
				if (locationInput != null) {
					var locationCell = tableView.DequeueReusableCell ("LocationFieldCell", indexPath) as LocationFieldTableViewCell;
					if (locationCell != null) {
						locationCell.LocationInput = locationInput;
						locationCell.FieldLabel.Text = input.Label;

						locationCell.LongitudeField.Text = locationInput?.Longitude.ToString ();
						locationCell.LatitudeField.Text = locationInput?.Latitude.ToString ();

						locationCell.LongitudeField.RemoveTarget (EditingChanged, UIControlEvent.EditingChanged);
						locationCell.LatitudeField.RemoveTarget (EditingChanged, UIControlEvent.EditingChanged);
						locationCell.LongitudeField.AddTarget (EditingChanged, UIControlEvent.EditingChanged);
						locationCell.LatitudeField.AddTarget (EditingChanged, UIControlEvent.EditingChanged);

						locationCell.LongitudeField.Delegate = this;
						locationCell.LatitudeField.Delegate = this;

						if (indexPath.Row == 0)
							locationCell.LatitudeField.BecomeFirstResponder ();
						locationCell.LookUpButton.Enabled = CLLocationManager.Status != CLAuthorizationStatus.Denied;

						return locationCell;
					}
				}

				var imgInput = input as ImageInput;
				if (imgInput != null) {
					var imgCell = tableView.DequeueReusableCell ("ImageFieldCell", indexPath) as ImageFieldTableViewCell;
					if (imgInput != null && imgCell != null) {
						imgCell.FieldLabel.Text = imgInput.Label;
						imgCell.ImageInput = imgInput;

						return imgCell;
					}
				}

				var boolInput = input as BooleanInput;
				if (boolInput != null) {
					var boolCell = tableView.DequeueReusableCell ("BooleanFieldCell", indexPath) as BooleanFieldTableViewCell;
					if (boolCell != null && boolInput != null) {
						boolCell.FieldLabel.Text = boolInput.Label;
						boolCell.BooleanField.On = boolInput.Value;
						boolCell.BooleanInput = boolInput;

						return boolCell;
					}
				}

				var selInput = input as SelectionInput;
				if (selInput != null) {
					var selCell = tableView.DequeueReusableCell ("SelectionFieldCell", indexPath) as SelectionFieldTableViewCell;
					if (selInput != null && selCell != null) {
						selCell.FieldLabel.Text = selInput.Label;
						var index = ValueOrDefault (selInput.Value);
						selCell.SelectedItemLabel.Text = selInput.Items.Count > 0 ? selInput.Items [index].Label : string.Empty;
						selCell.SelectionInput = selInput;

						return selCell;
					}
				}
			}

			return tableView.DequeueReusableCell ("FormFieldCell", indexPath);
		}

		[Export ("tableView:heightForRowAtIndexPath:")]
		public nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var codeSample = SelectedCodeSample;
			if (codeSample != null && codeSample.Inputs [indexPath.Row] is ImageInput)
				return 236;

			return tableView.RowHeight;
		}

		#endregion

		#region UITextFieldDelegate

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder ();
			return true;
		}

		void EditingChanged (object sender, EventArgs e)
		{
			EditingEnded ((UITextField) sender);
		}

		[Export ("textFieldDidEndEditing:")]
		public void EditingEnded (UITextField textField)
		{
			var contentView = textField.Superview;
			if (contentView == null)
				return;

			do {
				var textCell = contentView.Superview as TextFieldTableViewCell;
				if (textCell != null) {
					textCell.TextInput.Value = textField.Text ?? string.Empty;
					break;
				}

				var stackView = contentView.Superview;
				var stackCell = stackView?.Superview as LocationFieldTableViewCell;
				if (stackCell != null) {
					int? value = Parse (textField.Text);
					if (textField.Tag == 0)
						stackCell.LocationInput.Latitude = value;
					else if (textField.Tag == 1)
						stackCell.LocationInput.Longitude = value;
				}
			} while (false);

			ValidateInputs ();
		}

		[Export ("textFieldDidBeginEditing:")]
		public void EditingStarted (UITextField textField)
		{
			var contentView = textField.Superview;
			var stackView = contentView.Superview;
			var cell = stackView.Superview as LocationFieldTableViewCell;
			var errorLabel = cell?.ErrorLabel;

			if (errorLabel != null) {
				errorLabel.Hidden = true;
				errorLabel.LayoutIfNeeded ();
			}
		}

		#endregion

		#region UIPickerViewDataSource

		public nint GetComponentCount (UIPickerView pickerView)
		{
			return 1;
		}

		public nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			if (selectedSelectionCellIndex.HasValue) {
				var indexPath = NSIndexPath.FromRowSection (selectedSelectionCellIndex.Value, 0);
				var cell = TableView.CellAt (indexPath) as SelectionFieldTableViewCell;
				if (cell != null)
					return cell.SelectionInput.Items.Count;
			}
			return 0;
		}

		#endregion

		#region UIPickerViewDelegate

		[Export ("pickerView:titleForRow:forComponent:")]
		public string GetTitle (UIPickerView pickerView, nint row, nint component)
		{
			if (selectedSelectionCellIndex.HasValue) {
				var indexPath = NSIndexPath.FromRowSection (selectedSelectionCellIndex.Value, 0);
				var cell = TableView.CellAt (indexPath) as SelectionFieldTableViewCell;
				if (cell != null)
					return cell.SelectionInput.Items [(int) row].Label;
			}
			return null;
		}

		[Export ("pickerView:didSelectRow:inComponent:")]
		public void Selected (UIPickerView pickerView, nint row, nint component)
		{
			if (selectedSelectionCellIndex.HasValue) {
				var indexPath = NSIndexPath.FromRowSection (selectedSelectionCellIndex.Value, 0);
				var cell = TableView.CellAt (indexPath) as SelectionFieldTableViewCell;
				if (cell != null) {
					cell.SelectedItemLabel.Text = cell.SelectionInput.Items [(int) row].Label;
					UIView.AnimateNotify (0.4, () => {
						PickerHeightConstraint.Constant = 0;
						View.LayoutIfNeeded ();
					}, completed => {
						if (!completed)
							return;

						var oldValue = cell.SelectionInput.Value;
						if (oldValue.HasValue) {
							foreach (var index in cell.SelectionInput.Items [oldValue.Value].ToggleIndexes)
								SelectedCodeSample.Inputs [index].IsHidden = true;
						}
						foreach (var index in cell.SelectionInput.Items [(int) row].ToggleIndexes)
							SelectedCodeSample.Inputs [index].IsHidden = false;

						cell.SelectionInput.Value = (int) row;
						TableView.ReloadData ();
					});
				}
			}
		}

		#endregion

		#region CLLocationManagerDelegate

		[Export ("locationManager:didChangeAuthorizationStatus:")]
		public void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status)
		{
			if (!selectedLocationCellIndex.HasValue)
				return;

			int index = selectedImageCellIndex.Value;
			var indexPath = NSIndexPath.FromRowSection (index, 0);
			var cell = (LocationFieldTableViewCell) TableView.CellAt (indexPath);

			cell.LookUpButton.Enabled = status != CLAuthorizationStatus.Denied;
			if (status == CLAuthorizationStatus.AuthorizedWhenInUse)
				RequestLocationForCell (cell);
		}

		[Export ("locationManager:didFailWithError:")]
		public void Failed (CLLocationManager manager, NSError error)
		{
			if (!selectedLocationCellIndex.HasValue)
				return;

			var index = selectedImageCellIndex.Value;
			var indexPath = NSIndexPath.FromRowSection (index, 0);

			var cell = TableView.CellAt (indexPath) as LocationFieldTableViewCell;
			EndLocationLookupForCell (cell);
			cell.ErrorLabel.Hidden = false;
			cell.ErrorLabel.LayoutIfNeeded ();
		}

		[Export ("locationManager:didUpdateLocations:")]
		public void LocationsUpdated (CLLocationManager manager, CLLocation [] locations)
		{
			if (!selectedLocationCellIndex.HasValue)
				return;

			var index = selectedImageCellIndex.Value;
			var indexPath = NSIndexPath.FromRowSection (index, 0);
			var cell = (LocationFieldTableViewCell) TableView.CellAt (indexPath);

			EndLocationLookupForCell (cell);

			var location = locations.LastOrDefault ();
			if (location != null) {
				cell.SetCoordinate (location.Coordinate);
				ValidateInputs ();
			}
		}

		void RequestLocationForCell (LocationFieldTableViewCell cell)
		{
			LocationManager.DesiredAccuracy = CLLocation.AccuracyThreeKilometers;
			LocationManager.RequestLocation ();

			cell.LatitudeField.Enabled = false;
			cell.LongitudeField.Enabled = false;

			cell.Spinner.StartAnimating ();
			cell.Spinner.LayoutIfNeeded ();
		}

		void EndLocationLookupForCell (LocationFieldTableViewCell cell)
		{
			cell.LatitudeField.Enabled = true;
			cell.LongitudeField.Enabled = true;
			cell.LookUpButton.Enabled = true;
			cell.Spinner.StopAnimating ();
		}

		#endregion

		#region UIImagePickerControllerDelegate

		[Export ("imagePickerControllerDidCancel:")]
		public void Canceled (UIImagePickerController picker)
		{
			picker.DismissViewController (true, null);
		}

		[Export ("imagePickerController:didFinishPickingMediaWithInfo:")]
		public void FinishedPickingMedia (UIImagePickerController picker, NSDictionary info)
		{
			var selectedImage = (UIImage) info [UIImagePickerController.OriginalImage];
			var imageUrl = GetImageUrl ();
			if (selectedImage != null && selectedImageCellIndex.HasValue && imageUrl != null) {
				var index = selectedImageCellIndex.Value;
				var indexPath = NSIndexPath.FromRowSection (index, 0);
				var cell = (ImageFieldTableViewCell) TableView.CellAt (indexPath);
				var imageData = selectedImage.AsJPEG (0.8f);
				imageData?.Save (imageUrl, atomically: true);
				cell.AssetView.Image = selectedImage;
				cell.ImageInput.Value = imageUrl;
			}
			picker.DismissViewController (true, null);
		}

		NSUrl GetImageUrl ()
		{
			if (!selectedImageCellIndex.HasValue)
				return null;

			var index = selectedImageCellIndex.Value;
			var manager = NSFileManager.DefaultManager;

			NSError error;
			var directoyUrl = manager.GetUrl (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, null, true, out error);

			var tempImageName = $"ck_catalog_tmp_image_{index}";
			return directoyUrl.Append (tempImageName, false);
		}

		#endregion

		#region Actions

		[Action ("pickImage:")]
		void PickImage (UIButton sender)
		{
			var contentView = sender.Superview;
			var cell = contentView.Superview as ImageFieldTableViewCell;
			if (cell != null) {
				selectedImageCellIndex = TableView.IndexPathForCell (cell).Row;
				PresentViewController (ImagePickerController, true, null);
			}
		}

		[Action ("runCode:")]
		public void RunCode (UIBarButtonItem sender)
		{
			var codeSample = SelectedCodeSample;
			if (codeSample != null) {
				var error = codeSample.Error;
				if (error != null) {
					var alertController = UIAlertController.Create ("Invalid Parameter", error, UIAlertControllerStyle.Alert);
					alertController.AddAction (UIAlertAction.Create ("Dismiss", UIAlertActionStyle.Default, null));
					PresentViewController (alertController, true, null);
				} else {
					NavigationController.PerformSegue ("ShowLoadingView", new SegueArg { Sample = codeSample });
				}
			}
		}

		[Action ("lookUpLocation:")]
		void LookUpLocation (UIButton sender)
		{
			var stackView = sender.Superview;
			var contentView = stackView.Superview;
			var cell = contentView.Superview as LocationFieldTableViewCell;
			if (cell != null) {
				cell.ErrorLabel.Hidden = true;
				cell.LookUpButton.Enabled = false;
				selectedLocationCellIndex = TableView.IndexPathForCell (cell).Row;

				if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
					LocationManager.RequestWhenInUseAuthorization ();
				else
					RequestLocationForCell (cell);
			}
		}

		[Action ("selectOption:")]
		void SelectOption (UITapGestureRecognizer sender)
		{
			var location = sender.LocationInView (TableView);
			var indexPath = TableView.IndexPathForRowAtPoint (location);
			if (indexPath != null) {
				selectedSelectionCellIndex = indexPath.Row;
				PickerView.ReloadComponent (0);

				UIView.Animate (0.4, () => {
					PickerHeightConstraint.Constant = 200;
					View.LayoutIfNeeded ();
				});
			}
		}

		#endregion

		static int? Parse (string text)
		{
			int value;
			return int.TryParse (text, out value) ? value : (int?) null;
		}
	}
}
