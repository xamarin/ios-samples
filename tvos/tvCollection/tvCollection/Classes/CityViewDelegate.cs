using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using CoreGraphics;

namespace tvCollection
{
	public class CityViewDelegate : UICollectionViewDelegateFlowLayout
	{
		#region Application Access
		public static AppDelegate App {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Constructors
		public CityViewDelegate ()
		{
		}
		#endregion

		#region Override Methods
		public override CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			return new CGSize (361, 256);
		}

		public override bool CanFocusItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			if (indexPath == null) {
				return false;
			} else {
				var controller = collectionView as CityCollectionView;
				return controller.Source.Cities[indexPath.Row].CanSelect;
			}
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var controller = collectionView as CityCollectionView;
			App.SelectedCity = controller.Source.Cities [indexPath.Row];

			// Close Collection
			controller.ParentController.DismissViewController(true,null);
		}
		#endregion
	}
}

