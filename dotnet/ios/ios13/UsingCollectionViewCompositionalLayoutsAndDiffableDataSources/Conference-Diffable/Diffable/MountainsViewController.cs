/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Sample showing how we might create a search UI using a diffable data source
*/

using Conference_Diffable.Diffable.CellsAndSupplementaryViews;

namespace Conference_Diffable.Diffable;

public partial class MountainsViewController : UIViewController, IUISearchBarDelegate {
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

	MountainsController? mountainsController;
	UISearchBar? searchBar;
	UICollectionView? mountainCollectionView;
	UICollectionViewDiffableDataSource<Section, MountainsController.Mountain>? dataSource;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.

		NavigationItem.Title = "Mountains Search";
		ConfigureHierarchy ();
		ConfigureDataSource ();
		PerformQuery (null);
	}

	UICollectionViewLayout CreateLayout ()
	{
		return new UICollectionViewCompositionalLayout (SectionProviderHandler);

		NSCollectionLayoutSection SectionProviderHandler (nint sectionIndex, INSCollectionLayoutEnvironment layoutEnvironment)
		{
			var contentSize = layoutEnvironment.Container.EffectiveContentSize;
			var columns = contentSize.Width > 600 ? 3 : 2;
			var spacing = 10;

			var itemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateFractionalHeight (1));
			var item = NSCollectionLayoutItem.Create (itemSize);

			var groupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateAbsolute (32));
			var group = NSCollectionLayoutGroup.CreateHorizontal (groupSize, item, columns);
			group.InterItemSpacing = NSCollectionLayoutSpacing.CreateFixed (spacing);

			var section = NSCollectionLayoutSection.Create (group);
			section.InterGroupSpacing = spacing;
			section.ContentInsets = new NSDirectionalEdgeInsets (10, 10, 10, 10);

			return section;
		}
	}

	void ConfigureHierarchy ()
	{
		mountainsController = new MountainsController ();

		View!.BackgroundColor = UIColor.SystemBackgroundColor;
		mountainCollectionView = new UICollectionView (View.Bounds, CreateLayout ()) {
			TranslatesAutoresizingMaskIntoConstraints = false,
			BackgroundColor = UIColor.SystemBackgroundColor
		};
		View.AddSubview (mountainCollectionView);
		mountainCollectionView.RegisterClassForCell (typeof (LabelCell), LabelCell.Key);

		searchBar = new UISearchBar (CGRect.Empty) {
			TranslatesAutoresizingMaskIntoConstraints = false,
			Delegate = this
		};
		View.AddSubview (searchBar);

		string [] keys = { "cv", "searchBar" };
		UIView [] values = { mountainCollectionView, searchBar };
		var views = NSDictionary.FromObjectsAndKeys (values, keys, keys.Length);

		var constraints = new List<NSLayoutConstraint> ();
		constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("H:|[cv]|", 0, null, views));
		constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("H:|[searchBar]|", 0, null, views));
		constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("V:[searchBar]-20-[cv]|", 0, null, views));
		constraints.Add (searchBar.TopAnchor.ConstraintEqualToSystemSpacingBelowAnchor (View.SafeAreaLayoutGuide.TopAnchor, 1));
		NSLayoutConstraint.ActivateConstraints (constraints.ToArray ());
	}

	void ConfigureDataSource ()
	{
		dataSource = new UICollectionViewDiffableDataSource<Section, MountainsController.Mountain> (mountainCollectionView!, CellProviderHandler);

		static UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
		{
			var mountain = obj as MountainsController.Mountain;
			// Get a cell of the desired kind.
			var mountainCell = collectionView.DequeueReusableCell (LabelCell.Key, indexPath) as LabelCell;

			// Populate the cell with our item description.
			mountainCell!.Label!.Text = mountain?.Name;

			// Return the cell.
			return mountainCell;
		}
	}

	#region UISearchBar Delegate

	[Export ("searchBar:textDidChange:")]
	public void TextChanged (UISearchBar searchBar, string searchText) => PerformQuery (searchText);

	#endregion

	void PerformQuery (string? filter)
	{
		var mountains = mountainsController?.FilterMountains (filter).OrderBy (m => m.Name).ToArray ();

		var snapshot = new NSDiffableDataSourceSnapshot<Section, MountainsController.Mountain> ();
		snapshot.AppendSections (new [] { Section.Main });
		snapshot.AppendItems (mountains!);
		dataSource!.ApplySnapshot (snapshot, true);
	}
}
