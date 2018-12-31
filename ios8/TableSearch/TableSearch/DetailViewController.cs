using System;
using Foundation;
using UIKit;

namespace TableSearch
{
    /// <summary>
    /// The detail view controller navigated to from our main and results table.
    /// </summary>
    public partial class DetailViewController : UIViewController
    {
        // Constants for Storyboard/ViewControllers.
        private const string storyboardName = "Main";
        private const string viewControllerIdentifier = "DetailViewController";

        // Constants for state restoration.
        private const string restoreProduct = "restoreProductKey";


        public DetailViewController(IntPtr handle) : base(handle) { }

        public Product product;

        public static UIViewController Create(Product product)
        {
            var storyboard = UIStoryboard.FromName(DetailViewController.storyboardName, null);

            var viewController = storyboard.InstantiateViewController(DetailViewController.viewControllerIdentifier);
            if (viewController is DetailViewController detailViewController)
            {
                detailViewController.product = product;
            }

            return viewController;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                base.NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Never;
            }

            base.Title = product.Title;

            yearsLabel.Text = product.YearIntroduced.ToString();

            var numberFormatter = new NSNumberFormatter();
            numberFormatter.NumberStyle = NSNumberFormatterStyle.Currency;
            numberFormatter.FormatterBehavior = NSNumberFormatterBehavior.Default;
            var priceString = numberFormatter.StringFromNumber(new NSNumber(product.IntroPrice));
            priceLabel.Text = priceString;
            // string.Format ("{0:C}", Product.IntroPrice);
        }

        #region UIStateRestoration

        public override void EncodeRestorableState(NSCoder coder)
        {
            base.EncodeRestorableState(coder);
            // Encode the product.
            coder.Encode(product, DetailViewController.restoreProduct);
        }

        public override void DecodeRestorableState(NSCoder coder)
        {

            // Restore the product.
            if (coder.DecodeObject(DetailViewController.restoreProduct) is Product decodedProduct)
            {
                product = decodedProduct;
            }
            else
            {
                throw new Exception("A product did not exist. In your app, handle this gracefully.");
            }
            base.DecodeRestorableState(coder);
        }

        #endregion
    }
}