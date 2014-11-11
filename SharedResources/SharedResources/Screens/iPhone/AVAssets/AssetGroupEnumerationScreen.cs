using System;
using UIKit;
using AssetsLibrary;
using Foundation;
using System.Collections.Generic;

namespace Example_SharedResources.Screens.iPhone.AVAssets
{
	public class AssetGroupEnumerationScreen : UITableViewController
	{
		// declare vars
		protected ALAssetsLibrary assetsLibrary;
		protected AssetGroupTableSource assetGroupTableSource;
		protected List<ALAssetsGroup> groups = new List<ALAssetsGroup> ();
		protected Dictionary<ALAssetsGroup, List<ALAsset>> assetGroups = new Dictionary<ALAssetsGroup, List<ALAsset>> ();
		protected ALAssetsGroup currentGroup; //used to track the enumeration

		public AssetGroupEnumerationScreen () { }
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "All Asset Groups";
			
			// instantiate a reference to the shared assets library
			assetsLibrary = new ALAssetsLibrary();
			// enumerate the photo albums
			assetsLibrary.Enumerate(ALAssetsGroupType.All, GroupsEnumerator,
						(NSError e) => { Console.WriteLine ("Could not enumerate albums: " + e.LocalizedDescription); });
			
		}
		
		// Called for each group that is enumerated
		protected void GroupsEnumerator (ALAssetsGroup group, ref bool stop)
		{
		    // when the enumeration is completed, this method is invoked with group set to null
			if (group != null) {
				Console.WriteLine ("Group found: " + group.Type.ToString ());
				
				// don't stop, baby
				stop = false;
		
				// photos and videos. could also pass AllVideos, AllVideos, etc.
				group.SetAssetsFilter (ALAssetsFilter.AllAssets);
		
				if (group.Name != null) 
					Console.WriteLine("Group Name: " + group.Name);
				
				// add the group to the assets dictionary
				groups.Add(group);
				assetGroups.Add(group, new List<ALAsset> ());
				currentGroup = group;
				
				// enumerate each asset within the group
				group.Enumerate(AssetEnumerator);
		    } else {
				Console.WriteLine ("Group enumeration completed.");
				
				assetGroupTableSource = new AssetGroupTableSource (groups);
				TableView.Source = assetGroupTableSource;
			
				assetGroupTableSource.GroupSelected += (object sender, AssetGroupTableSource.GroupSelectedEventArgs e) => {
					AssetEnumerationScreen assetScreen = new AssetEnumerationScreen (e.Group.Name, assetGroups[e.Group]);
					NavigationController.PushViewController (assetScreen, true);
				};

				TableView.ReloadData ();
			}
		}
		
		/// <summary>
		/// A simple asset enumerator that adds the asset to our asset list
		/// </summary>
		protected void AssetEnumerator (ALAsset asset, int index, ref bool stop)
		{
		    // when the enumeration is completed, this method is invoked with group set to null
			if(asset != null) {
				Console.WriteLine ("Found asset: " + index.ToString ());
				
				// add the asset to the group list
				assetGroups[currentGroup].Add (asset);
				
				// keep going
				stop = false;
				
				//Console.WriteLine(asset.AssetType.ToString());
			}
			else
				Console.WriteLine("Asset enumeration completed.");
		}
		
		/// <summary>
		/// Simple data source to display the asset groups
		/// </summary>
		protected class AssetGroupTableSource : UITableViewSource
		{
			public event EventHandler<GroupSelectedEventArgs> GroupSelected;
			protected List<ALAssetsGroup> groups;
			
			public AssetGroupTableSource(List<ALAssetsGroup> groups) { this.groups = groups; }
			
			public override nint NumberOfSections (UITableView tableView) { return 1; }
			
			public override nint RowsInSection (UITableView tableview, nint section) { return groups.Count; }
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("AlbumCell");
				if(cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Default, "AlbumCell");
				cell.TextLabel.Text = groups[indexPath.Row].Name;
				return cell;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// raise our event
				var handler = GroupSelected;
				if (handler != null)
					handler (this, new GroupSelectedEventArgs (groups[indexPath.Row]));
			}
		
			public class GroupSelectedEventArgs : EventArgs
			{
				public ALAssetsGroup Group { get; set; }
				public GroupSelectedEventArgs(ALAssetsGroup group) : base()
				{ Group = group; }
			}
		}
	}
}

