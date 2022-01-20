namespace XamarinShot;


public interface IWorldMapSelectorDelegate
{
void WorldMapSelector(WorldMapSelectorViewController worldMapSelector, Uri selectedMap);
}

/// <summary>
/// Allows the user to load a pre-saved map of the physical world for ARKit. The app then uses this to place the board in real space.
/// </summary>
public partial class WorldMapSelectorViewController : UITableViewController
{
        List<NSUrl> maps = new List<NSUrl> ();

        public WorldMapSelectorViewController (IntPtr handle) : base (handle) { }

        public IWorldMapSelectorDelegate? Delegate { get; set; }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();

                var docs = NSFileManager.DefaultManager.GetUrl (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, null, true, out NSError error);
                if (error is not null)
                {
                        Console.WriteLine ($"error loading world maps directory: {error.LocalizedDescription ?? string.Empty}");
                        DismissViewController (true, null);
                }

                var mapsDirectory = docs.Append ("maps", isDirectory: true);
                var localMaps = NSFileManager.DefaultManager.GetDirectoryContent (mapsDirectory, null, default (NSDirectoryEnumerationOptions), out NSError _);
                maps = localMaps.Select (map => map.AbsoluteUrl).ToList ();
        }

        #region IUITableViewDelegate

        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
                Delegate?.WorldMapSelector (this, maps [indexPath.Row]);
                DismissViewController (true, null);
        }

        #endregion

        #region IUITableViewDataSource

        public override nint RowsInSection (UITableView tableView, nint section)
        {
                return maps.Count;
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
                var cell = tableView.DequeueReusableCell ("MapCell", indexPath);
                cell.TextLabel.Text = maps [indexPath.Row].RemovePathExtension ().LastPathComponent;
                return cell;
        }

        #endregion
}
