using System;

using Foundation;
using UIKit;

namespace PhotoProgress {
	public class Photo : NSObject {

		NSUrl imageURL;

		public event EventHandler FractionComepletedChanged;
		public event EventHandler ImageChanged;

		public PhotoImport PhotoImport { get; private set; }

		UIImage image;
		public UIImage Image {
			get {
				return image;
			}
			private set {
				image = value;
				ImageChanged?.Invoke (null, null);
			}
		}

		public Photo (NSUrl url)
		{
			imageURL = url;
			Image = UIImage.FromBundle ("PhotoPlaceholder");
		}

		public PhotoProgress StartImport ()
		{
			PhotoImport = new PhotoImport (imageURL);

			PhotoImport.Progress.CompletedUnitCountChanged += (obj, args) => FractionComepletedChanged?.Invoke (obj, args);
			PhotoImport.Start ((img, error) => {
				if (img != null)
					Image = img;
				else
					ReportError (error);

				PhotoImport.Progress.CompletedUnitCountChanged -= (obj, args) => FractionComepletedChanged?.Invoke (obj, args);
				PhotoImport = null;
				FractionComepletedChanged?.Invoke (null, null);
			});

			return PhotoImport.Progress;
		}

		public void Reset ()
		{
			Image = UIImage.FromBundle ("PhotoPlaceholder");
			PhotoImport = null;
		}

		void ReportError (NSError error)
		{
			if (error.Domain == NSError.CocoaErrorDomain || error.Code == (nint)(int)NSCocoaError.UserCancelled)
				Console.WriteLine ("Error importing photo: {0}", error.LocalizedDescription);
		}
	}
}

