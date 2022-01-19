/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A simple outline view for the sample apps main UI
*/

using Conference_Diffable.CompositionalLayout.AdvancedLayoutsViewControllers;
using Conference_Diffable.CompositionalLayout.AppSamplesViewControllers;
using Conference_Diffable.CompositionalLayout.BasicsViewControllers;
using Conference_Diffable.Diffable;

namespace Conference_Diffable;

public partial class OutlineViewController : UIViewController, IUICollectionViewDelegate {
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

	class OutlineItem : NSObject {
		public string Id { get; private set; }
		public string Title { get; private set; }
		public int IndentLevel { get; private set; }
		public OutlineItem []? Subitems { get; private set; }
		public UIViewController? OutlineViewController { get; private set; }
		public bool Expanded { get; set; }
		public bool IsGroup { get => OutlineViewController is null; }

		public OutlineItem (string title, int indentLevel = 0, UIViewController? outlineViewController = null, OutlineItem []? subitems = null)
		{
			Title = title;
			IndentLevel = indentLevel;
			Subitems = subitems;
			OutlineViewController = outlineViewController;

			Id = new NSUuid ().ToString ();
		}

		public static bool operator == (OutlineItem left, OutlineItem right)
		{
			if (ReferenceEquals (left, right))
				return true;

			if (left is null)
				return false;

			if (right is null)
				return false;

			return left.Equals (right);
		}

		public static bool operator != (OutlineItem left, OutlineItem right) => !(left == right);
		public override bool Equals (object obj) => this == (OutlineItem)obj;
		public bool Equals (OutlineItem other) => Id == other.Id;
		public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);
	}

	#region Constructors

	public OutlineViewController (IntPtr handle) : base (handle)
	{
	}

	#endregion

	UICollectionViewDiffableDataSource<Section, OutlineItem>? dataSource;
	UICollectionView? outlineCollectionView;
	OutlineItem [] menuItems = new [] {
		new OutlineItem ("Compositional Layout", subitems: new [] {
			new OutlineItem ("Getting Started", 1, subitems: new [] {
				new OutlineItem ("List", 2, new ListViewController ()),
				new OutlineItem ("Grid", 2, new GridViewController ()),
				new OutlineItem ("Inset Items Grid", 2, new InsetItemsGridViewController ()),
				new OutlineItem ("Two-Column Grid", 2, new TwoColumnViewController ()),
				new OutlineItem ("Per-Section Layout", 2, subitems: new [] {
					new OutlineItem ("Distinct Sections", 3, new DistinctSectionsViewController ()),
					new OutlineItem ("Adaptive Sections", 3, new AdaptiveSectionsViewController ())
				})
			}),
			new OutlineItem ("Advanced Layouts", 1, subitems: new [] {
				new OutlineItem ("Supplementary Views", 2, subitems: new [] {
					new OutlineItem ("Item Badges", 3, new ItemBadgeSupplementaryViewController ()),
					new OutlineItem ("Section Headers/Footers", 3, new SectionHeadersFootersViewController ()),
					new OutlineItem ("Pinnned Section Headers", 3, new PinnedSectionHeaderFooterViewController ()),
				}),
				new OutlineItem ("Section Background Decoration", 2, new SectionDecorationViewController ()),
				new OutlineItem ("Nested Groups", 2, new NestedGroupsViewController ()),
				new OutlineItem ("Orthogonal Sections", 2, subitems: new [] {
					new OutlineItem ("Orthogonal Sections", 3, new OrthogonalScrollingViewController ()),
					new OutlineItem ("Orthogonal Section Behaviors", 3, new OrthogonalScrollBehaviorViewController ()),
				}),
			}),
			new OutlineItem ("Conference App", 1, subitems: new [] {
				new OutlineItem ("Videos", 2, new ConferenceVideoSessionsViewController ()),
				new OutlineItem ("News", 2, new ConferenceNewsFeedViewController ()),
			}),
		}),
		new OutlineItem ("Diffable Data Source", subitems: new [] {
			new OutlineItem ("Mountains Search", 1, new MountainsViewController ()),
			new OutlineItem ("Settings: Wi-Fi", 1, new WiFiSettingsViewController ()),
			new OutlineItem ("Insertion Sort Visualization", 1, new InsertionSortViewController ()),
			new OutlineItem ("UITableView: Editing", 1, new TableViewEditingViewController ()),
		})
	};

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		NavigationItem.Title = "Diffable + CompLayout";
		ConfigureCollectionView ();
		ConfigureDataSource ();
	}

	void ConfigureCollectionView ()
	{
		if (View is null)
			throw new InvalidOperationException (nameof (View));

		var collectionView = new UICollectionView (View.Bounds, GenerateLayout ()) {
			AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
			BackgroundColor = UIColor.SystemBackgroundColor,
			Delegate = this,
		};
		View.AddSubview (collectionView);
		collectionView.RegisterClassForCell (typeof (OutlineItemCell), OutlineItemCell.Key);
		outlineCollectionView = collectionView;
	}

	void ConfigureDataSource ()
	{
		if (outlineCollectionView is null)
			throw new InvalidOperationException (nameof (outlineCollectionView));

		dataSource = new UICollectionViewDiffableDataSource<Section, OutlineItem> (outlineCollectionView, CellProviderHandler);

		// load our initial data
		var snapshot = GetSnapshotForCurrentState ();
		dataSource.ApplySnapshot (snapshot, false);

		UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
		{
			if (obj is OutlineItem menuItem) {
				if (collectionView.DequeueReusableCell (OutlineItemCell.Key, indexPath) is OutlineItemCell cell) {
					cell.ConfigureIfNeeded ();

					cell.Label.Text = menuItem.Title;
					cell.IndentLevel = menuItem.IndentLevel;
					cell.Group = menuItem.IsGroup;
					cell.Expanded = menuItem.Expanded;
					return cell;
				}
				throw new InvalidOperationException ("UICollectionViewCell");
			}
			throw new InvalidOperationException ("OutlineItem");
		}
	}

	static UICollectionViewLayout GenerateLayout ()
	{
		var itemHeightDimension = NSCollectionLayoutDimension.CreateAbsolute (44);
		var itemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1), itemHeightDimension);
		var item = NSCollectionLayoutItem.Create (itemSize);

		var groupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1), itemHeightDimension);
		var group = NSCollectionLayoutGroup.CreateHorizontal (groupSize, item, 1);

		var section = NSCollectionLayoutSection.Create (group);
		section.ContentInsets = new NSDirectionalEdgeInsets (0, 10, 0, 10);

		return new UICollectionViewCompositionalLayout (section);
	}

	NSDiffableDataSourceSnapshot<Section, OutlineItem> GetSnapshotForCurrentState ()
	{
		var snapshot = new NSDiffableDataSourceSnapshot<Section, OutlineItem> ();
		snapshot.AppendSections (new [] { Section.Main });

		void AddItems (OutlineItem menuItem)
		{
			snapshot.AppendItems (new [] { menuItem });
			if (menuItem.Expanded && menuItem.Subitems is not null)
				foreach (var item in menuItem.Subitems)
					AddItems (item);
		}

		foreach (var menuItem in menuItems)
			AddItems (menuItem);

		return snapshot;
	}

	void UpdateUI ()
	{
		var snapshot = GetSnapshotForCurrentState ();

		if (dataSource is not null)
			dataSource.ApplySnapshot (snapshot, true);
	}

	#region UICollectionView Delegate

	[Export ("collectionView:didSelectItemAtIndexPath:")]
	public void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
	{
		if (dataSource is null)
			return;

		var menuItem = dataSource.GetItemIdentifier (indexPath);
		collectionView.DeselectItem (indexPath, true);

		if (menuItem is null)
			return;

		if (menuItem.IsGroup) {
			menuItem.Expanded = !menuItem.Expanded;
			if (collectionView.CellForItem (indexPath) is OutlineItemCell cell) {
				UIView.Animate (0.3, () => {
					cell.Expanded = menuItem.Expanded;
					UpdateUI ();
				});
			}
		} else {
			if (menuItem.OutlineViewController is null)
				return;

			var navigationController = new UINavigationController (menuItem.OutlineViewController);
			PresentViewController (navigationController, true, null);
		}
	}

	#endregion
}
