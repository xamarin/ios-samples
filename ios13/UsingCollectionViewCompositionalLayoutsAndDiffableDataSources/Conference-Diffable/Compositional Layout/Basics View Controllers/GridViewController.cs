/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A basic grid described by compositional layout
*/

using System;
using System.Linq;
using Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;
using Foundation;
using UIKit;

namespace Conference_Diffable.CompositionalLayout.BasicsViewControllers {
	public partial class GridViewController : UIViewController {
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

			NavigationItem.Title = "Grid";
			ConfigureHierarchy ();
			ConfigureDataSource ();
		}

		UICollectionViewLayout CreateLayout ()
		{
			var itemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (.2f),
				NSCollectionLayoutDimension.CreateFractionalHeight (1));
			var item = NSCollectionLayoutItem.Create (itemSize);

			var groupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateFractionalWidth (.2f));
			var group = NSCollectionLayoutGroup.CreateHorizontal (groupSize, item);

			var section = NSCollectionLayoutSection.Create (group);

			var layout = new UICollectionViewCompositionalLayout (section);
			return layout;
		}

		void ConfigureHierarchy ()
		{
			collectionView = new UICollectionView (View.Bounds, CreateLayout ()) {
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				BackgroundColor = UIColor.SystemBackgroundColor
			};
			collectionView.RegisterClassForCell (typeof (TextCell), TextCell.Key);
			View.AddSubview (collectionView);
		}

		void ConfigureDataSource ()
		{
			dataSource = new UICollectionViewDiffableDataSource<Section, NSNumber> (collectionView, CellProviderHandler);

			var items = Enumerable.Range (0, 94).Select (i => NSNumber.FromInt32 (i)).ToArray ();

			// initial data
			var snapshot = new NSDiffableDataSourceSnapshot<Section, NSNumber> ();
			snapshot.AppendSections (new [] { Section.Main });
			snapshot.AppendItems (items);
			dataSource.ApplySnapshot (snapshot, false);

			UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
			{
				var id = (obj as NSNumber).Int32Value;
				// Get a cell of the desired kind.
				var cell = collectionView.DequeueReusableCell (TextCell.Key, indexPath) as TextCell;

				// Populate the cell with our item description.
				cell.Label.Text = id.ToString ();
				cell.ContentView.BackgroundColor = UIColorExtensions.CornflowerBlue;
				cell.Layer.BorderColor = UIColor.Black.CGColor;
				cell.Layer.BorderWidth = 1;
				cell.Label.TextAlignment = UITextAlignment.Center;
				cell.Label.Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Title1);

				// Return the cell.
				return cell;
			}
		}
	}
}

