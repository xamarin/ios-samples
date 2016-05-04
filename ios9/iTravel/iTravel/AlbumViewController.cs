using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using Newtonsoft.Json;

namespace iTravel {
	public partial class AlbumViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate {
		const string textCellIdentifier = "DataItem";

		readonly IntPtr progressObserverContext = IntPtr.Zero;
		NSBundleResourceRequest request;
		List<PreviewDetail> previewDetails = new List<PreviewDetail> ();

		protected AlbumViewController(IntPtr handle) : base (handle)
		{
		}

		public async void LoadAlbum (string whichAlbum)
		{
			request = new NSBundleResourceRequest (new [] { whichAlbum });

			var options = NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial;
			request.Progress.AddObserver (this, "fractionCompleted", options, progressObserverContext);

			if (ProgressView != null)
				ProgressView.Hidden = false;

			try {
				await request.BeginAccessingResourcesAsync ();
			} catch (NSErrorException ex) {
				Console.WriteLine ("Error occurred: {0}", ex.Error.LocalizedDescription);
			}

			NSOperationQueue.MainQueue.AddOperation (() => {
				ProgressView.Hidden = true;
				PopulateCollectionView (whichAlbum.ToLower ());

				ProgressView.Progress = 1f;
				DetailsLabel.Text = "Loaded";
			});

		}

		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (context == progressObserverContext && keyPath == "fractionCompleted") {
				NSOperationQueue.MainQueue.AddOperation (() => {
					var progress = (NSProgress)ofObject;

					ProgressView.Progress = (float)progress.FractionCompleted;
					DetailsLabel.Text = progress.LocalizedDescription;
				});
			} else {
				base.ObserveValue (keyPath, ofObject, change, context);
			}
		}

		public override void ViewDidUnload ()
		{
			request.Progress.RemoveObserver (this, "fractionCompleted");
		}

		void PopulateCollectionView (string whichAlbum)
		{
			if (string.IsNullOrEmpty (whichAlbum))
				return;

			var tableData = new NSDataAsset (whichAlbum);
			try {
				var jsonString = NSString.FromData (tableData.Data, NSStringEncoding.UTF8);
				previewDetails = JsonConvert.DeserializeObject<List<PreviewDetail>> (jsonString);
				CustomTableView.ReloadData ();
			} catch (Exception e) {
				Console.WriteLine ("Error occurred: {0}", e.Message);
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier != "showPicture")
				return;

			var albumViewController = segue.DestinationViewController as ImageViewController;
			if (albumViewController == null)
				return;

			var selectedRowIndex = CustomTableView.IndexPathForSelectedRow.Row;
			albumViewController.PictureName = previewDetails[selectedRowIndex].Picture;
			albumViewController.Title = previewDetails[selectedRowIndex].Caption;
		}

		[Export ("numberOfSectionsInTableView:")]
		public nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public nint RowsInSection (UITableView tableView, nint section)
		{
			return previewDetails.Count;
		}

		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (textCellIdentifier, indexPath);
			var item = previewDetails [indexPath.Row];
			cell.TextLabel.Text = item.Caption;

			return cell;
		}
	}
}

