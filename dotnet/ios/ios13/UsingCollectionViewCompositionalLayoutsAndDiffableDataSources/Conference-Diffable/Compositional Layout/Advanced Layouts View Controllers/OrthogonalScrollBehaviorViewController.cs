/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Orthogonal scrolling section behaviors example
*/

namespace Conference_Diffable.CompositionalLayout.AdvancedLayoutsViewControllers;

public partial class OrthogonalScrollBehaviorViewController : UIViewController, IUICollectionViewDelegate {
	static readonly string headerElementKind = nameof (headerElementKind);

	class SectionKind : NSObject, IEquatable<SectionKind> {
		readonly string description;

		static SectionKind Unknown { get; } = new SectionKind (-1, nameof (Unknown));
		static SectionKind Continuous { get; } = new SectionKind (0, nameof (Continuous));
		static SectionKind ContinuousGroupLeadingBoundary { get; } = new SectionKind (1, nameof (ContinuousGroupLeadingBoundary));
		static SectionKind Paging { get; } = new SectionKind (2, nameof (Paging));
		static SectionKind GroupPaging { get; } = new SectionKind (3, nameof (GroupPaging));
		static SectionKind GroupPagingCentered { get; } = new SectionKind (4, nameof (GroupPagingCentered));
		static SectionKind None { get; } = new SectionKind (5, nameof (None));

		public int EnumValue { get; private set; }

		public static SectionKind [] AllSections { get; } = {
			Continuous, ContinuousGroupLeadingBoundary, Paging, GroupPaging, GroupPagingCentered, None
		};

		SectionKind (int enumValue, string description)
		{
			EnumValue = enumValue;
			this.description = description;
		}

		public static SectionKind GetSectionKind (nint sectionIndex)
			=> (int)sectionIndex switch {
				-1 => Unknown,
				0 => Continuous,
				1 => ContinuousGroupLeadingBoundary,
				2 => Paging,
				3 => GroupPaging,
				4 => GroupPagingCentered,
				5 => None,
				_ => Unknown,
			};

		public UICollectionLayoutSectionOrthogonalScrollingBehavior GetOrthogonalScrollingBehavior ()
			=> EnumValue switch {
				0 => UICollectionLayoutSectionOrthogonalScrollingBehavior.Continuous,
				1 => UICollectionLayoutSectionOrthogonalScrollingBehavior.ContinuousGroupLeadingBoundary,
				2 => UICollectionLayoutSectionOrthogonalScrollingBehavior.Paging,
				3 => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPaging,
				4 => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPagingCentered,
				5 => UICollectionLayoutSectionOrthogonalScrollingBehavior.None,
				_ => UICollectionLayoutSectionOrthogonalScrollingBehavior.None,
			};

		public static bool operator == (SectionKind left, SectionKind right)
		{
			if (ReferenceEquals (left, right))
				return true;

			if (left is null)
				return false;

			if (right is null)
				return false;

			return left.Equals (right);
		}

		public static bool operator != (SectionKind left, SectionKind right) => !(left == right);
		public override bool Equals (object obj) => this == (SectionKind)obj;
		public bool Equals (SectionKind? other) => EnumValue == other?.EnumValue;
		public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), EnumValue);

		public override string ToString () => description;
	}

	UICollectionViewDiffableDataSource<NSNumber, NSNumber>? dataSource;
	UICollectionView? collectionView;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.

		NavigationItem.Title = "Orthogonal Section Behaviors";
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
		var config = new UICollectionViewCompositionalLayoutConfiguration { InterSectionSpacing = 20 };
		return new UICollectionViewCompositionalLayout (SectionProviderHandler, config);

		NSCollectionLayoutSection SectionProviderHandler (nint sectionIndex, INSCollectionLayoutEnvironment layoutEnvironment)
		{
			var sectionKind = SectionKind.GetSectionKind (sectionIndex);

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

			var orthogonallyScrolls = sectionKind.GetOrthogonalScrollingBehavior () != UICollectionLayoutSectionOrthogonalScrollingBehavior.None;
			var containerGroupFractionalWidth = orthogonallyScrolls ? .85f : 1;
			var containerGroupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (containerGroupFractionalWidth),
				NSCollectionLayoutDimension.CreateFractionalHeight (.4f));
			var containerGroup = NSCollectionLayoutGroup.CreateHorizontal (containerGroupSize, leadingItem, trailingGroup);

			var section = NSCollectionLayoutSection.Create (containerGroup);
			section.OrthogonalScrollingBehavior = sectionKind.GetOrthogonalScrollingBehavior ();

			var sectionHeaderSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateAbsolute (44));
			var sectionHeader = NSCollectionLayoutBoundarySupplementaryItem.Create (sectionHeaderSize, headerElementKind, NSRectAlignment.Top);
			section.BoundarySupplementaryItems = new [] { sectionHeader };

			return section;
		}
	}

	void ConfigureHierarchy ()
	{
		if (View is null)
			throw new InvalidOperationException (nameof (View));

		collectionView = new UICollectionView (View.Bounds, CreateLayout ()) {
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
			BackgroundColor = UIColor.SystemBackgroundColor,
			Delegate = this
		};
		View.AddSubview (collectionView);
		collectionView.RegisterClassForCell (typeof (TextCell), TextCell.Key);
		collectionView.RegisterClassForCell (typeof (ListCell), ListCell.Key);
		collectionView.RegisterClassForSupplementaryView (typeof (TitleSupplementaryView), new NSString (headerElementKind), TitleSupplementaryView.Key);
	}

	void ConfigureDataSource ()
	{
		if (collectionView is null)
			throw new InvalidOperationException (nameof (collectionView));

		dataSource = new UICollectionViewDiffableDataSource<NSNumber, NSNumber> (collectionView, CellProviderHandler) {
			SupplementaryViewProvider = SupplementaryViewProviderHandler
		};

		// initial data
		var snapshot = new NSDiffableDataSourceSnapshot<NSNumber, NSNumber> ();
		var idOffset = 0;
		var itemsPerSection = 18;

		foreach (var section in SectionKind.AllSections) {
			snapshot.AppendSections (new [] { NSNumber.FromInt32 (section.EnumValue) });
			var items = Enumerable.Range (idOffset, itemsPerSection).Select (i => NSNumber.FromInt32 (i)).ToArray ();
			snapshot.AppendItems (items);
			idOffset += itemsPerSection;
		}

		dataSource.ApplySnapshot (snapshot, false);

		UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
		{
			// Get a cell of the desired kind.
			if (collectionView.DequeueReusableCell (TextCell.Key, indexPath) is TextCell cell){
				// Populate the cell with our item description.
				cell.Label.Text = $"{indexPath.Section}, {indexPath.Row}";
				cell.ContentView.BackgroundColor = CornflowerBlue;
				cell.ContentView.Layer.BorderColor = UIColor.Black.CGColor;
				cell.ContentView.Layer.BorderWidth = 1;
				cell.ContentView.Layer.CornerRadius = 8;
				cell.Label.TextAlignment = UITextAlignment.Center;
				cell.Label.Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Title1);

				// Return the cell.
				return cell;
			}
			throw new InvalidOperationException ("UICollectionViewCell");
		}

		UICollectionReusableView SupplementaryViewProviderHandler (UICollectionView collectionView, string kind, NSIndexPath indexPath)
		{
			var sectionKind = SectionKind.GetSectionKind (indexPath.Section);

			// Get a supplementary view of the desired kind.
			if (collectionView.DequeueReusableSupplementaryView (new NSString (kind), TitleSupplementaryView.Key, indexPath) is TitleSupplementaryView header) {
				// Populate the view with our section's description.
				header.Label.Text = $".{sectionKind}";

				// Return the view.
				return header;
			}
			throw new InvalidOperationException ("UICollectionReusableView");
		}
	}

	#region UICollectionView Delegate

	[Export ("collectionView:didSelectItemAtIndexPath:")]
	public static void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		=> collectionView.DeselectItem (indexPath, true);

	#endregion
}
