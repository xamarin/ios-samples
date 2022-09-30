namespace XamarinShot;


/// <summary>
/// View controller for finding network games.
/// </summary>
public partial class NetworkGameBrowserViewController : UIViewController,
                                                    IUITableViewDelegate,
                                                    IUITableViewDataSource,
                                                    IGameBrowserDelegate
{
        List<NetworkGame> games = new List<NetworkGame> ();

        GameBrowser? browser;

        public NetworkGameBrowserViewController (IntPtr handle) : base (handle) { }

        // must be set by parent
        public ProximityManager? ProximityManager { get; set; }

        /// <summary>
        /// Must be set by parent
        /// </summary>
        /// <value>The browser.</value>
        public GameBrowser? Browser
        {
                get
                {
                        return browser;
                }

                set
                {
                        browser?.Stop ();

                        browser = value;
                }
        }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();

                tableView.DataSource = this;
                tableView.Delegate = this;
                tableView.Layer.CornerRadius = 10;
                tableView.ClipsToBounds = true;
                StartBrowser ();
        }

        void StartBrowser ()
        {
                if (browser is not null)
                {
                        browser.Delegate = this;
                        browser.Start ();

                        tableView.ReloadData ();
                }
        }

        void JoinGame (NetworkGame game)
        {
                var session = browser?.Join (game);
                if (session is not null)
                {
                        if (ParentViewController is GameStartViewController parent)
                        {
                                parent.JoinGame (session);
                        }
                        else
                        {
                                throw new ArgumentException ("Unexpected parent", nameof (ParentViewController));
                        }
                }
        }

        #region IGameBrowserDelegate

        public void SawGames (GameBrowser browser, IList<NetworkGame> games)
        {
                // os_log(.info, "saw %d games!", games.count)
                var location = ProximityManager?.ClosestLocation;
                if (location is not null && UserDefaults.GameRoomMode)
                {
                        games = games.Where (game => game.Location == location).ToList ();
                }
                else
                {
                        games = games.ToList ();
                }

                tableView.ReloadData ();
        }

        #endregion

        #region UITableViewDataSource

        public nint RowsInSection (UITableView tableView, nint section)
        {
                return games.Count;
        }

        [Export ("numberOfSectionsInTableView:")]
        public nint NumberOfSections (UITableView tableView)
        {
                return 1;
        }

        public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
                var game = games [indexPath.Row];

                var cell = tableView.DequeueReusableCell ("GameCell", indexPath);
                cell.TextLabel.Text = game.Name;
                return cell;
        }

        #endregion

        #region IUITableViewDelegate

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
                tableView.DeselectRow (indexPath, true);
                var otherPlayer = games [indexPath.Row];
                JoinGame (otherPlayer);
        }

        #endregion
}
