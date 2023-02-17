
namespace XamarinShot {
	using Foundation;
	using XamarinShot.Models;
	using XamarinShot.Models.Enums;
	using XamarinShot.Utils;
	using System;
	using UIKit;

	public interface IGameStartViewControllerDelegate {
		void OnSoloGamePressed (UIViewController controller, UIButton button);
		void OnGameSelected (UIViewController controller, NetworkSession game);
		void OnGameStarted (UIViewController controller, NetworkSession game);
		void OnSettingsSelected (UIViewController controller);
	}

	/// <summary>
	/// View controller for in-game 2D overlay UI.
	/// </summary>
	public partial class GameStartViewController : UIViewController, IProximityManagerDelegate {
		private readonly Player myself = UserDefaults.Myself;

		private readonly ProximityManager proximityManager = ProximityManager.Shared;

		private GameBrowser gameBrowser;

		private ButtonBeep backButtonBeep;

		private ButtonBeep buttonBeep;

		public GameStartViewController (IntPtr handle) : base (handle) { }

		public IGameStartViewControllerDelegate Delegate { get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.proximityManager.Delegate = this;
			this.hostButton.ClipsToBounds = true;
			this.hostButton.Layer.CornerRadius = 30f;

			this.joinButton.ClipsToBounds = true;
			this.joinButton.Layer.CornerRadius = 30f;

			this.buttonBeep = ButtonBeep.Create ("button_forward.wav", 0.5f);
			this.backButtonBeep = ButtonBeep.Create ("button_backward.wav", 0.5f);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (UserDefaults.GameRoomMode) {
				//os_log(.debug, "Will start beacon ranging")
				this.proximityManager.Start ();
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (!string.IsNullOrEmpty (segue.Identifier) &&
			   Enum.TryParse<GameSegue> (segue.Identifier, true, out var segueType)) {
				switch (segueType) {
				case GameSegue.EmbeddedGameBrowser:
					if (segue.DestinationViewController is NetworkGameBrowserViewController browser) {
						this.gameBrowser = new GameBrowser (this.myself);
						browser.Browser = this.gameBrowser;
						browser.ProximityManager = this.proximityManager;
					}

					break;
				}
			}
		}

		public void JoinGame (NetworkSession session)
		{
			this.Delegate?.OnGameSelected (this, session);
			this.SetupOverlayVC ();
		}

		partial void startSoloGamePressed (UIButton sender)
		{
			this.Delegate?.OnSoloGamePressed (this, sender);
		}

		partial void startGamePressed (UIButton sender)
		{
			this.buttonBeep.Play ();
			this.StartGame (this.myself);
		}

		partial void settingsPressed (UIButton sender)
		{
			this.Delegate?.OnSettingsSelected (this);
		}

		partial void joinButtonPressed (UIButton sender)
		{
			this.buttonBeep.Play ();
			this.ShowViews (false);
		}

		partial void backButtonPressed (UIButton sender)
		{
			this.backButtonBeep.Play ();
			this.SetupOverlayVC ();
		}

		private void SetupOverlayVC ()
		{
			this.ShowViews (true);
		}

		private void ShowViews (bool forSetup)
		{
			UIView.Transition (this.View, 1d, UIViewAnimationOptions.TransitionCrossDissolve, () => {
				this.blurView.Hidden = forSetup;
				this.browserContainerView.Hidden = forSetup;
				this.backButton.Hidden = forSetup;
				this.nearbyGamesLabel.Hidden = forSetup;

				this.joinButton.Hidden = !forSetup;
				this.hostButton.Hidden = !forSetup;
			}, null);
		}

		private void StartGame (Player player)
		{
			GameTableLocation location = null;
			if (UserDefaults.GameRoomMode) {
				location = this.proximityManager.ClosestLocation;
			}

			var gameSession = new NetworkSession (player, true, location, this.myself);
			this.Delegate?.OnGameStarted (this, gameSession);
			this.SetupOverlayVC ();
		}

		#region IProximityManagerDelegate

		public void LocationChanged (ProximityManager manager, GameTableLocation location)
		{
			this.gameBrowser?.Refresh ();
		}

		public void AuthorizationChanged (ProximityManager manager, bool authorization) { }

		#endregion
	}
}
