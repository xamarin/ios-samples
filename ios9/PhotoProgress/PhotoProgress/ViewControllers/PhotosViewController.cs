using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace PhotoProgress {
	public partial class PhotosViewController : UIViewController {

		Album album;
		bool progressViewIsHidden = true;

		PhotoProgress overallProgress;
		PhotoProgress OverallProgress {
			get {
				return overallProgress;
			}
			set {
				Unsubscribe ();
				overallProgress = value;
				Subscribe ();
				Update (null, null);
			}
		}

		bool OverallProgressIsFinished {
			get {
				var completed = OverallProgress.CompletedUnitCount;
				var total = OverallProgress.TotalUnitCount;
				return completed >= total && total > 0;
			}
		}

		[Export ("initWithCoder:")]
		public PhotosViewController (NSCoder coder) : base (coder)
		{
			album = new Album ();
		}

		public void Update (object sender, EventArgs e)
		{
			NSOperationQueue.MainQueue.AddOperation (() => {
				UpdateProgressView ();
				UpdateToolbar ();
			});
		}

		partial void StartImport (NSObject sender)
		{
			OverallProgress = album.ImportPhotos ();
		}

		partial void CanceltImport (NSObject sender)
		{
			OverallProgress.Cancel();
		}

		partial void PauseImport (NSObject sender)
		{
			OverallProgress.Pause ();
		}

		partial void ResetImport (NSObject sender)
		{
			album.ResetPhotos ();
			OverallProgress = null;
		}

		partial void ResumeImport (NSObject sender)
		{
			OverallProgress.Resume ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier != "photos collection")
				return;

			var collectionViewController = (PhotosCollectionViewController)segue.DestinationViewController;
			collectionViewController.Album = album;
		}

		void Unsubscribe ()
		{
			if (OverallProgress == null)
				return;

			OverallProgress.FractionCompletedChanged -= Update;
			OverallProgress.CompletedUnitCountChanged -= Update;
			OverallProgress.TotalUnitCountChanged -= Update;
			OverallProgress.PhotoProgressCanceled -= Update;
			OverallProgress.PhotoProgressPaused -= Update;
		}

		void Subscribe ()
		{
			if (OverallProgress == null)
				return;

			OverallProgress.FractionCompletedChanged += Update;
			OverallProgress.CompletedUnitCountChanged += Update;
			OverallProgress.TotalUnitCountChanged += Update;
			OverallProgress.PhotoProgressCanceled += Update;
			OverallProgress.PhotoProgressPaused += Update;
		}

		void UpdateProgressView ()
		{
			bool shouldHide;

			if (OverallProgress != null) {
				shouldHide = OverallProgressIsFinished || OverallProgress.Cancelled;
				ProgressView.Progress = (float)OverallProgress.FractionCompleted;
			} else {
				shouldHide = true;
			}

			if (progressViewIsHidden != shouldHide) {
				UIView.Animate (0.2, () => {
					ProgressContainerView.Alpha = shouldHide ? 0 : 1;
				});

				progressViewIsHidden = shouldHide;
			}
		}

		void UpdateToolbar ()
		{
			var items = new List<UIBarButtonItem> ();
			if (OverallProgress != null) {
				if (OverallProgressIsFinished || OverallProgress.Cancelled) {
					items.Add (ResetToolbarItem);
				} else {
					items.Add (CancelToolbarItem);
					items.Add (OverallProgress.Paused ? ResumeToolbarItem : PauseToolbarItem);
				}
			} else {
				items.Add (StartToolbarItem);
			}

			NavigationController.Toolbar.SetItems (items.ToArray (), true);
		}
	}
}
