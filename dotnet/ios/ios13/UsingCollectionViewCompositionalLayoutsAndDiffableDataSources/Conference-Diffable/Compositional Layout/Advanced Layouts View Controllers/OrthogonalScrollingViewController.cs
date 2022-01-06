using System;
using System.Linq;
using Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;
using Foundation;
using UIKit;

namespace Conference_Diffable.CompositionalLayout.AdvancedLayoutsViewControllers {
	public partial class OrthogonalScrollingViewController : UIViewController, IUICollectionViewDelegate {
		UICollectionViewDiffableDataSource<NSNumber, NSNumber> dataSource;
		UICollectionView collectionView;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			NavigationItem.Title = "Orthogonal Sections";
			ConfigureHierarchy ();
			ConfigureDataSource ();
		}

		//   +-----------------------------------------------------+
		//   | +---------------------------------+  +-----------+  |
		//   | |                                 |  |           |  |
		//   | |                                 |  |           |  |
		//   | |                                 |  |     1     |  |
		//   | |                                 |  |           |  |
		//   | |                                 |  |           |  |
		//   | |                                 |  +-----------+  |
		//   | |               0                 |                 |
		//   | |                                 |  +-----------+  |
		//   | |                                 |  |           |  |
		//   | |                                 |  |           |  |
		//   | |                                 |  |     2     |  |
		//   | |                                 |  |           |  |
		//   | |                                 |  |           |  |
		//   | +---------------------------------+  +-----------+  |
		//   +-----------------------------------------------------+

		UICollectionViewLayout CreateLayout ()
		{
			return new UICollectionViewCompositionalLayout (SectionProviderHandler);

			NSCollectionLayoutSection SectionProviderHandler (nint sectionIndex, INSCollectionLayoutEnvironment layoutEnvironment)
			{
				var leadingItemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (.7f),
					NSCollectionLayoutDimension.CreateFractionalHeight (1));
				var leadingItem = NSCollectionLayoutItem.Create (leadingItemSize);
				leadingItem.ContentInsets = new NSDirectionalEdgeInsets (10, 10, 10, 10);

				var trailingItemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
					NSCollectionLayoutDimension.CreateFractionalHeight (.3f));
				var trailingItem = NSCollectionLayoutItem.Create (trailingItemSize);
				trailingItem.ContentInsets = new NSDirectionalEdgeInsets (10, 10, 10, 10);

				var trailingGroupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (.3f),
					NSCollectionLayoutDimension.CreateFractionalHeight (1));
				var trailingGroup = NSCollectionLayoutGroup.CreateVertical (trailingGroupSize, trailingItem, 2);

				var containerGroupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (.85f),
					NSCollectionLayoutDimension.CreateFractionalHeight (.4f));
				var containerGroup = NSCollectionLayoutGroup.CreateHorizontal (containerGroupSize, leadingItem, trailingGroup);

				var section = NSCollectionLayoutSection.Create (containerGroup);
				section.OrthogonalScrollingBehavior = UICollectionLayoutSectionOrthogonalScrollingBehavior.Continuous;

				return section;
			}
		}

		void ConfigureHierarchy ()
		{
			collectionView = new UICollectionView (View.Bounds, CreateLayout ()) {
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				BackgroundColor = UIColor.SystemBackgroundColor,
				Delegate = this
			};
			View.AddSubview (collectionView);
			collectionView.RegisterClassForCell (typeof (TextCell), TextCell.Key);
		}

		void ConfigureDataSource ()
		{
			dataSource = new UICollectionViewDiffableDataSource<NSNumber, NSNumber> (collectionView, CellProviderHandler);

			// initial data
			var snapshot = new NSDiffableDataSourceSnapshot<NSNumber, NSNumber> ();
			var idOffset = 0;
			var itemsPerSection = 30;
			var sections = Enumerable.Range (0, 5).Select (i => NSNumber.FromInt32 (i)).ToArray ();

			foreach (var section in sections) {
				snapshot.AppendSections (new [] { section });
				var items = Enumerable.Range (idOffset, itemsPerSection).Select (i => NSNumber.FromInt32 (i)).ToArray ();
				snapshot.AppendItems (items);
				idOffset += itemsPerSection;
			}

			dataSource.ApplySnapshot (snapshot, false);

			UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
			{
				// Get a cell of the desired kind.
				var cell = collectionView.DequeueReusableCell (TextCell.Key, indexPath) as TextCell;

				// Populate the cell with our item description.
				cell.Label.Text = $"{indexPath.Section}, {indexPath.Row}";
				cell.ContentView.BackgroundColor = UIColorExtensions.CornflowerBlue;
				cell.ContentView.Layer.BorderColor = UIColor.Black.CGColor;
				cell.ContentView.Layer.BorderWidth = 1;
				cell.ContentView.Layer.CornerRadius = 8;
				cell.Label.TextAlignment = UITextAlignment.Center;
				cell.Label.Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Title1);

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

