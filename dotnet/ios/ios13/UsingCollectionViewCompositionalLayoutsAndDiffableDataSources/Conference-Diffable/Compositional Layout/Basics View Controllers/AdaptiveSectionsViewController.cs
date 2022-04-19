/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A layout that adapts to a changing layout environment
*/

namespace Conference_Diffable.CompositionalLayout.BasicsViewControllers;

public partial class AdaptiveSectionsViewController : UIViewController, IUICollectionViewDelegate {
	class SectionLayoutKind : NSObject, IEquatable<SectionLayoutKind> {
		public static SectionLayoutKind Unknown { get; } = new SectionLayoutKind (-1);
		public static SectionLayoutKind List { get; } = new SectionLayoutKind (0);
		public static SectionLayoutKind Grid5 { get; } = new SectionLayoutKind (1);
		public static SectionLayoutKind Grid3 { get; } = new SectionLayoutKind (2);

		public int EnumValue { get; private set; }

		public static SectionLayoutKind [] AllSections { get; } = { List, Grid5, Grid3 };

		SectionLayoutKind (int enumValue) => EnumValue = enumValue;

		public static SectionLayoutKind GetSectionLayoutKind (nint sectionIndex)
		{
			if (List.EnumValue == sectionIndex) return List;
			if (Grid5.EnumValue == sectionIndex) return Grid5;
			if (Grid3.EnumValue == sectionIndex) return Grid3;
			return Unknown;
		}

		public int GetColumnCount (nfloat width)
		{
			var wideMode = width > 600;

			if (List.EnumValue == EnumValue) return wideMode ? 2 : 1;
			if (Grid5.EnumValue == EnumValue) return wideMode ? 10 : 5;
			if (Grid3.EnumValue == EnumValue) return wideMode ? 6 : 3;
			return 0;
		}

		public static bool operator == (SectionLayoutKind left, SectionLayoutKind right)
		{
			if (ReferenceEquals (left, right))
				return true;

			if (left is null)
				return false;

			if (right is null)
				return false;

			return left.Equals (right);
		}

		public static bool operator != (SectionLayoutKind left, SectionLayoutKind right) => !(left == right);
		public override bool Equals (object obj) => this == (SectionLayoutKind)obj;
		public bool Equals (SectionLayoutKind? other) => EnumValue == other?.EnumValue;
		public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), EnumValue);
	}

	UICollectionViewDiffableDataSource<SectionLayoutKind, NSNumber>? dataSource;
	UICollectionView? collectionView;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.

		NavigationItem.Title = "Adaptive Sections";
		ConfigureHierarchy ();
		ConfigureDataSource ();
	}

	static UICollectionViewLayout? CreateLayout ()
	{
		if (SectionProviderHandler != null)
			return new UICollectionViewCompositionalLayout (SectionProviderHandler!);

		return null;

		static NSCollectionLayoutSection? SectionProviderHandler (nint sectionIndex, INSCollectionLayoutEnvironment layoutEnvironment)
		{
			var sectionLayoutKind = SectionLayoutKind.GetSectionLayoutKind (sectionIndex);

			if (sectionLayoutKind == SectionLayoutKind.Unknown)
				return null;

			var columns = sectionLayoutKind.GetColumnCount (layoutEnvironment.Container.EffectiveContentSize.Width);

			var itemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (.2f),
				NSCollectionLayoutDimension.CreateFractionalHeight (1));
			var item = NSCollectionLayoutItem.Create (itemSize);
			item.ContentInsets = new NSDirectionalEdgeInsets (2, 2, 2, 2);

			var groupHeight = sectionLayoutKind == SectionLayoutKind.List ?
				NSCollectionLayoutDimension.CreateAbsolute (44) : NSCollectionLayoutDimension.CreateFractionalWidth (.2f);
			var groupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1), groupHeight);
			var group = NSCollectionLayoutGroup.CreateHorizontal (groupSize, item, columns);

			var section = NSCollectionLayoutSection.Create (group);
			section.ContentInsets = new NSDirectionalEdgeInsets (20, 20, 20, 20);
			return section;
		}
	}

	void ConfigureHierarchy ()
	{
		if (View is null)
			throw new InvalidOperationException (nameof (View));

		var layout = CreateLayout ();
		if (layout is null)
			return;

		collectionView = new UICollectionView (View.Bounds, layout) {
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
			BackgroundColor = UIColor.SystemBackground,
			Delegate = this
		};
		collectionView.RegisterClassForCell (typeof (TextCell), TextCell.Key);
		collectionView.RegisterClassForCell (typeof (ListCell), ListCell.Key);
		View.AddSubview (collectionView);
	}

	void ConfigureDataSource ()
	{
		if (collectionView is null)
			throw new InvalidOperationException (nameof (collectionView));

		dataSource = new UICollectionViewDiffableDataSource<SectionLayoutKind, NSNumber> (collectionView, CellProviderHandler);

		// initial data
		var itemsPerSection = 10;
		var snapshot = new NSDiffableDataSourceSnapshot<SectionLayoutKind, NSNumber> ();

		foreach (var section in SectionLayoutKind.AllSections) {
			snapshot.AppendSections (new [] { section });

			var itemOffset = section.EnumValue * itemsPerSection;
			var items = Enumerable.Range (itemOffset, itemsPerSection).Select (i => NSNumber.FromInt32 (i)).ToArray ();
			snapshot.AppendItems (items);
		}

		dataSource.ApplySnapshot (snapshot, false);

		static UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
		{
			var id = (obj as NSNumber)?.Int32Value;
			var section = SectionLayoutKind.GetSectionLayoutKind (indexPath.Section);

			if (section == SectionLayoutKind.List) {
				// Get a cell of the desired kind.
				if (collectionView.DequeueReusableCell (ListCell.Key, indexPath) is ListCell cell) {

					// Populate the cell with our item description.
					cell.Label.Text = id?.ToString ();

					// Return the cell.
					return cell;
				}
				throw new InvalidOperationException ("UICollectionViewCell");
			} else {
				// Get a cell of the desired kind.
				if (collectionView.DequeueReusableCell (TextCell.Key, indexPath) is TextCell cell) {
					cell.Label.Text = id.ToString ();
					cell.ContentView.BackgroundColor = CornflowerBlue;
					cell.ContentView.Layer.CornerRadius = section == SectionLayoutKind.Grid5 ? 8 : 0;
					cell.ContentView.Layer.BorderColor = UIColor.Black.CGColor;
					cell.ContentView.Layer.BorderWidth = 1;
					cell.Label.TextAlignment = UITextAlignment.Center;
					cell.Label.Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Title1);

					// Return the cell.
					return cell;
				}
				throw new InvalidOperationException ("UICollectionViewCell");
			}
		}
	}

	#region UICollectionView Delegate

	[Export ("collectionView:didSelectItemAtIndexPath:")]
	public static void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		=> collectionView.DeselectItem (indexPath, true);

	#endregion
}
