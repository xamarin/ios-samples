
namespace XamarinShot {
	using Foundation;
	using XamarinShot.Models;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;

	/// <summary>
	/// View controller for finding network games.
	/// </summary>
	public partial class NetworkGameBrowserViewController : UIViewController,
															IUITableViewDelegate,
															IUITableViewDataSource,
															IGameBrowserDelegate {
		private List<NetworkGame> games = new List<NetworkGame> ();

		private GameBrowser browser;

		public NetworkGameBrowserViewController (IntPtr handle) : base (handle) { }

		// must be set by parent
		public ProximityManager ProximityManager { get; set; }

		/// <summary>
		/// Must be set by parent
		/// </summary>
		/// <value>The browser.</value>
		public GameBrowser Browser {
			get {
				return this.browser;
			}

			set {
				if (this.browser != null) {
					this.browser.Stop ();
				}

				this.browser = value;
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.tableView.DataSource = this;
			this.tableView.Delegate = this;
			this.tableView.Layer.CornerRadius = 10;
			this.tableView.ClipsToBounds = true;
			this.StartBrowser ();
		}

		private void StartBrowser ()
		{
			if (this.browser != null) {
				this.browser.Delegate = this;
				this.browser.Start ();

				this.tableView.ReloadData ();
			}
		}

		private void JoinGame (NetworkGame game)
		{
			var session = this.browser?.Join (game);
			if (session != null) {
				if (this.ParentViewController is GameStartViewController parent) {
					parent.JoinGame (session);
				} else {
					throw new ArgumentException ("Unexpected parent", nameof (this.ParentViewController));
				}
			}
		}

		#region IGameBrowserDelegate

		public void SawGames (GameBrowser browser, IList<NetworkGame> games)
		{
			// os_log(.info, "saw %d games!", games.count)
			var location = this.ProximityManager?.ClosestLocation;
			if (location != null && UserDefaults.GameRoomMode) {
				this.games = games.Where (game => game.Location == location).ToList ();
			} else {
				this.games = games.ToList ();
			}

			this.tableView.ReloadData ();
		}

		#endregion

		#region UITableViewDataSource

		public nint RowsInSection (UITableView tableView, nint section)
		{
			return this.games.Count;
		}

		[Export ("numberOfSectionsInTableView:")]
		public nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var game = this.games [indexPath.Row];

			var cell = tableView.DequeueReusableCell ("GameCell", indexPath);
			cell.TextLabel.Text = game.Name;
			return cell;
		}

		#endregion

		#region IUITableViewDelegate

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			this.tableView.DeselectRow (indexPath, true);
			var otherPlayer = this.games [indexPath.Row];
			this.JoinGame (otherPlayer);
		}

		#endregion
	}
}
