
namespace XamarinShot {
	using Foundation;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;

	public interface IWorldMapSelectorDelegate {
		void WorldMapSelector (WorldMapSelectorViewController worldMapSelector, Uri selectedMap);
	}

	/// <summary>
	/// Allows the user to load a pre-saved map of the physical world for ARKit. The app then uses this to place the board in real space.
	/// </summary>
	public partial class WorldMapSelectorViewController : UITableViewController {
		private List<NSUrl> maps = new List<NSUrl> ();

		public WorldMapSelectorViewController (IntPtr handle) : base (handle) { }

		public IWorldMapSelectorDelegate Delegate { get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var docs = NSFileManager.DefaultManager.GetUrl (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, null, true, out NSError error);
			if (error != null) {
				Console.WriteLine ($"error loading world maps directory: {error.LocalizedDescription ?? string.Empty}");
				this.DismissViewController (true, null);
			}

			var mapsDirectory = docs.Append ("maps", isDirectory: true);
			var localMaps = NSFileManager.DefaultManager.GetDirectoryContent (mapsDirectory, null, default (NSDirectoryEnumerationOptions), out NSError _);
			this.maps = localMaps.Select (map => map.AbsoluteUrl).ToList ();
		}

		#region IUITableViewDelegate

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			this.Delegate?.WorldMapSelector (this, this.maps [indexPath.Row]);
			this.DismissViewController (true, null);
		}

		#endregion

		#region IUITableViewDataSource

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return this.maps.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("MapCell", indexPath);
			cell.TextLabel.Text = this.maps [indexPath.Row].RemovePathExtension ().LastPathComponent;
			return cell;
		}

		#endregion
	}
}
