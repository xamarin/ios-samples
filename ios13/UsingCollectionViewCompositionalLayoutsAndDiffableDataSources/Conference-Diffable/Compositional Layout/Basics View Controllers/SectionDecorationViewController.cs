/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Section background decoration view example
*/

using System;
using System.Linq;
using Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;
using Foundation;
using UIKit;

namespace Conference_Diffable.CompositionalLayout.BasicsViewControllers {
	public partial class SectionDecorationViewController : UIViewController, IUICollectionViewDelegate {
		static readonly NSString sectionBackgroundDecorationElementKind = new NSString (nameof (sectionBackgroundDecorationElementKind));

		NSDiffableDataSourceSnapshot<NSNumber, NSNumber> currentSnapshot;
		UICollectionViewDiffableDataSource<NSNumber, NSNumber> dataSource;
		UICollectionView collectionView;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			NavigationItem.Title = "Section Background Decoration View";
			ConfigureHierarchy ();
			ConfigureDataSource ();
		}

		UICollectionViewLayout CreateLayout ()
		{
			var itemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateFractionalHeight (1));
			var item = NSCollectionLayoutItem.Create (itemSize);

			var groupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateAbsolute (44));
			var group = NSCollectionLayoutGroup.CreateHorizontal (groupSize, item);

			var section = NSCollectionLayoutSection.Create (group);
			section.InterGroupSpacing = 5;
			section.ContentInsets = new NSDirectionalEdgeInsets (10, 10, 10, 10);

			var sectionBackgroundDecoration = NSCollectionLayoutDecorationItem.Create (sectionBackgroundDecorationElementKind);
			sectionBackgroundDecoration.ContentInsets = new NSDirectionalEdgeInsets (5, 5, 5, 5);
			section.DecorationItems = new [] { sectionBackgroundDecoration };

			var layout = new UICollectionViewCompositionalLayout (section);
			layout.RegisterClassForDecorationView (typeof (SectionBackgroundDecorationView), new NSString (sectionBackgroundDecorationElementKind));

			return layout;
		}

		void ConfigureHierarchy ()
		{
			collectionView = new UICollectionView (View.Bounds, CreateLayout ()) {
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				BackgroundColor = UIColor.SystemBackgroundColor,
				Delegate = this
			};
			collectionView.RegisterClassForCell (typeof (ListCell), ListCell.Key);
			View.AddSubview (collectionView);
		}

		void ConfigureDataSource ()
		{
			dataSource = new UICollectionViewDiffableDataSource<NSNumber, NSNumber> (collectionView, CellProviderHandler);

			// initial data
			var itemsPerSection = 5;
			var sections = Enumerable.Range (0, 5).Select (i => NSNumber.FromInt32 (i)).ToArray ();
			currentSnapshot = new NSDiffableDataSourceSnapshot<NSNumber, NSNumber> ();
			var itemOffset = 0;

			foreach (var section in sections) {
				currentSnapshot.AppendSections (new [] { section });
				var items = Enumerable.Range (itemOffset, itemsPerSection).Select (i => NSNumber.FromInt32 (i)).ToArray ();
				currentSnapshot.AppendItems (items);
				itemOffset += itemsPerSection;
			}

			dataSource.ApplySnapshot (currentSnapshot, false);

			UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
			{
				var sectionId = currentSnapshot.SectionIdentifiers [indexPath.Section];
				var numberOfItemsInSection = currentSnapshot.GetNumberOfItems (sectionId);
				var isLastCell = indexPath.Item + 1 == numberOfItemsInSection;

				// Get a cell of the desired kind.
				var cell = collectionView.DequeueReusableCell (ListCell.Key, indexPath) as ListCell;

				// Populate the cell with our item description.
				cell.Label.Text = $"{indexPath.Section}, {indexPath.Row}";
				cell.SeparatorView.Hidden = isLastCell;

				// Return the cell.
				return cell;
			}
		}

		#region UICollectionView Delegate

		[Export ("collectionView:didSelectItemAtIndexPath:")]
		public void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
			=> collectionView.DeselectItem (indexPath, true);

		#endregion
	}
}

