/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Visual illustration of an insertion sort using diffable data sources to update the UI
*/

namespace Conference_Diffable.Diffable;

public partial class InsertionSortViewController : UIViewController {
	static readonly CGSize nodeSize = new CGSize (16, 34);
	static readonly NSString key = new NSString (nameof (InsertionSortViewController));
	UICollectionView? insertionCollectionView;
	UICollectionViewDiffableDataSource<InsertionSortArray, InsertionSortArray.SortNode>? dataSource;
	bool sorting;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.

		NavigationItem.Title = "Insertion Sort Visualizer";
		ConfigureHierarchy ();
		ConfigureDataSource ();
		ConfigureNavItem ();
	}

	UICollectionViewLayout CreateLayout ()
	{
		return new UICollectionViewCompositionalLayout (SectionProviderHandler);

		NSCollectionLayoutSection SectionProviderHandler (nint sectionIndex, INSCollectionLayoutEnvironment layoutEnvironment)
		{
			var contentSize = layoutEnvironment.Container.EffectiveContentSize;
			var columns = (int)(contentSize.Width / nodeSize.Width);
			var rowHeight = nodeSize.Height;
				
			var itemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateFractionalHeight (1));
			var item = NSCollectionLayoutItem.Create (itemSize);

			var groupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateAbsolute (rowHeight));
			var group = NSCollectionLayoutGroup.CreateHorizontal (groupSize, item, columns);

			var section = NSCollectionLayoutSection.Create (group);
			return section;
		}
	}

	void ConfigureHierarchy ()
	{
		insertionCollectionView = new UICollectionView (View!.Bounds, CreateLayout ()) {
			BackgroundColor = UIColor.SystemBackgroundColor,
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
		};
		View.AddSubview (insertionCollectionView);
		insertionCollectionView.RegisterClassForCell (typeof (UICollectionViewCell), key);
	}

	void ConfigureDataSource ()
	{
		dataSource = new UICollectionViewDiffableDataSource<InsertionSortArray, InsertionSortArray.SortNode> (insertionCollectionView!, CellProviderHandler);
		var snapshot = RandomizedSnapshot (insertionCollectionView!.Bounds);
		dataSource.ApplySnapshot (snapshot, true);

		UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
		{
			var sortNode = obj as InsertionSortArray.SortNode;
			// Get a cell of the desired kind.
			var cell = collectionView.DequeueReusableCell (key, indexPath) as UICollectionViewCell;
			// Populate the cell with our item description.
			cell!.BackgroundColor = sortNode?.Color;

			// Return the cell.
			return cell;
		}
	}

	void ConfigureNavItem ()
	{
		NavigationItem.RightBarButtonItem = new UIBarButtonItem (sorting ? "Stop" : "Sort", UIBarButtonItemStyle.Plain, ToggleSort!);

		void ToggleSort (object sender, EventArgs e)
		{
			sorting = !sorting;

			if (sorting) PerformSortStep ();

			ConfigureNavItem ();
		}
	}

	private void PerformSortStep ()
	{
		if (!sorting) return;

		var sectionCountNeedingSort = 0;

		// grab the current state of the UI from the data source
		var updatedSnapshot = dataSource!.Snapshot;

		// for each section, if needed, step through and perform the next sorting step
		foreach (var section in updatedSnapshot.SectionIdentifiers) {
			if (section.Sorted) continue;

			// step the sort algorthim
			section.SortNext ();
			var items = section.Nodes;

			// replace our items for this section with the newly sorted items
			updatedSnapshot.DeleteItems (items);
			updatedSnapshot.AppendItems (items, section);

			sectionCountNeedingSort += 1;
		}

		var shouldReset = false;
		var delay = 125;

		if (sectionCountNeedingSort > 0)
			dataSource.ApplySnapshot (updatedSnapshot, true);
		else {
			delay = 1000;
			shouldReset = true;
		}

		var bounds = insertionCollectionView!.Bounds;

		Task.Factory.StartNew (() => {
			Task.Delay (delay).Wait ();

			InvokeOnMainThread (() => {
				if (shouldReset) {
					var snapshot = RandomizedSnapshot (bounds);
					dataSource.ApplySnapshot (snapshot, false);
				}

				PerformSortStep ();
			});
		});
	}

	public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
	{
		base.ViewWillTransitionToSize (toSize, coordinator);

		var bounds = insertionCollectionView!.Bounds;
		var snapshot = RandomizedSnapshot (bounds);
		dataSource?.ApplySnapshot (snapshot, false);
	}

	NSDiffableDataSourceSnapshot<InsertionSortArray, InsertionSortArray.SortNode> RandomizedSnapshot (CGRect bounds)
	{
		var snapshot = new NSDiffableDataSourceSnapshot<InsertionSortArray, InsertionSortArray.SortNode> ();
		var rowCount = GetRows(bounds);
		var columnCount = GetColumns(bounds);

		for (int i = 0; i < rowCount; i++) {
			var section = new InsertionSortArray (columnCount);
			snapshot.AppendSections (new [] { section });
			snapshot.AppendItems (section.Nodes);
		}

		return snapshot;
	}

	static int GetRows (CGRect bounds) => (int)(bounds.Height / nodeSize.Height);
	static int GetColumns (CGRect bounds) => (int)(bounds.Width / nodeSize.Width);
}
