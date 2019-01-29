using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace WeatherWidget
{
    /// <summary>
    /// The initial view controller for the app to display weather forecast data in a collection view.
    /// </summary>
    public partial class ForecastViewController : UICollectionViewController, IUICollectionViewDelegateFlowLayout
    {
        private List<WeatherForecast> weatherForecastData = WeatherForecast.LoadSharedData();

        public ForecastViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Register the collection view cells.
            var weatherForecastCellNib = UINib.FromName("ForecastCollectionViewCell", null);
            base.CollectionView.RegisterNibForCell(weatherForecastCellNib, ForecastCollectionViewCell.ReuseIdentifier);
        }

        #region Collection view data source

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.weatherForecastData.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(ForecastCollectionViewCell.ReuseIdentifier, indexPath) as ForecastCollectionViewCell;

            var weatherForecast = this.weatherForecastData[(int)indexPath.Item];
            cell.Label = weatherForecast.DaysFromNowDescription;
            cell.Image = weatherForecast.Forecast.GetImageAsset();
            cell.ForecastLabel = weatherForecast.Forecast.GetDescription();

            return cell;
        }

        #endregion

        partial void RefreshForecast(NSObject sender)
        {
            // Generate new forecast data, save it to disk and reload the collection view.
            this.weatherForecastData = WeatherForecast.GenerateRandomForecastData();
            WeatherForecast.SaveSharedData(this.weatherForecastData);
            base.CollectionView.ReloadData();
        }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CoreGraphics.CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            // The full size of the collection view minus any top or bottom bars.
            return collectionView.AdjustedContentInset.InsetRect(collectionView.Bounds).Size;
        }

        public void ScrollToForecast(int index)
        {
            var indexPath = NSIndexPath.FromItemSection(index, 0);
            base.CollectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredHorizontally, false);
        }
    }
}