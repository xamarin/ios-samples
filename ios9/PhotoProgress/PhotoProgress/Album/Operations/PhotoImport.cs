using System;

using Foundation;
using UIKit;

namespace PhotoProgress {
	public class PhotoImport : NSObject, INSProgressReporting {

		PhotoDownload download;
		Action<UIImage, NSError> completionHandler;

		PhotoProgress progress;
		public PhotoProgress Progress {
			get {
				return progress;
			}
		}

		public PhotoImport (NSUrl url)
		{
			progress = new PhotoProgress {
				TotalUnitCount = 10
			};

			download = new PhotoDownload (url);
		}

		public void Start (Action<UIImage, NSError> completionHandler)
		{
			this.completionHandler = completionHandler;
			progress.AddChild (download.Progress, 9);

			download.Start ((data, error) => {

				if (data == null) {
					CallCompletionHandler (null, error);
					return;
				}

				var image = UIImage.LoadFromData (data);

				progress.BecomeCurrent (1);
				var filteredImage = PhotoFilter.FilteredImage (image);
				progress.ResignCurrent ();
				CallCompletionHandler (filteredImage, null);
			});
		}

		void CallCompletionHandler (UIImage image, NSError error)
		{
			completionHandler?.Invoke (image, error);
			completionHandler = null;
		}
	}
}

