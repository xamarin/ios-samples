using System;
using System.Collections.Generic;
using System.Linq;

using CoreFoundation;
using Foundation;
using UIKit;

namespace MusicMotion {
	public partial class SongViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource, INSCoding {
		
		readonly string textCellIdentifier = "SongCell";

		MotionManager motionManager;
		SongManager songManager;
		List<Song> cachedSongQueue;

		public SongViewController (IntPtr handle) : base (handle)
		{
			Initilize ();
		}

		[Export ("initWithCoder:")]
		public SongViewController (NSCoder coder) : base (coder)
		{
			Initilize ();
		}

		void Initilize ()
		{
			motionManager = new MotionManager ();
			songManager = new SongManager ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetNeedsStatusBarAppearanceUpdate ();

			motionManager.DidEncounterAuthorizationError += songManager.HandleEncounterAuthorizationError;
			motionManager.DrivingContextStarted += songManager.DrivingContextStarted;
			motionManager.HighIntensityContextStarted += songManager.HighIntensityContextStarted;
			motionManager.LowIntensityContextStarted += songManager.LowIntensityContextStarted;
			motionManager.MediumIntensityContextStarted += songManager.MediumIntensityContextStarted;

			DispatchQueue.MainQueue.DispatchAsync (motionManager.StartMonitoring);

			songManager.DidEncounterAuthorizationError += DidEncounterAuthorizationError;
			songManager.DidUpdateSongQueue += DidUpdateSongQueue;

			cachedSongQueue = songManager.SongQueue;

			UpdateAlbumViewWithSong (null);
			SelectFirstRow ();
		}

		public override void ViewDidUnload ()
		{
			motionManager.DidEncounterAuthorizationError -= songManager.HandleEncounterAuthorizationError;
			motionManager.DrivingContextStarted -= songManager.DrivingContextStarted;
			motionManager.HighIntensityContextStarted -= songManager.HighIntensityContextStarted;
			motionManager.LowIntensityContextStarted -= songManager.LowIntensityContextStarted;
			motionManager.MediumIntensityContextStarted -= songManager.MediumIntensityContextStarted;

			songManager.DidEncounterAuthorizationError -= DidEncounterAuthorizationError;
			songManager.DidUpdateSongQueue -= DidUpdateSongQueue;
		}

		void DidEncounterAuthorizationError (object sender, EventArgs e)
		{
			var alert = UIAlertController.Create (
				"Motion Activity Not Authorized",
				"To enable Motion features, please allow access to Motion & Fitness in Settings under Privacy.",
				UIAlertControllerStyle.Alert);

			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null);
			alert.AddAction (cancelAction);

			var openSettingsAction = UIAlertAction.Create ("Open Settings", UIAlertActionStyle.Default, _ => {
				var url = new NSUrl (UIApplication.OpenSettingsUrlString);
				UIApplication.SharedApplication.OpenUrl (url);
			});
			alert.AddAction (openSettingsAction);

			DispatchQueue.MainQueue.DispatchAsync (() =>
				PresentViewController (alert, true, null)
			);
		}

		void SelectFirstRow ()
		{
			var rowToSelect = NSIndexPath.FromItemSection (0, 0);
			SongTableView.SelectRow (rowToSelect, false, UITableViewScrollPosition.None);
		}

		void UpdateAlbumViewWithSong (Song song)
		{
			var currentSong = song ?? cachedSongQueue.FirstOrDefault ();

			if (currentSong == null)
				return;
			
			AlbumView.Image = currentSong.AlbumImage;
		}

		void DidUpdateSongQueue (object sender, EventArgs e)
		{
			var indexSet = new NSIndexSet (0);
			cachedSongQueue = ((SongManager)sender).SongQueue;
			DispatchQueue.MainQueue.DispatchAsync (() => {
				SongTableView.ReloadSections (indexSet, UITableViewRowAnimation.Fade);
				UpdateAlbumViewWithSong (null);
				SelectFirstRow ();
			});
		}

		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (textCellIdentifier, indexPath);
			var song = cachedSongQueue.ElementAt (indexPath.Row);

			cell.TextLabel.Text = song.Artist;
			cell.DetailTextLabel.Text = song.Title;
			return cell;
		}

		[Export ("tableView:titleForHeaderInSection:")]
		public string TitleForHeader (UITableView tableView, nint section)
		{
			return songManager.ContextDescription;
		}

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var song = cachedSongQueue [indexPath.Row];
			UpdateAlbumViewWithSong (song);
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public virtual nint RowsInSection (UITableView tableView, nint section)
		{
			return cachedSongQueue.Count;
		}
	}
}
