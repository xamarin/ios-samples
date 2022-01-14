/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Sample demonstrating UITableViewDiffableDataSource using editing, reordering and header/footer titles support
*/

namespace Conference_Diffable.Diffable;

public partial class TableViewEditingViewController : UIViewController {
	static readonly NSString key = new (nameof (TableViewEditingViewController));

	class Section : NSObject, IEquatable<Section> {
		readonly string description;

		public static Section Unknown { get; } = new Section (-1, nameof (Unknown));
		public static Section Visited { get; } = new Section (0, nameof (Visited));
		public static Section BucketList { get; } = new Section (1, "Bucket List");

		public int EnumValue { get; private set; }

		public static Section [] AllSections { get; } = {
			Visited, BucketList
		};

		Section (int enumValue, string description)
		{
			EnumValue = enumValue;
			this.description = description;
		}

		public static Section GetSectionKind (nint sectionIndex)
		{
			if (Visited.EnumValue == sectionIndex) return Visited;
			if (BucketList.EnumValue == sectionIndex) return BucketList;
			return Unknown;
		}

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
		public bool Equals (Section? other) => EnumValue == other?.EnumValue;
		public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), EnumValue);

		public override string ToString () => description;

		public string ToSecondaryString ()
		{
			if (Visited.EnumValue == EnumValue)
				return "Trips I've made!";
			if (BucketList.EnumValue == EnumValue)
				return "Need to do this before I go!";
			return "Unknown";
		}
	}

	// Subclassing our data source to supply various UITableViewDataSource methods
	class DataSource : UITableViewDiffableDataSource<Section, MountainsController.Mountain> {
		public DataSource (UITableView tableView, UITableViewDiffableDataSourceCellProvider cellProvider) : base (tableView, cellProvider)
		{
		}

		#region header/footer titles support

		public override string TitleForHeader (UITableView tableView, nint section)
			=> Section.GetSectionKind (section).ToString ();

		public override string TitleForFooter (UITableView tableView, nint section)
			=> Section.GetSectionKind (section).ToSecondaryString ();

		#endregion

		#region reordering support

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath) => true;

		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			if (sourceIndexPath == destinationIndexPath) return;

			var sourceId = GetItemIdentifier (sourceIndexPath);
			var destinationId = GetItemIdentifier (destinationIndexPath);

			var snapshot = Snapshot;

			if (destinationId is not null && sourceId is not null) {
				var sourceIndex = snapshot.GetIndex (sourceId);
				var destinationIndex = snapshot.GetIndex (destinationId);

				if (sourceIndex > -1 && destinationIndex > -1) {
					if (snapshot.GetSectionIdentifierForSection (sourceId) is null ||
						snapshot.GetSectionIdentifierForSection (destinationId) is null)
						throw new InvalidOperationException ("snapshot.GetSectionIdentifierForSection cannot use 'null' are argument");

					var isAfter = destinationIndex > sourceIndex &&
						snapshot.GetSectionIdentifierForSection (sourceId)! ==
						snapshot.GetSectionIdentifierForSection (destinationId)!;
					snapshot.DeleteItems (new [] { sourceId });

					if (isAfter)
						snapshot.InsertItemsAfter (new [] { sourceId }, destinationId);
					else
						snapshot.InsertItemsBefore (new [] { sourceId }, destinationId);
				}
			} else if (sourceId is not null) {
				var destinationSectionId = Snapshot.SectionIdentifiers [destinationIndexPath.Section];
				snapshot.DeleteItems (new [] { sourceId });
				snapshot.AppendItems (new [] { sourceId }, destinationSectionId);
			}

			ApplySnapshot (snapshot, false);
		}

		#endregion

		#region editing support

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath) => true;

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete) {
				var idToDelete = GetItemIdentifier (indexPath);

				if (idToDelete is not null) {
					var snapshot = Snapshot;
					snapshot.DeleteItems (new [] { idToDelete });
					ApplySnapshot (snapshot, true);
				}
			}
		}

		#endregion
	}

	UITableView? tableView;
	DataSource? dataSource;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.

		NavigationItem.Title = "UITableView: Editing";
		ConfigureHierarchy ();
		ConfigureDataSource ();
		ConfigureNavigationItem ();
	}

	void ConfigureHierarchy ()
	{
		if (View is null)
			throw new InvalidOperationException (nameof (View));

		tableView = new UITableView (CGRect.Empty, UITableViewStyle.InsetGrouped) {
			TranslatesAutoresizingMaskIntoConstraints = false
		};
		View.BackgroundColor = UIColor.SystemBackgroundColor;
		View.AddSubview (tableView);

		tableView.LeadingAnchor.ConstraintEqualTo (View.SafeAreaLayoutGuide.LeadingAnchor).Active = true;
		tableView.TrailingAnchor.ConstraintEqualTo (View.SafeAreaLayoutGuide.TrailingAnchor).Active = true;
		tableView.TopAnchor.ConstraintEqualTo (View.SafeAreaLayoutGuide.TopAnchor).Active = true;
		tableView.BottomAnchor.ConstraintEqualTo (View.SafeAreaLayoutGuide.BottomAnchor).Active = true;
	}

	void ConfigureDataSource ()
	{
		var formatter = new NSNumberFormatter
		{
			GroupingSize = 3,
			UsesGroupingSeparator = true
		};

		if (tableView is null)
			throw new InvalidOperationException (nameof (tableView));

		var snapshot = InitialSnapshot ();
		dataSource = new DataSource (tableView, CellProviderHandler);
		dataSource.ApplySnapshot (snapshot, false);

		UITableViewCell CellProviderHandler (UITableView tableView, NSIndexPath indexPath, NSObject obj)
		{
			if (obj is MountainsController.Mountain mountain)
			{
				// Get a cell of the desired kind.
				var cell = tableView.DequeueReusableCell (key) ?? new UITableViewCell (UITableViewCellStyle.Subtitle, key);

				var content = cell.DefaultContentConfiguration;
				content.Text = mountain.Name;
				content.SecondaryText = formatter.StringFromNumber (NSNumber.FromInt32 (mountain.Height));
				cell.ContentConfiguration = content;

				// Return the cell.
				return cell;
			}
			throw new InvalidOperationException ("MountainsController.Mountain");
		}
	}

	static NSDiffableDataSourceSnapshot<Section, MountainsController.Mountain> InitialSnapshot ()
	{
		var mountainsController = new MountainsController ();
		var limit = 8;
		var mountains = mountainsController.FilterMountains (limit: limit);
		var bucketList = mountains.Take (limit / 2).ToArray ();
		var visited = mountains.Skip (limit / 2).Take (limit / 2).ToArray ();

		var snapshot = new NSDiffableDataSourceSnapshot<Section, MountainsController.Mountain> ();
		snapshot.AppendSections (new [] { Section.Visited });
		snapshot.AppendItems (visited);
		snapshot.AppendSections (new [] { Section.BucketList });
		snapshot.AppendItems (bucketList);

		return snapshot;
	}

	void ConfigureNavigationItem ()
	{
		if (tableView is null)
			throw new InvalidOperationException (nameof (tableView));

		var editingItem = new UIBarButtonItem (tableView.Editing ? "Done" : "Edit", UIBarButtonItemStyle.Plain, ToggleEditing!);
		NavigationItem.RightBarButtonItem = editingItem;

		void ToggleEditing (object sender, EventArgs e)
		{
			tableView.SetEditing (!tableView.Editing, true);
			ConfigureNavigationItem ();
		}
	}
}
