/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A basic list described by compositional layout
*/

namespace Conference_Diffable.CompositionalLayout.BasicsViewControllers;

public partial class ListViewController : UIViewController, IUICollectionViewDelegate {
	class Section : NSObject, IEquatable<Section> {
		public static Section Main { get; } = new Section (1);

		public int Value { get; private set; }

		Section (int value) => Value = value;

		public static bool operator == (Section left, Section right)
		{
			if (ReferenceEquals (left, right))
				return true;

			if (left is null)
				return false;

			if (right is null)
				return false;

			return left.Equals (right);
		}

		public static bool operator != (Section left, Section right) => !(left == right);
		public override bool Equals (object obj) => this == (Section)obj;
		public bool Equals (Section? other) => Value == other?.Value;
		public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Value);
	}

	UICollectionViewDiffableDataSource<Section, NSNumber>? dataSource;
	UICollectionView? collectionView;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.

		NavigationItem.Title = "List";
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

		var layout = new UICollectionViewCompositionalLayout (section);
		return layout;
	}

	void ConfigureHierarchy ()
	{
		if (View is null)
			throw new InvalidOperationException ("View");

		collectionView = new UICollectionView (View.Bounds, CreateLayout ()) {
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
			BackgroundColor = UIColor.SystemBackgroundColor,
			Delegate = this
		};
		View.AddSubview (collectionView);
		collectionView.RegisterClassForCell (typeof (ListCell), ListCell.Key);

	}

	void ConfigureDataSource ()
	{
		if (collectionView is null)
			throw new InvalidOperationException ("collectionView");

		dataSource = new UICollectionViewDiffableDataSource<Section, NSNumber> (collectionView, CellProviderHandler);

		var items = Enumerable.Range (0, 94).Select (i => NSNumber.FromInt32 (i)).ToArray ();

		// initial data
		var snapshot = new NSDiffableDataSourceSnapshot<Section, NSNumber> ();
		snapshot.AppendSections (new [] { Section.Main });
		snapshot.AppendItems (items);
		dataSource.ApplySnapshot (snapshot, false);

		UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
		{
			var id = (obj as NSNumber)?.Int32Value;
			// Get a cell of the desired kind.
			var cell = collectionView.DequeueReusableCell (ListCell.Key, indexPath) as ListCell;

			if (cell is null || cell.Label is null)
				throw new InvalidOperationException ("cell or cell.Label");

			// Populate the cell with our item description.
			cell.Label.Text = id.ToString ();

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
