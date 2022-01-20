namespace XamarinShot;

public interface IGameStartViewControllerDelegate
{
        void OnSoloGamePressed (UIViewController controller, UIButton button);
        void OnGameSelected (UIViewController controller, NetworkSession game);
        void OnGameStarted (UIViewController controller, NetworkSession game);
        void OnSettingsSelected (UIViewController controller);
}

/// <summary>
/// View controller for in-game 2D overlay UI.
/// </summary>
public partial class GameStartViewController : UIViewController, IProximityManagerDelegate
{
        private readonly Player myself = UserDefaults.Myself;

        private readonly ProximityManager proximityManager = ProximityManager.Shared;

        private GameBrowser? gameBrowser;

        private ButtonBeep? backButtonBeep;

        private ButtonBeep? buttonBeep;

        public GameStartViewController (IntPtr handle) : base (handle) { }

        public IGameStartViewControllerDelegate? Delegate { get; set; }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();

                proximityManager.Delegate = this;
                hostButton.ClipsToBounds = true;
                hostButton.Layer.CornerRadius = 30f;

                joinButton.ClipsToBounds = true;
                joinButton.Layer.CornerRadius = 30f;

                buttonBeep = ButtonBeep.Create ("button_forward.wav", 0.5f);
                backButtonBeep = ButtonBeep.Create ("button_backward.wav", 0.5f);
        }

        public override void ViewDidAppear (bool animated)
        {
                base.ViewDidAppear (animated);

                if (UserDefaults.GameRoomMode)
                {
                        //os_log(.debug, "Will start beacon ranging")
                        proximityManager.Start ();
                }
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject? sender)
        {
                if (!string.IsNullOrEmpty (segue.Identifier) &&
                        Enum.TryParse<GameSegue> (segue.Identifier, true, out var segueType))
                {
                        switch (segueType)
                        {
                                case GameSegue.EmbeddedGameBrowser:
                                        if (segue.DestinationViewController is NetworkGameBrowserViewController browser)
                                        {
                                                gameBrowser = new GameBrowser (myself);
                                                browser.Browser = gameBrowser;
                                                browser.ProximityManager = proximityManager;
                                        }
                                        break;
                        }
                }
        }

        public void JoinGame (NetworkSession session)
        {
                Delegate?.OnGameSelected (this, session);
                SetupOverlayVC ();
        }

        partial void startSoloGamePressed (UIButton sender)
        {
                Delegate?.OnSoloGamePressed (this, sender);
        }

        partial void startGamePressed (UIButton sender)
        {
                buttonBeep?.Play ();
                StartGame (myself);
        }

        partial void settingsPressed (UIButton sender)
        {
                Delegate?.OnSettingsSelected (this);
        }

        partial void joinButtonPressed (UIButton sender)
        {
                buttonBeep?.Play ();
                ShowViews (false);
        }

        partial void backButtonPressed (UIButton sender)
        {
                backButtonBeep?.Play ();
                SetupOverlayVC ();
        }

        private void SetupOverlayVC ()
        {
                ShowViews (true);
        }

        private void ShowViews (bool forSetup)
        {
                UIView.Transition (View, 1d, UIViewAnimationOptions.TransitionCrossDissolve, () =>
                {
                        blurView.Hidden = forSetup;
                        browserContainerView.Hidden = forSetup;
                        backButton.Hidden = forSetup;
                        nearbyGamesLabel.Hidden = forSetup;

                        joinButton.Hidden = !forSetup;
                        hostButton.Hidden = !forSetup;
                }, null);
        }

        private void StartGame (Player player)
        {
                GameTableLocation? location = null;
                if (UserDefaults.GameRoomMode)
                {
                        location = proximityManager.ClosestLocation;
                }

                var gameSession = new NetworkSession (player, true, location, myself);
                Delegate?.OnGameStarted (this, gameSession);
                SetupOverlayVC ();
        }

        #region IProximityManagerDelegate

        public void LocationChanged (ProximityManager manager, GameTableLocation location)
        {
                gameBrowser?.Refresh ();
        }

        public void AuthorizationChanged (ProximityManager manager, bool authorization) { }

        #endregion
}
