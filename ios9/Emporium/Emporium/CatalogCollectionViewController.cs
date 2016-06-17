using System;

using UIKit;
using Foundation;

namespace Emporium
{
	[Register ("CatalogCollectionViewController")]
	public class CatalogCollectionViewController : UICollectionViewController
	{
		static readonly NSString reuseIdentifier = (NSString)"ProductCell";
		static readonly NSString segueIdentifier = (NSString)"ProductDetailSegue";

		Product[] products;

		Product[] Products {
			get {
				if (products == null) {
					// Populate the products array from a plist.
					NSUrl productsURL = NSBundle.MainBundle.GetUrlForResource ("ProductsList", "plist");
					var productArr = NSArray.FromFile (productsURL.Path);

					products = new Product[(int)productArr.Count];
					for (nuint i = 0; i < productArr.Count; i++) {
						var container = new ProductContainer (productArr.GetItem<NSDictionary> (i));
						products [(int)i] = container.Product;
					}
				}

				return products;
			}
		}

		public CatalogCollectionViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == segueIdentifier) {
				var indexPaths = CollectionView.GetIndexPathsForSelectedItems ();
				if (indexPaths == null || indexPaths.Length == 0)
					return;

				var viewController = (ProductTableViewController)segue.DestinationViewController;
				viewController.Product = products [indexPaths [0].Row];
			} else {
				base.PrepareForSegue (segue, sender);
			}
		}

		#region IUICollectionViewDataSource & IUICollectionViewDelegate

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return Products.Length;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (ProductCell)collectionView.DequeueReusableCell (reuseIdentifier, indexPath);
			ConfigureCell (cell, Products [indexPath.Row]);

			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			PerformSegue (segueIdentifier, this);
		}

		#endregion

		static void ConfigureCell (ProductCell cell, Product product)
		{
			cell.TitleLabel.Text = product.Name;
			cell.PriceLabel.Text = product.Price;
			cell.SubtitleLabel.Text = product.Description;
		}
	}
}