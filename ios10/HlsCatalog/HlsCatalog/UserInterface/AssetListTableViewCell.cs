using System;
using CoreFoundation;
using Foundation;
using UIKit;

namespace HlsCatalog
{
	public interface IAssetListTableViewCellDelegate
	{
		void DownloadStateDidChange (AssetListTableViewCell cell, DownloadState newState);
	}

	public partial class AssetListTableViewCell : UITableViewCell
	{
		public static readonly string CellReusableId = "AssetListTableViewCellIdentifier";

		[Outlet ("assetNameLabel")]
		UILabel AssetNameLabel { get; set; }

		[Outlet ("downloadStateLabel")]
		UILabel DownloadStateLabel { get; set; }

		[Outlet ("downloadProgressView")]
		UIProgressView DownloadProgressView { get; set; }

		public IAssetListTableViewCellDelegate Delegate { get; set; }

		Asset asset;
		public Asset Asset {
			get {
				return asset;
			}
			set {
				if ((asset = value) != null) {
					var downloadState = AssetPersistenceManager.SharedManager.GetDownloadState (asset);
					switch (downloadState) {
					case DownloadState.Downloaded:
						DownloadProgressView.Hidden = true;
						break;

					case DownloadState.Downloading:
						DownloadProgressView.Hidden = false;
						break;
					}

					AssetNameLabel.Text = asset.Name;
					DownloadStateLabel.Text = downloadState.ToString ();

					AssetPersistenceManager.DownloadStateChanged += OnDownloadStateChanged;
					AssetPersistenceManager.DownloadingProgressChanged += OnDownloadingProgressChanged;
				} else {
					DownloadProgressView.Hidden = false;
					AssetNameLabel.Text = string.Empty;
					DownloadStateLabel.Text = string.Empty;
				}
			}
		}

		public AssetListTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		void OnDownloadStateChanged (object sender, DownloadStateEventArgs e)
		{
			if (Asset == null || Asset.Name != e.AssetName)
				return;

			DispatchQueue.MainQueue.DispatchAsync (() => {
				switch (e.State) {
				case DownloadState.Downloading:
					DownloadProgressView.Hidden = false;

					if (!string.IsNullOrWhiteSpace (e.DisplayName)) {
						DownloadStateLabel.Text = $"{e.State}: {e.DisplayName}";
						return;
					}
					break;

				case DownloadState.Downloaded:
				case DownloadState.NotDownloaded:
					DownloadProgressView.Hidden = true;
					break;
				}

				Delegate?.DownloadStateDidChange (this, e.State);
			});
		}

		void OnDownloadingProgressChanged (object sender, HlsCatalog.DownloadProgressEventArgs e)
		{
			if (Asset == null || Asset.Name != e.AssetName)
				return;
			DownloadProgressView.SetProgress ((float)e.PercentDownloaded, true);
		}
	}
}
