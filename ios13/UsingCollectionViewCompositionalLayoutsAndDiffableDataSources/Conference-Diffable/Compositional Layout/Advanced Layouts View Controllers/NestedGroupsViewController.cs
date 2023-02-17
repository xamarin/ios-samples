/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Nested NSCollectionLayoutGroup example
*/

using System;
using System.Linq;
using Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;
using Foundation;
using UIKit;

namespace Conference_Diffable.CompositionalLayout.AdvancedLayoutsViewControllers {
	public partial class NestedGroupsViewController : UIViewController, IUICollectionViewDelegate {
		class Section : NSObject, IEquatable<Section> {
			public static Section Main { get; } = new Section (1);

			public int Value { get; private set; }

			Section (int value) => Value = value;

			public static bool operator == (Section left, Section right)
			{
				if (ReferenceEquals (left, right))
					return true;

				if (ReferenceEquals (left, null))
					return false;

				if (ReferenceEquals (right, null))
					return false;

				return left.Equals (right);
			}

			public static bool operator != (Section left, Section right) => !(left == right);
			public override bool Equals (object obj) => this == (Section) obj;
			public bool Equals (Section other) => Value == other.Value;
			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Value);
		}

		UICollectionViewDiffableDataSource<Section, NSNumber> dataSource;
		UICollectionView collectionView;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			NavigationItem.Title = "Nested Groups";
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

			NSCollectionLayoutSection SectionProviderHandler (nint section, INSCollectionLayoutEnvironment layoutEnvironment)
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

				var nestedGroupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
					NSCollectionLayoutDimension.CreateFractionalHeight (.4f));
				var nestedGroup = NSCollectionLayoutGroup.CreateHorizontal (nestedGroupSize, leadingItem, trailingGroup);

				return NSCollectionLayoutSection.Create (nestedGroup);
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
			dataSource = new UICollectionViewDiffableDataSource<Section, NSNumber> (collectionView, CellProviderHandler);

			var items = Enumerable.Range (0, 100).Select (i => NSNumber.FromInt32 (i)).ToArray ();

			// initial data
			var snapshot = new NSDiffableDataSourceSnapshot<Section, NSNumber> ();
			snapshot.AppendSections (new [] { Section.Main });
			snapshot.AppendItems (items);
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

