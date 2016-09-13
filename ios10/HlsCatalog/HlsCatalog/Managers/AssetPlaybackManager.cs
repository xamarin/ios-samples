using System;

using AVFoundation;
using Foundation;

using static Foundation.NSKeyValueObservingOptions;

namespace HlsCatalog
{
	// IAssetPlaybackDelegate provides a common interface for AssetPlaybackManager to provide callbacks to its delegate.
	public interface IAssetPlaybackDelegate
	{
		// This is called when the internal AVPlayer in AssetPlaybackManager is ready to start playback.
		void PlayerReadyToPlay (AssetPlaybackManager streamPlaybackManager, AVPlayer player);

		// This is called when the internal AVPlayer's currentItem has changed.
		void PlayerCurrentItemChanged (AssetPlaybackManager streamPlaybackManager, AVPlayer player);
	}

	public class AssetPlaybackManager : IDisposable
	{
		// Singleton for AssetPlaybackManager.
		public static AssetPlaybackManager SharedManager { get; } = new AssetPlaybackManager ();

		public IAssetPlaybackDelegate Delegate { get; set; }

		// The instance of AVPlayer that will be used for playback of AssetPlaybackManager.playerItem.
		readonly AVPlayer player = new AVPlayer ();

		// A Bool tracking if the AVPlayerItem.Status has changed to .ReadyToPlay for the current AssetPlaybackManager.PlayerItem.
		bool readyForPlayback;

		IDisposable statusToken;
		IDisposable playableToken;
		IDisposable currentItemToken;

		// The AVPlayerItem associated with AssetPlaybackManager.asset.urlAsset
		AVPlayerItem playerItem;
		AVPlayerItem PlayerItem {
			get {
				return playerItem;
			}
			set {
				statusToken?.Dispose ();
				playerItem = value;
				statusToken = playerItem?.AddObserver ("status", Initial | New, StatusChanged);
			}
		}

		// The Asset that is currently being loaded for playback.
		Asset asset;
		public Asset Asset {
			get {
				return asset;
			}
			set {
				playableToken?.Dispose ();
				if ((asset = value) != null) {
					playableToken = asset.UrlAsset.AddObserver ("isPlayable", Initial | New, IsPlayableChanged);
				} else {
					PlayerItem = null;
					player.ReplaceCurrentItemWithPlayerItem (null);
					readyForPlayback = false;
				}
			}
		}

		public AssetPlaybackManager ()
		{
			currentItemToken = player.AddObserver ("currentItem", New, CurrentItemChanged);
		}

		// TODO: why we need this? prop should be sufficient 
		// Replaces the currently playing Asset, if any, with a new Asset. If null is passed,
		// AssetPlaybackManager will handle unloading the existing Asset and handle KVO cleanup.
		void SetAssetForPlayback (Asset asset)
		{
			Asset = asset;
		}

		void StatusChanged (NSObservedChange obj)
		{
			if (PlayerItem == null || PlayerItem.Status != AVPlayerItemStatus.ReadyToPlay)
				return;

			if (!readyForPlayback) {
				readyForPlayback = true;
				Delegate?.PlayerReadyToPlay (this, player);
			}
		}

		void IsPlayableChanged (NSObservedChange obj)
		{
			var assetItem = Asset;
			if (assetItem == null || !assetItem.UrlAsset.Playable)
				return;

			PlayerItem = new AVPlayerItem (assetItem.UrlAsset);
			player.ReplaceCurrentItemWithPlayerItem (PlayerItem);
		}

		void CurrentItemChanged (NSObservedChange obj)
		{
			Delegate?.PlayerCurrentItemChanged (this, player);
		}

		public void Dispose ()
		{
			currentItemToken?.Dispose ();
		}
	}
}
