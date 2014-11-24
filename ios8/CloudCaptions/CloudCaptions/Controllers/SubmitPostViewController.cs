using System;
using System.Threading.Tasks;

using UIKit;
using Foundation;
using CloudKit;

namespace CloudCaptions
{
	[Register("SubmitPostViewController")]
	public class SubmitPostViewController : UIViewController
	{
		class PickerViewModel : UIPickerViewModel
		{
			SubmitPostViewController controller;

			public PickerViewModel (SubmitPostViewController controller)
			{
				this.controller = controller;
			}

			public override nint GetComponentCount (UIPickerView picker)
			{
				return 1;
			}

			public override nint GetRowsInComponent (UIPickerView picker, nint component)
			{
				// One row for each font in the familyNames array
				return UIFont.FamilyNames.Length;
			}

			public override UIView GetView (UIPickerView picker, nint row, nint component, UIView view)
			{
				// Sets each item in pickerview as the name of each font with its typeface in its own font
				// (e.g. Helvetica appears in Helvetica, Courier New appears in Courier New
				UILabel fontLabel = (UILabel)view ?? new UILabel ();

				var familyName = UIFont.FamilyNames [row];
				fontLabel.Font = UIFont.FromName (familyName, 24);
				fontLabel.Text = familyName;

				return fontLabel;
			}

			public override nfloat GetRowHeight (UIPickerView picker, nint component)
			{
				// This method sets the height of each row in the pickerView
				return 35;
			}

			public override void Selected (UIPickerView picker, nint row, nint component)
			{
				// Sets the imageLabel font to the selected font
				string fontName = UIFont.FamilyNames [row];
				controller.ImageLabel.Font = UIFont.FromName (fontName, 24);
			}
		}

		public MainViewController MainController { get; set; }

		public Image ImageRecord { get; set; }

		[Outlet ("progressBar")]
		UIProgressView ProgressBar { get; set; }

		[Outlet ("imageView")]
		UIImageView ImageView { get; set; }

		[Outlet ("fontPicker")]
		UIPickerView FontPicker { get; set; }

		[Outlet ("hiddenText")]
		UITextField HiddenText { get; set; }

		[Outlet ("tagField")]
		UITextField TagField { get; set; }

		[Outlet ("imageLabel")]
		UILabel ImageLabel { get; set; }

		[Outlet ("postButton")]
		UIBarButtonItem PostButton { get; set; }

		UIPickerViewModel model;

		public SubmitPostViewController (IntPtr handle)
			: base (handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Sets the preview to the image record passed in
			ImageView.Image = ImageRecord.FullImage;

			// sets up font picker and picks a random font
			model = new PickerViewModel (this);
			FontPicker.Model = model;
			var randomFont = (new Random ()).Next () % UIFont.FamilyNames.Length;
			FontPicker.Select (randomFont, 0, false);

			// Sets up the label with random font
			ImageLabel.Font = UIFont.FromName (UIFont.FamilyNames [randomFont], 24);

			// sets delegates so enter dimsisses keyboard
			TagField.ShouldReturn = TextFieldShouldReturn;
			HiddenText.ShouldReturn = TextFieldShouldReturn;

			// typing into the hiddent text field automatically updates the label on the image
			HiddenText.EditingChanged += OnDidEditField;

			// start editing the text field as soon as the view is done loading
			HiddenText.BecomeFirstResponder ();
		}

		bool TextFieldShouldReturn (UITextField textField)
		{
			// This method is called to dismiss the keyboard
			View.EndEditing (true);
			return true;
		}

		void OnDidEditField (object sender, EventArgs e)
		{
			// This is called when the user types into a textField. Keeps the label up to date
			ImageLabel.Text = HiddenText.Text;
		}

		#region IBActions

		[Export ("editText:")]
		void EditText (NSObject sender)
		{
			// Pulls up the keyboard for the hidden text field when photo is tapped
			HiddenText.BecomeFirstResponder ();
		}

		[Export ("cancelPost:")]
		void CancelPost (NSObject sender)
		{
			// Hides the keyboards and then returns back to SubmitPostViewController
			HiddenText.EndEditing (true);
			TagField.EndEditing (true);
			DismissViewController (true, null);
		}

		[Export ("publishPost:")]
		void PublishPost (NSObject sender)
		{
			// Prevents multiple posting, locks as soon as a post is made
			PostButton.Enabled = false;
			UIActivityIndicatorView indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
			indicator.StartAnimating ();
			PostButton.CustomView = indicator;

			// Hides the keyboards and dispatches a UI update to show the upload progress
			HiddenText.EndEditing (true);
			TagField.EndEditing (true);
			ProgressBar.Hidden = false;

			// Creates post record type and initizalizes all of its values
			CKRecord newRecord = new CKRecord (Post.RecordType);
			newRecord [Post.FontKey] = (NSString)ImageLabel.Font.Name;
			newRecord [Post.ImageRefKey] = new CKReference (ImageRecord.Record.Id, CKReferenceAction.DeleteSelf);
			newRecord [Post.TextKey] = (NSString)HiddenText.Text;
			string[] tags = TagField.Text.ToLower ().Split (new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
			newRecord [Post.TagsKey] = NSArray.FromObjects (tags);

			Post newPost = new Post (newRecord);
			newPost.ImageRecord = ImageRecord;

			// Only upload image record if it is not on server, otherwise just upload the new post record
			CKRecord[] recordsToSave = ImageRecord.IsOnServer
				? new CKRecord[] { newRecord }
				: new CKRecord[] { newRecord, ImageRecord.Record };
			// TODO: https://trello.com/c/A9T8Spyp second param is null
			CKModifyRecordsOperation saveOp = new CKModifyRecordsOperation (recordsToSave, new CKRecordID[0]);
			saveOp.PerRecordProgress = (CKRecord record, double progress) => {
				// Image record type is probably going to take the longest to upload. Reflect it's progress in the progress bar
				if (record.RecordType == Image.RecordType)
					InvokeOnMainThread (() => {
						var val = (float)(progress * 0.95);
						ProgressBar.SetProgress (val, true);
					});
			};

			// When completed it notifies the tableView to add the post we just uploaded, displays error if it didn't work
			saveOp.Completed = (CKRecord[] savedRecords, CKRecordID[] deletedRecordIDs, NSError operationError) => {
				Error errorResponse = HandleError (operationError);
				switch (errorResponse) {
					case Error.Success:
						// Tells delegate to update so it can display our new post
						InvokeOnMainThread (() => {
							DismissViewController (true, null);
							MainController.Submit (newPost);
						});
						break;

					case Error.Retry:
						CKErrorInfo errorInfo = new CKErrorInfo (operationError.UserInfo);
						nint retryAfter = errorInfo.RetryAfter.HasValue ? errorInfo.RetryAfter.Value : 3;
						Console.WriteLine ("Error: {0}. Recoverable, retry after {1} seconds", operationError.Description, retryAfter);
						Task.Delay ((int)retryAfter * 1000).ContinueWith (_ => PublishPost (sender));
						break;

					case Error.Ignore:
						Console.WriteLine ("Error saving record: {0}", operationError.Description);

						string errorTitle = "Error";
						string dismissButton = "Okay";
						string errorMessage = operationError.Code == (long)CKErrorCode.NotAuthenticated
							? "You must be logged in to iCloud in order to post"
							: "Unrecoverable error with the upload, check console logs";

						InvokeOnMainThread (() => {
							UIAlertController alert = UIAlertController.Create (errorTitle, errorMessage, UIAlertControllerStyle.Alert);
							alert.AddAction (UIAlertAction.Create (dismissButton, UIAlertActionStyle.Cancel, null));

							PostButton.Enabled = true;
							PresentViewController (alert, true, null);
							ProgressBar.Hidden = true;
							PostButton.CustomView = null;
						});
						break;

					default:
						throw new NotImplementedException ();
				}
			};
			CKContainer.DefaultContainer.PublicCloudDatabase.AddOperation (saveOp);
		}

		Error HandleError (NSError operationError)
		{
			if (operationError == null) {
				return Error.Success;
			}
			switch ((CKErrorCode)(long)operationError.Code) {
				case CKErrorCode.UnknownItem:
					// This error occurs if it can't find the subscription named autoUpdate. (It tries to delete one that doesn't exits or it searches for one it can't find)
					// This is okay and expected behavior
					return Error.Ignore;

				case CKErrorCode.NetworkUnavailable:
				case CKErrorCode.NetworkFailure:
					// A reachability check might be appropriate here so we don't just keep retrying if the user has no service
				case CKErrorCode.ServiceUnavailable:
				case CKErrorCode.RequestRateLimited:
					return Error.Retry;

				case CKErrorCode.PartialFailure:
					// This shouldn't happen on a query operation
				case CKErrorCode.NotAuthenticated:
				case CKErrorCode.BadDatabase:
				case CKErrorCode.IncompatibleVersion:
				case CKErrorCode.BadContainer:
				case CKErrorCode.PermissionFailure:
				case CKErrorCode.MissingEntitlement:
					// This app uses the publicDB with default world readable permissions
				case CKErrorCode.AssetFileNotFound:
				case CKErrorCode.AssetFileModified:
					// Users don't really have an option to delete files so this shouldn't happen
				case CKErrorCode.QuotaExceeded:
					// We should not retry if it'll exceed our quota
				case CKErrorCode.OperationCancelled:
					// Nothing to do here, we intentionally cancelled
				case CKErrorCode.InvalidArguments:
				case CKErrorCode.ResultsTruncated:
				case CKErrorCode.ServerRecordChanged:
				case CKErrorCode.ChangeTokenExpired:
				case CKErrorCode.BatchRequestFailed:
				case CKErrorCode.ZoneBusy:
				case CKErrorCode.ZoneNotFound:
				case CKErrorCode.LimitExceeded:
				case CKErrorCode.UserDeletedZone:
					// All of these errors are irrelevant for this save operation. We're only saving new records, not modifying old ones
				case CKErrorCode.InternalError:
				case CKErrorCode.ServerRejectedRequest:
				case CKErrorCode.ConstraintViolation:
					//Non-recoverable, should not retry
				default:
					return Error.Ignore;
			}
		}

		#endregion

	}
}

