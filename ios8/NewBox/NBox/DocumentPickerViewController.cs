using System;
using System.Drawing;

using Foundation;
using UIKit;

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

			SetupMoveExportButton (mode);
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
			Console.WriteLine ("OpenDocumentMoveExportClicked");
			var storeUrl = DocumentStorageUrl.Append(OriginalUrl.LastPathComponent, false);
			// Provide here a destination Url
			DismissGrantingAccess(storeUrl);
		}
	}
}
