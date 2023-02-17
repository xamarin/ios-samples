using System;

using Foundation;
using UIKit;
using MusicKitSample.Models;

namespace MusicKitSample {
	public partial class MediaItemTableViewCell : UITableViewCell {
		#region Cell Identifier

		public static readonly NSString Key = new NSString ("MediaItemTableViewCell");

		#endregion

		#region Properties

		public UIImage AssetCoverArt {
			get { return ImgAssetCoverArt.Image; }
			set { ImgAssetCoverArt.Image = value; }
		}

		public string Title {
			get { return LblTitle.Text; }
			set { LblTitle.Text = value; }
		}

		public string Artist {
			get { return LblArtist.Text; }
			set { LblArtist.Text = value; }
		}

		public bool AddToPlaylistButtonEnabled {
			get { return BtnAddToPlaylist.Enabled; }
			set { BtnAddToPlaylist.Enabled = value; }
		}

		public bool PlayItemButtonEnabled {
			get { return BtnPlayItem.Enabled; }
			set { BtnPlayItem.Enabled = value; }
		}

		MediaItem mediaItem;
		public MediaItem MediaItem {
			get { return mediaItem; }
			set {
				mediaItem = value;
				Title = value?.Name ?? string.Empty;
				Artist = value?.ArtistName ?? string.Empty;
				AssetCoverArt = null;
			}
		}

		public IMediaSearchTableViewCellDelegate Delegate { get; set; }

		#endregion

		#region Constructors

		protected MediaItemTableViewCell (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		#endregion

		#region User Interactions

		partial void BtnAddToPlaylist_TouchUpInside (UIButton sender)
		{
			if (mediaItem != null)
				Delegate?.AddToPlaylist (this, mediaItem);
		}

		partial void BtnPlayItem_TouchUpInside (UIButton sender)
		{
			if (mediaItem != null)
				Delegate?.PlayMediaItem (this, mediaItem);
		}

		#endregion
	}

	public interface IMediaSearchTableViewCellDelegate {
		void AddToPlaylist (MediaItemTableViewCell mediaSearchTableViewCell, MediaItem mediaItem);
		void PlayMediaItem (MediaItemTableViewCell mediaSearchTableViewCell, MediaItem mediaItem);
	}
}
