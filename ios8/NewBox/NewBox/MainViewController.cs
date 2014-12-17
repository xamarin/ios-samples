using System;

using UIKit;
using Foundation;
using MobileCoreServices;

namespace NewBox
{
	[Register ("MainViewController")]
	public class MainViewController : UIViewController
	{
		NSUrl documentURL;
		string[] allowedUTIs;

		public MainViewController(IntPtr handle)
			: base(handle)
		{
		}

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// For demo we put all available UTI types
			#region Init UTIs
			allowedUTIs = new string[] {
				UTType.AliasFile,
				UTType.AliasRecord,
				UTType.AppleICNS,
				UTType.AppleProtectedMPEG4Audio,
				UTType.AppleProtectedMPEG4Video,
				UTType.AppleScript,
				UTType.Application,
				UTType.ApplicationBundle,
				UTType.ApplicationFile,
				UTType.Archive,
				UTType.AssemblyLanguageSource,
				UTType.Audio,
				UTType.AudioInterchangeFileFormat,
				UTType.AudiovisualContent,
				UTType.AVIMovie,
				UTType.BinaryPropertyList,
				UTType.BMP,
				UTType.Bookmark,
				UTType.Bundle,
				UTType.Bzip2Archive,
				UTType.CalendarEvent,
				UTType.CHeader,
				UTType.CommaSeparatedText,
				UTType.CompositeContent,
				UTType.ConformsToKey,
				UTType.Contact,
				UTType.Content,
				UTType.CPlusPlusHeader,
				UTType.CPlusPlusSource,
				UTType.CSource,
				UTType.Data,
				UTType.Database,
				UTType.DelimitedText,
				UTType.DescriptionKey,
				UTType.Directory,
				UTType.DiskImage,
				UTType.ElectronicPublication,
				UTType.EmailMessage,
				UTType.Executable,
				UTType.ExportedTypeDeclarationsKey,
				UTType.FileURL,
				UTType.FlatRTFD,
				UTType.Folder,
				UTType.Font,
				UTType.Framework,
				UTType.GIF,
				UTType.GNUZipArchive,
				UTType.HTML,
				UTType.ICO,
				UTType.IconFileKey,
				UTType.IdentifierKey,
				UTType.Image,
				UTType.ImportedTypeDeclarationsKey,
				UTType.InkText,
				UTType.InternetLocation,
				UTType.Item,
				UTType.JavaArchive,
				UTType.JavaClass,
				UTType.JavaScript,
				UTType.JavaSource,
				UTType.JPEG,
				UTType.JPEG2000,
				UTType.JSON,
				UTType.Log,
				UTType.M3UPlaylist,
				UTType.Message,
				UTType.MIDIAudio,
				UTType.MountPoint,
				UTType.Movie,
				UTType.MP3,
				UTType.MPEG,
				UTType.MPEG2TransportStream,
				UTType.MPEG2Video,
				UTType.MPEG4,
				UTType.MPEG4Audio,
				UTType.ObjectiveCPlusPlusSource,
				UTType.ObjectiveCSource,
				UTType.OSAScript,
				UTType.OSAScriptBundle,
				UTType.Package,
				UTType.PDF,
				UTType.PerlScript,
				UTType.PHPScript,
				UTType.PICT,
				UTType.PKCS12,
				UTType.PlainText,
				UTType.Playlist,
				UTType.PluginBundle,
				UTType.PNG,
				UTType.Presentation,
				UTType.PropertyList,
				UTType.PythonScript,
				UTType.QuickLookGenerator,
				UTType.QuickTimeImage,
				UTType.QuickTimeMovie,
				UTType.RawImage,
				UTType.ReferenceURLKey,
				UTType.Resolvable,
				UTType.RTF,
				UTType.RTFD,
				UTType.RubyScript,
				UTType.ScalableVectorGraphics,
				UTType.Script,
				UTType.ShellScript,
				UTType.SourceCode,
				UTType.SpotlightImporter,
				UTType.Spreadsheet,
				UTType.SymLink,
				UTType.SystemPreferencesPane,
				UTType.TabSeparatedText,
				UTType.TagClassFilenameExtension,
				UTType.TagClassMIMEType,
				UTType.TagSpecificationKey,
				UTType.Text,
				UTType.ThreeDContent,
				UTType.TIFF,
				UTType.ToDoItem,
				UTType.TXNTextAndMultimediaData,
				UTType.UnixExecutable,
				UTType.URL,
				UTType.URLBookmarkData,
				UTType.UTF16ExternalPlainText,
				UTType.UTF16PlainText,
				UTType.UTF8PlainText,
				UTType.UTF8TabSeparatedText,
				UTType.VCard,
				UTType.VersionKey,
				UTType.Video,
				UTType.Volume,
				UTType.WaveformAudio,
				UTType.WebArchive,
				UTType.WindowsExecutable,
				UTType.X509Certificate,
				UTType.XML,
				UTType.XMLPropertyList,
				UTType.XPCService,
				UTType.ZipArchive
			};
			#endregion

			var bundleFileUrl = NSBundle.MainBundle.GetUrlForResource ("TextDocument", "txt");
			documentURL = await ResourceHelper.CopyToDocumentsDirectoryAsync (bundleFileUrl);
		}

		#region Document picker's actions

		[Export("importFromDocPicker:")]
		public void ImportFromDocPicker(UIButton sender)
		{
			UIDocumentPickerViewController vc = new UIDocumentPickerViewController (allowedUTIs, UIDocumentPickerMode.Import);
			vc.WasCancelled += OnPickerCancel;
			vc.DidPickDocument += DidPickDocumentForImport;
			PresentViewController (vc, true, null);
		}

		void DidPickDocumentForImport (object sender, UIDocumentPickedEventArgs e)
		{
			// The url refers to a copy of the selected document.
			// This document is a temporary file.
			// It remains available only until your application terminates.
			// To keep a permanent copy, you must move this file to a permanent location inside your sandbox.
			NSUrl temporaryFileUrl = e.Url;
			PrintFileContent (temporaryFileUrl);
		}

		[Export("exportToDocPicker:")]
		public void ExportToDocPicker(UIButton sender)
		{
			if (TryShowFileNotExistsError ())
				return;

			UIDocumentPickerViewController vc = new UIDocumentPickerViewController (documentURL, UIDocumentPickerMode.ExportToService);
			vc.WasCancelled += OnPickerCancel;
			vc.DidPickDocument += DidPickDocumentForExport;

			PresentViewController (vc, true, null);
		}

		void DidPickDocumentForExport (object sender, UIDocumentPickedEventArgs e)
		{
			// The URL refers to the new copy of the exported document at the selected destination.
			// This URL refers to a file outside your app’s sandbox.
			// You cannot access this copy; the URL is passed only to indicate success.
			NSUrl url = e.Url;
			Console.WriteLine ("{0} exported to new location outside your app’s sandbox {1}", documentURL, url);
		}

		[Export("openDocPicker:")]
		public void OpenDocPicker(UIButton sender)
		{
			UIDocumentPickerViewController vc = new UIDocumentPickerViewController (allowedUTIs, UIDocumentPickerMode.Open);
			vc.WasCancelled += OnPickerCancel;
			vc.DidPickDocument += DidPickDocumentForOpen;
			PresentViewController (vc, true, null);
		}

		void DidPickDocumentForOpen (object sender, UIDocumentPickedEventArgs e)
		{
			// The url refers to the selected document.
			// The provided url is a security-scoped URL referring to a file outside your app’s sandbox.
			// For more information on working with external, security-scoped URLs, see Requirements in Document Picker Programming Guide
			// https://developer.apple.com/library/ios/documentation/FileManagement/Conceptual/DocumentPickerProgrammingGuide/AccessingDocuments/AccessingDocuments.html#//apple_ref/doc/uid/TP40014451-CH2-SW3
			var securityScopedUrl = e.Url;
			PrintOutsideFileContent (securityScopedUrl);
		}

		[Export("moveToDocPicker:")]
		private void MoveToDocPicker(UIButton sender)
		{
			if (TryShowFileNotExistsError ())
				return;

			UIDocumentPickerViewController vc = new UIDocumentPickerViewController (documentURL, UIDocumentPickerMode.MoveToService);
			vc.WasCancelled += OnPickerCancel;
			vc.DidPickDocument += DidPickDocumentForMove;
			PresentViewController (vc, true, null);
		}

		void DidPickDocumentForMove (object sender, UIDocumentPickedEventArgs e)
		{
			// The URL refers to the document’s new location.
			// The provided URL is a security-scoped URL referring to a file outside your app’s sandbox.
			// For more information on working with external, security-scoped URLs, see Requirements in Document Picker Programming Guide.
			// https://developer.apple.com/library/ios/documentation/FileManagement/Conceptual/DocumentPickerProgrammingGuide/AccessingDocuments/AccessingDocuments.html#//apple_ref/doc/uid/TP40014451-CH2-SW3
			NSUrl securityScopedUrl = e.Url;
			PrintOutsideFileContent (securityScopedUrl);
		}

		void PrintOutsideFileContent(NSUrl securityScopedUrl)
		{
			if (!securityScopedUrl.StartAccessingSecurityScopedResource ())
				return;

			PrintFileContent (securityScopedUrl);

			securityScopedUrl.StopAccessingSecurityScopedResource ();
		}

		void PrintFileContent(NSUrl url)
		{
			NSData data = null;
			NSError error = null;
			NSFileCoordinator fileCoordinator = new NSFileCoordinator ();
			fileCoordinator.CoordinateRead (url, (NSFileCoordinatorReadingOptions)0, out error, newUrl => {
				data = NSData.FromUrl(newUrl);
			});

			if (error != null) {
				Console.WriteLine ("CoordinateRead error {0}", error);
			} else {
				Console.WriteLine ("File name: {0}", url.LastPathComponent);
				Console.WriteLine (data);
			}
		}

		#endregion

		#region Documnet menu's actions

		[Export("importFromDocMenu:")]
		public void ImportFromDocMenu(UIButton sender)
		{
			UIDocumentMenuViewController vc = new UIDocumentMenuViewController (allowedUTIs, UIDocumentPickerMode.Import);
			SetupDelegateThenPresent (vc, sender);
		}

		[Export("openDocMenu:")]
		public void OpenDocMenu(UIButton sender)
		{
			UIDocumentMenuViewController vc = new UIDocumentMenuViewController (allowedUTIs, UIDocumentPickerMode.Open);
			SetupDelegateThenPresent (vc, sender);
		}

		[Export("exportToDocMenu:")]
		public void ExportToDocMenu(UIButton sender)
		{
			if (TryShowFileNotExistsError ())
				return;

			UIDocumentMenuViewController vc = new UIDocumentMenuViewController (documentURL, UIDocumentPickerMode.ExportToService);
			SetupDelegateThenPresent (vc, sender);
		}

		[Export("moveToDocMenu:")]
		public void MoveToDocMenu(UIButton sender)
		{
			if (TryShowFileNotExistsError ())
				return;

			UIDocumentMenuViewController vc = new UIDocumentMenuViewController (documentURL, UIDocumentPickerMode.MoveToService);
			SetupDelegateThenPresent (vc, sender);
		}

		void SetupDelegateThenPresent(UIDocumentMenuViewController vc, UIButton button)
		{
			vc.WasCancelled += OnPickerSelectionCancel;
			vc.DidPickDocumentPicker += OnPickerPicked;

			vc.AddOption ("Custom Option", null, UIDocumentMenuOrder.First, () => {
				Console.WriteLine ("completionHandler Hit");
			});

			vc.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			PresentViewController (vc, true, null);

			UIPopoverPresentationController presentationPopover = vc.PopoverPresentationController;
			if (presentationPopover != null) {
				presentationPopover.SourceView = View;
				presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Down;
				presentationPopover.SourceRect = button.Frame;
			}
		}

		#endregion

		#region Document menu's handlers

		void OnPickerSelectionCancel (object sender, EventArgs e)
		{
			var menu = (UIDocumentMenuViewController)sender;
			Unsibscribe (menu);

			Console.WriteLine ("Picker selection was canceled");
		}

		void OnPickerPicked (object sender, UIDocumentMenuDocumentPickedEventArgs e)
		{
			var menu = (UIDocumentMenuViewController)sender;
			Unsibscribe (menu);

			var documentPicker = e.DocumentPicker;
			documentPicker.WasCancelled += OnPickerCancel;
			switch (documentPicker.DocumentPickerMode) {
				case UIDocumentPickerMode.Import:
					documentPicker.DidPickDocument += DidPickDocumentForImport;
					break;

				case UIDocumentPickerMode.Open:
					documentPicker.DidPickDocument += DidPickDocumentForOpen;
					break;

				case UIDocumentPickerMode.ExportToService:
					documentPicker.DidPickDocument += DidPickDocumentForExport;
					break;

				case UIDocumentPickerMode.MoveToService:
					documentPicker.DidPickDocument += DidPickDocumentForMove;
					break;
			}

			PresentViewController (documentPicker, true, null);
		}

		#endregion

		void OnPickerCancel (object sender, EventArgs e)
		{
			Console.WriteLine ("Cancel pick document");
		}

		bool TryShowFileNotExistsError()
		{
			if (NSFileManager.DefaultManager.FileExists (documentURL.Path))
				return false;

			UIAlertController alert = UIAlertController.Create (documentURL.LastPathComponent, "File doesn't exist. Maybe you moved or Exported it earlier. Re run the app", UIAlertControllerStyle.Alert);
			alert.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));
			PresentViewController (alert, true, null);
			return true;
		}

		void Unsibscribe(UIDocumentMenuViewController menu)
		{
			menu.WasCancelled -= OnPickerSelectionCancel;
			menu.DidPickDocumentPicker -= OnPickerPicked;
		}
	}
}