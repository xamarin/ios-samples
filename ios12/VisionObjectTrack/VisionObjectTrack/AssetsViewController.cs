
namespace VisionObjectTrack
{
    using AVFoundation;
    using CoreFoundation;
    using CoreGraphics;
    using Foundation;
    using Photos;
    using System;
    using UIKit;

    public partial class AssetsViewController : UICollectionViewController
    {
        private const string ShowTrackingViewSegueIdentifier = "ShowTrackingView";

        private PHFetchResult assets;

        public AssetsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PHPhotoLibrary.RequestAuthorization((status) =>
            {
                if (status == PHAuthorizationStatus.Authorized)
                {
                    DispatchQueue.MainQueue.DispatchAsync(() => this.LoadAssetsFromLibrary());
                }
            });
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            this.RecalculateItemSize();
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        private void LoadAssetsFromLibrary()
        {
            var assetsOptions = new PHFetchOptions();
            // include all source types
            assetsOptions.IncludeAssetSourceTypes = PHAssetSourceType.CloudShared | PHAssetSourceType.UserLibrary | PHAssetSourceType.iTunesSynced;
            // show most recent first
            assetsOptions.SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("modificationDate", false) };

            // fecth videos
            this.assets = PHAsset.FetchAssets(PHAssetMediaType.Video, assetsOptions);

            // setup collection view
            this.RecalculateItemSize();
            this.CollectionView?.ReloadData();
        }

        private void RecalculateItemSize()
        {
            if (this.CollectionView != null)
            {
                if (this.CollectionView.CollectionViewLayout is UICollectionViewFlowLayout layout)
                {
                    var desiredItemCount = this.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact ? 4 : 6;
                    var availableSize = this.CollectionView.Bounds.Width;

                    var insets = layout.SectionInset;
                    availableSize -= (insets.Left + insets.Right);
                    availableSize -= layout.MinimumInteritemSpacing * (desiredItemCount - 1f);
                   
                    var itemSize = Math.Floor(availableSize / desiredItemCount);
                    if (layout.ItemSize.Width != itemSize)
                    {
                        layout.ItemSize = new CGSize(itemSize, itemSize);
                        layout.InvalidateLayout();
                    }
                }
            }
        }

        private PHAsset Asset(string identifier)
        {
            PHAsset foundAsset = null;
            this.assets.Enumerate((NSObject element, nuint index, out bool stop) =>
            {
                if (element is PHAsset asset && asset?.LocalIdentifier == identifier)
                {
                    foundAsset = asset;
                    stop = true;
                }
                else
                {
                    stop = false;
                }
            });

            return foundAsset;
        }

        #region Navigation

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == ShowTrackingViewSegueIdentifier)
            {
                if (sender is AVAsset avAsset)
                {
                    if (segue.DestinationViewController is TrackingViewController trackingController)
                    {
                        trackingController.VideoAsset = avAsset;
                    }
                    else
                    {
                        throw new Exception("Unexpected destination view controller type");
                    }
                }
                else
                {
                    throw new Exception("Unexpected sender type");
                }
            }
        }

        #endregion

        #region UICollectionViewDataSource

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.assets?.Count ?? 0;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell result = null;
            if (this.assets?[indexPath.Item] is PHAsset asset)
            {
                var genericCell = collectionView.DequeueReusableCell(AssetsCell.Identifier, indexPath);
                if (genericCell is AssetsCell cell)
                {
                    cell.RepresentedAssetIdentifier = asset.LocalIdentifier;
                    var imageManager = new PHImageManager();
                    var options = new PHImageRequestOptions
                    {
                        NetworkAccessAllowed = true,
                        ResizeMode = PHImageRequestOptionsResizeMode.Fast,
                        DeliveryMode = PHImageRequestOptionsDeliveryMode.Opportunistic,
                    };

                    imageManager.RequestImageForAsset(asset, cell.Bounds.Size, PHImageContentMode.AspectFill, options, (image, _) =>
                    {
                        if (asset.LocalIdentifier == cell.RepresentedAssetIdentifier)
                        {
                            cell.ImageView.Image = image;
                        }
                    });

                    result = cell;
                }
                else
                {
                    result = genericCell as UICollectionViewCell;
                }
            }
            else
            {
                throw new Exception($"Failed to find asset at index {indexPath.Item}");
            }

            return result;
        }

        #endregion

        #region UICollectionViewDelegate

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (collectionView.CellForItem(indexPath) is AssetsCell cell)
            {
                var assetId = cell.RepresentedAssetIdentifier;
                var asset = this.Asset(assetId);
                if (asset == null)
                {
                    throw new Exception($"Failed to find asset with identifier {assetId}");
                }

                var imageManager = PHImageManager.DefaultManager;
                var videoOptions = new PHVideoRequestOptions
                {
                    NetworkAccessAllowed = true,
                    DeliveryMode = PHVideoRequestOptionsDeliveryMode.HighQualityFormat
                };

                imageManager.RequestAvAsset(asset, videoOptions, (avAsset, _, __) =>
                {
                    if (avAsset != null)
                    {
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            this.PerformSegue(ShowTrackingViewSegueIdentifier, avAsset);
                        });
                    }
                });
            }
            else
            {
                throw new Exception($"Failed to find cell as index path {indexPath}");
            }
        }

        #endregion
    }
}