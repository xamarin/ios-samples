using Foundation;
using System;
using UIKit;

namespace TableSearch
{
    /// <summary>
    /// The detail view controller navigated to from our main and results table.
    /// </summary>
    public partial class DetailViewController : UIViewController
    {
        // Constants for Storyboard/ViewControllers.
        private const string StoryboardName = "Main";
        private const string ViewControllerIdentifier = "DetailViewController";

        // Constants for state restoration.
        private const string RestoreProductKey = "restoreProduct";

        public DetailViewController(IntPtr handle) : base(handle) { }

        public Product Product { get; set; }

        public static UIViewController Create(Product product)
        {
            var storyboard = UIStoryboard.FromName(DetailViewController.StoryboardName, null);

            var viewController = storyboard.InstantiateViewController(DetailViewController.ViewControllerIdentifier);
            if (viewController is DetailViewController detailViewController)
            {
                detailViewController.Product = product;
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

            base.Title = Product.Title;
            this.yearsLabel.Text = this.Product.YearIntroduced.ToString();

            /* Here you can use 'NSNumberFormatter' as a Swift developer or string format as a C# developer */

            //var numberFormatter = new NSNumberFormatter();
            //numberFormatter.NumberStyle = NSNumberFormatterStyle.Currency;
            //numberFormatter.FormatterBehavior = NSNumberFormatterBehavior.Default;
            //var priceString = numberFormatter.StringFromNumber(new NSNumber(Product.IntroPrice));
            this.priceLabel.Text = this.Product.IntroPrice.ToString("C");
        }

        #region UIStateRestoration

        public override void EncodeRestorableState(NSCoder coder)
        {
            base.EncodeRestorableState(coder);

            // Encode the product.
            coder.Encode(Product, DetailViewController.RestoreProductKey);
        }

        public override void DecodeRestorableState(NSCoder coder)
        {
            base.DecodeRestorableState(coder);

            // Restore the product.
            if (coder.DecodeObject(DetailViewController.RestoreProductKey) is Product decodedProduct)
            {
                Product = decodedProduct;
            }
            else
            {
                throw new Exception("A product did not exist. In your app, handle this gracefully.");
            }
        }

        #endregion
    }
}