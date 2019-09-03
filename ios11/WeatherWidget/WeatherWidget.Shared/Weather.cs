using Foundation;
using System;
using UIKit;

namespace WeatherWidget
{
    public enum Weather
    {
        Sun = 0,
        Cloud = 1,
        Wind = 2,
        Rain = 3,
        Storm = 4,
    }

    public static class WeatherExtensions
    {
        public static string GetDescription(this Weather self)
        {
            switch (self)
            {
                case Weather.Sun: return "Sunny";
                case Weather.Cloud: return "Cloudy";
                case Weather.Wind: return "Windy";
                case Weather.Rain: return "Rainy";
                case Weather.Storm: return "Stormy";
                default: throw new NotImplementedException();
            }
        }

        public static string GetImageAssetName(this Weather self)
        {
            switch (self)
            {
                case Weather.Sun: return "Sun";
                case Weather.Cloud: return "Cloud";
                case Weather.Wind: return "Wind";
                case Weather.Rain: return "Rain";
                case Weather.Storm: return "Storm";
                default: throw new NotImplementedException();
            }
        }

        public static UIImage GetImageAsset(this Weather self)
        {
            var assetName = self.GetImageAssetName();
            var image = UIImage.FromBundle(assetName);
            if (image == null)
            {
                throw new Exception($"Expected an image named {assetName}");
            }

            return image;
        }
    }
}