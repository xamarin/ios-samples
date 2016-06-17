using System;

using Foundation;
using UIKit;

namespace PhotoProgress {
	public partial class PhotoCollectionViewCell : UICollectionViewCell, INSCoding {

		bool disposed = false;

		Photo photo;
		public Photo Photo {
			get {
				return photo;
			}
			set {	
				Unsubscribe ();
				photo = value;
				Subscribe ();
				UpdateProgressView (null, null);
				UpdateImageView (null, null);
			}
		}

		[Export ("initWithCoder:")]
		public PhotoCollectionViewCell (NSCoder coder) : base (coder)
		{
		}

		void Unsubscribe ()
		{
			if (photo == null)
				return;

			photo.FractionComepletedChanged -= UpdateProgressView;
			photo.ImageChanged -= UpdateImageView;
		}

		void Subscribe ()
		{
			if (photo == null)
				return;

			photo.FractionComepletedChanged += UpdateProgressView;
			photo.ImageChanged += UpdateImageView;
		}

		void UpdateProgressView (object sender, EventArgs e)
		{
			NSOperationQueue.MainQueue.AddOperation (() => {
				var photoImport = Photo?.PhotoImport;
				if (photoImport != null) {
					ProgressView.Progress = (float)photoImport.Progress.FractionCompleted;
					ProgressView.Hidden = false;
				} else {
					ProgressView.Hidden = true;
				}
			});
		}

		void UpdateImageView (object sender, EventArgs e)
		{
			NSOperationQueue.MainQueue.AddOperation (() => UIView.Transition (ImageView, 0.5, UIViewAnimationOptions.TransitionCrossDissolve, () => {
				ImageView.Image = Photo.Image;
			}, null));
		}

		protected override void Dispose (bool disposing)
		{
			if (disposed)
				return;

			if (disposing) {
				if (ImageView != null && ImageView.Image != null)
					ImageView.Image.Dispose ();

				if (Photo != null && Photo.Image != null)
					Photo.Image.Dispose ();
			}

			disposed = true;
			base.Dispose(disposing);
		}
	}
}
