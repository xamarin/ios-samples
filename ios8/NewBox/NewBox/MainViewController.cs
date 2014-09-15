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

			documentURL = await ResourceHelper.CopyResourceToCloud ();
		}

		#region Document picker's actions

		[Export("importFromDocPicker:")]
		public void ImportFromDocPicker(UIButton sender)
		{
			UIDocumentPickerViewController vc = new UIDocumentPickerViewController (allowedUTIs, UIDocumentPickerMode.Import);
			SetupDelegateThenPresent (vc);
		}

		[Export("openDocPicker:")]
		public void OpenDocPicker(UIButton sender)
		{
			UIDocumentPickerViewController vc = new UIDocumentPickerViewController (allowedUTIs, UIDocumentPickerMode.Open);
			SetupDelegateThenPresent (vc);
		}

		[Export("exportToDocPicker:")]
		public void ExportToDocPicker(UIButton sender)
		{
			UIDocumentPickerViewController vc = new UIDocumentPickerViewController (documentURL, UIDocumentPickerMode.ExportToService);
			SetupDelegateThenPresent (vc);
		}

		[Export("moveToDocPicker:")]
		private void MoveToDocPicker(UIButton sender)
		{
			UIDocumentPickerViewController vc = new UIDocumentPickerViewController (documentURL, UIDocumentPickerMode.MoveToService);
			SetupDelegateThenPresent (vc);
		}

		private void SetupDelegateThenPresent(UIDocumentPickerViewController docPickerViewController)
		{
			docPickerViewController.WasCancelled += OnPickerCancel;
			docPickerViewController.DidPickDocument += OnDocumentPicked;
			PresentViewController (docPickerViewController, true, null);
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
			UIDocumentMenuViewController vc = new UIDocumentMenuViewController (documentURL, UIDocumentPickerMode.ExportToService);
			SetupDelegateThenPresent (vc, sender);
		}

		[Export("moveToDocMenu:")]
		public void MoveToDocMenu(UIButton sender)
		{
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

			e.DocumentPicker.WasCancelled += OnPickerCancel;
			PresentViewController (e.DocumentPicker, true, null);
		}

		#endregion

		#region Document picker's handlers

		void OnPickerCancel (object sender, EventArgs e)
		{
			Console.WriteLine ("Cancel pick document");
		}

		void OnDocumentPicked (object sender, UIDocumentPickedEventArgs e)
		{
			Console.WriteLine ("OnDocumentPicked called");
			bool startAccessingWorked = e.Url.StartAccessingSecurityScopedResource ();

			NSUrl ubiquityURL = NSFileManager.DefaultManager.GetUrlForUbiquityContainer (null);
			Console.WriteLine ("ubiquityURL {0}", ubiquityURL);
			Console.WriteLine ("start {0}", startAccessingWorked);

			// TODO: This should work but doesn't
//			NSFileCoordinator fileCoordinator = new NSFileCoordinator ();
//			NSError error = null;
//			Console.WriteLine ("MainViewController before CoordinateRead {0}", e.Url);
//			fileCoordinator.CoordinateRead (e.Url, (NSFileCoordinatorReadingOptions)0, out error, newUrl => {
//				Console.WriteLine ("MainViewController inside CoordinateRead");
//				NSData data = NSData.FromUrl(newUrl);
//				Console.WriteLine ("error {0}", error);
//				Console.WriteLine ("data {0}", data);
//			});
//			Console.WriteLine ("MainViewController after CoordinateRead error {0}", error);

			e.Url.StopAccessingSecurityScopedResource ();
		}

		#endregion

		void Unsibscribe(UIDocumentMenuViewController menu)
		{
			menu.WasCancelled -= OnPickerSelectionCancel;
			menu.DidPickDocumentPicker -= OnPickerPicked;
		}

		void Unsibscribe(UIDocumentPickerViewController picker)
		{
			picker.WasCancelled -= OnPickerCancel;
			picker.DidPickDocument -= OnDocumentPicked;
		}
	}
}

