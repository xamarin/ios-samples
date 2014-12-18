using System;
using System.Drawing;

using Foundation;
using UIKit;
using System.Text;

namespace NBox
{
	public partial class DocumentPickerViewController : UIDocumentPickerExtensionViewController
	{
		public DocumentPickerViewController (IntPtr handle) : base (handle)
		{
			Console.WriteLine (IntPtr.Size);
			Console.WriteLine ("DocumentPickerViewController ctor");
		}

		public override void PrepareForPresentation (UIDocumentPickerMode mode)
		{
			// TODO: present a view controller appropriate for picker mode here
			base.PrepareForPresentation (mode);

			Console.WriteLine ("DocumentPickerViewController PrepareForPresentation");
			Console.WriteLine ("UIDocumentPickerMode={0}", mode);

			// Contains the original file’s URL when in export or move mode.
			// Otherwise it contains null.
			Console.WriteLine ("OriginalUrl={0}", OriginalUrl);

			//  If you do not provide a File Provider extension, ProviderIdentifier equals null
			Console.WriteLine ("ProviderIdentifier={0}", ProviderIdentifier);

			//  If you do not provide a File Provider extension, it returns null
			Console.WriteLine ("DocumentStorageUrl={0}", DocumentStorageUrl);

			FillMovedImportedList ();
			SetupMoveExportButton (mode);
		}

		void FillMovedImportedList()
		{
			NSError error;
			string[] files = NSFileManager.DefaultManager.GetDirectoryContent (DocumentStorageUrl.Path, out error);
			if (error != null) {
				Console.WriteLine ("GetDirectoryContent error: {0}", error);
				return;
			}

			int number = 1;
			var sb = new StringBuilder ();
			foreach (var f in files)
				sb.AppendFormat ("{0}. {1}", number++, f);

			Console.WriteLine ("Moved or Imported:");
			Console.WriteLine (sb);
			MovedImportedList.Text = sb.ToString ();
		}

		void SetupMoveExportButton(UIDocumentPickerMode mode)
		{
			switch (mode) {
			case UIDocumentPickerMode.Import:
			case UIDocumentPickerMode.Open:
				HideMoveExportButton ();
				break;

			case UIDocumentPickerMode.ExportToService:
				ShowMoveExportButton ("Export to this location");
				break;

			case UIDocumentPickerMode.MoveToService:
				ShowMoveExportButton ("Move to this location");
				break;
			}
		}

		void HideMoveExportButton()
		{
			MoveExportBtn.Hidden = true;
			MoveExportBtn.Enabled = false;
		}

		void ShowMoveExportButton(string title)
		{
			MoveExportBtn.Hidden = false;
			MoveExportBtn.Enabled = true;
			MoveExportBtn.SetTitle (title, UIControlState.Normal);
		}

		partial void OpenDocument (NSObject sender)
		{
			Console.WriteLine ("DocumentPickerViewController OpenDocument");
			// DocumentStorageUrl - is read-only property contains the value returned by
			// your File Provider extension’s DocumentStorageURL property.
			// If you do not provide a File Provider extension, it returns null
			var documentURL = DocumentStorageUrl.Append ("TextDocument.txt", false);
			Console.WriteLine ("documentURL {0}", documentURL);

			// TODO: if you do not have a corresponding file provider, you must ensure that the URL returned here is backed by a file
			DismissGrantingAccess (documentURL);
		}

		partial void OnExportMoveClicked(NSObject sender)
		{
			Console.WriteLine ("DocumentPickerViewController MoveExportClicked");

			// Export/Move Document Picker mode:
			// Before calling DismissGrantingAccess method, copy the file to the selected destination.
			// Your extensions also need to track the file and make sure it is synced to your server (if needed).
			// After the copy is complete, call DismissGrantingAccess method, and provide the URL to the new copy

			NSError error;
			var destinationUrl = DocumentStorageUrl.Append(OriginalUrl.LastPathComponent, false);
			NSFileManager.DefaultManager.Copy(OriginalUrl.Path, destinationUrl.Path, out error);

			// Provide here a destination Url
			DismissGrantingAccess(destinationUrl);
		}
	}
}
