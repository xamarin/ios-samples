using System;
using MonoTouch.UIKit;
using MonoTouch.AssetsLibrary;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace Example_SharedResources.Screens.iPhone.AVAssets
{
	public class AssetEnumerationScreen : UITableViewController
	{
		// declare vars
		string groupName = string.Empty;
		List<ALAsset> assets = null;
		AssetsDataSource dataSource = null;
				
		public AssetEnumerationScreen (string groupName, List<ALAsset> assets)
		{
			this.groupName = groupName;
			this.assets = assets;
		}
		
				
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set the title
			Title = groupName;
			
			// create our table source
			dataSource = new AssetsDataSource(assets);
			TableView.Source = dataSource;
		}


		/// <summary>
		/// Simple data source to display the assets
		/// </summary>
		protected class AssetsDataSource : UITableViewSource
		{
			protected List<ALAsset> items;
			
			public AssetsDataSource (List<ALAsset> items) { this.items = items; }
			
			public override int NumberOfSections (UITableView tableView) { return 1; }
			
			public override int RowsInSection (UITableView tableview, int section) 
			{ 
				return items.Count; 
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("AssetCell");
				if(cell == null) 
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, "AssetCell");
				
				// set the text
				cell.TextLabel.Text = items[indexPath.Row].AssetType.ToString ();
				cell.DetailTextLabel.Text = items[indexPath.Row].Date.ToString ();
				// set the image
				if(items[indexPath.Row].Thumbnail != null)
					cell.ImageView.Image = UIImage.FromImage(items[indexPath.Row].Thumbnail);
				
				return cell;
			}
		}
		
	}
}

