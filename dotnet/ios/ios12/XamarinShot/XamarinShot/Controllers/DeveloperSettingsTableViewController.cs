

namespace XamarinShot;


/// <summary>
/// View controller for development & debugging settings.
/// </summary>
public partial class DeveloperSettingsTableViewController : UITableViewController, IUITextFieldDelegate
{
        readonly ProximityManager proximityManager = ProximityManager.Shared;

        List<UISwitch>? switches;

        public DeveloperSettingsTableViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();

                // set default delegates

                TableView.Delegate = this;
                TableView.DataSource = this;
                trailWidthTextField.Delegate = this;
                trailLengthTextField.Delegate = this;

                // initialize

                NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidBecomeActiveNotification, this.DidApplicationBecomeActive);
                antialiasingMode.On = UserDefaults.AntialiasingMode;
                useEncryptionSwitch.On = UserDefaults.UseEncryption;

                // if user turns of location permissions in settings after enabling them, we should turn off gameRoomMode
                if (!proximityManager.IsAuthorized)
                {
                        UserDefaults.GameRoomMode = false;
                }

                gameRoomModeSwitch.On = UserDefaults.GameRoomMode;
                useAutofocusSwitch.On = UserDefaults.AutoFocus;
                allowGameBoardAutoSizeSwitch.On = UserDefaults.AllowGameBoardAutoSize;

                // level
                showResetLevelSwitch.On = UserDefaults.ShowResetLever;
                showFlags.On = UserDefaults.ShowFlags;
                showClouds.On = UserDefaults.ShowClouds;
                showRopeSimulation.On = UserDefaults.ShowRopeSimulation;

                // happens here so the switches have been loaded from the storyboard
                switches = new List<UISwitch> {
                        showARDebugSwitch, showRenderStatsSwitch,
                        showTrackingStateSwitch, showWireframe, showLOD,
                        showPhysicsDebugSwitch, showSettingsSwitch, showARRelocalizationHelp,
                        showNetworkDebugSwitch, showThermalStateSwitch
                };

                ConfigureUISwitches ();
                ConfigureBoardLocationCells ();
                ConfigureProjectileTrail ();
        }

        void DidApplicationBecomeActive (NSNotification notification)
        {
                // check for permission changes after becoming active again
                if (!proximityManager.IsAuthorized)
                {
                        UserDefaults.GameRoomMode = false;
                }

                gameRoomModeSwitch.On = UserDefaults.GameRoomMode;
        }

        void ConfigureUISwitches ()
        {
                disableInGameUISwitch.On = UserDefaults.DisableInGameUI;
                showARDebugSwitch.On = UserDefaults.ShowARDebug;
                showRenderStatsSwitch.On = UserDefaults.ShowSceneViewStats;
                showTrackingStateSwitch.On = UserDefaults.ShowTrackingState;
                showWireframe.On = UserDefaults.ShowWireframe;
                showLOD.On = UserDefaults.ShowLOD;
                showPhysicsDebugSwitch.On = UserDefaults.ShowPhysicsDebug;
                showSettingsSwitch.On = UserDefaults.ShowSettingsInGame;
                showARRelocalizationHelp.On = UserDefaults.ShowARRelocalizationHelp;
                showNetworkDebugSwitch.On = UserDefaults.ShowNetworkDebug;
                showProjectileTrailSwitch.On = UserDefaults.ShowProjectileTrail;
                synchronizeMusicWithWallClockSwitch.On = UserDefaults.SynchronizeMusicWithWallClock;
                showThermalStateSwitch.On = UserDefaults.ShowThermalState;

                if (switches is not null)
                {
                        foreach (var @switch in switches)
                        {
                                @switch.Enabled = !UserDefaults.DisableInGameUI;
                        }
                }
        }

        void ConfigureBoardLocationCells ()
        {
                var boardLocationMode = UserDefaults.BoardLocatingMode;
                worldMapCell.Accessory = boardLocationMode == BoardLocatingMode.WorldMap ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                manualCell.Accessory = boardLocationMode == BoardLocatingMode.Manual ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
        }

        void ConfigureProjectileTrail ()
        {
                useCustomTrailSwitch.On = UserDefaults.UseCustomTrail;
                taperTrailSwitch.On = UserDefaults.TrailShouldNarrow;

                if (UserDefaults.UseCustomTrail)
                {
                        var width = UserDefaults.TrailWidth ?? TrailBallProjectile.DefaultTrailWidth;
                        trailWidthTextField.Text = width.ToString ();
                        trailWidthTextField.Enabled = true;
                        trailLengthTextField.Text = $"{UserDefaults.TrailLength ?? TrailBallProjectile.DefaultTrailLength}";
                        trailLengthTextField.Enabled = true;
                } else {
                        var defaultText = NSBundle.MainBundle.GetLocalizedString ("Default"); // when no custom selected
                        trailWidthTextField.Text = defaultText;
                        trailWidthTextField.Enabled = false;
                        trailLengthTextField.Text = defaultText;
                        trailLengthTextField.Enabled = false;
                }
        }

        partial void showARDebugChanged (UISwitch sender)
        {
                UserDefaults.ShowARDebug = sender.On;
        }

        partial void showRenderStatsChanged (UISwitch sender)
        {
                UserDefaults.ShowSceneViewStats = sender.On;
        }

        partial void showTrackingStateChanged (UISwitch sender)
        {
                UserDefaults.ShowTrackingState = sender.On;
        }

        partial void showPhysicsChanged (UISwitch sender)
        {
                UserDefaults.ShowPhysicsDebug = sender.On;
        }

        partial void showNetworkDebugChanged (UISwitch sender)
        {
                UserDefaults.ShowNetworkDebug = sender.On;
        }

        partial void showWireframeChanged (UISwitch sender)
        {
                UserDefaults.ShowWireframe = sender.On;
        }

        partial void antialiasingModeChanged (UISwitch sender)
        {
                UserDefaults.AntialiasingMode = sender.On;
        }

        partial void enableGameRoomModeChanged (UISwitch sender)
        {
                UserDefaults.GameRoomMode = sender.On;
                if (!proximityManager.IsAuthorized && sender.On)
                {
                        // if trying to enable beacons without location permissions, display alert
                        var alertController = UIAlertController.Create (NSBundle.MainBundle.GetLocalizedString ("Insufficient Location Permissions For Beacons"),  // "User didn't enable location services"
                                                                       NSBundle.MainBundle.GetLocalizedString ("Please go to Settings and enable location services for XamarinShot to look for nearby beacons"), // "Steps the user can take to activate beacon"
                                                                       UIAlertControllerStyle.Alert);
                        alertController.AddAction (UIAlertAction.Create (NSBundle.MainBundle.GetLocalizedString ("Dismiss"),
                                                                       UIAlertActionStyle.Default,
                                                                       null));
                        PresentViewController (alertController, true, null);
                        UserDefaults.GameRoomMode = false;
                        gameRoomModeSwitch.SetState (false, true);
                }
        }

        partial void useEncryptionChanged (UISwitch sender)
        {
                UserDefaults.UseEncryption = sender.On;
        }

        partial void showSettingsChanged (UISwitch sender)
        {
                UserDefaults.ShowSettingsInGame = sender.On;
        }

        partial void showARRelocalizationHelpChanged (UISwitch sender)
        {
                UserDefaults.ShowARRelocalizationHelp = sender.On;
        }

        partial void showResetSwitch (UISwitch sender)
        {
                UserDefaults.ShowResetLever = sender.On;
        }

        partial void showCloudsChanged (UISwitch sender)
        {
                UserDefaults.ShowClouds = sender.On;
        }

        partial void showFlagsChanged (UISwitch sender)
        {
                UserDefaults.ShowFlags = sender.On;
        }

        partial void showRopeSimulationChanged (UISwitch sender)
        {
                UserDefaults.ShowRopeSimulation = sender.On;
        }

        partial void showLODChanged (UISwitch sender)
        {
                UserDefaults.ShowLOD = sender.On;
        }

        partial void useAutofocus (UISwitch sender)
        {
                UserDefaults.AutoFocus = sender.On;
        }

        partial void disableInGameUIChanged (UISwitch sender)
        {
                UserDefaults.DisableInGameUI = sender.On;
                if (sender.On)
                {
                        // also turn off everything else
                        UserDefaults.ShowARDebug = false;
                        UserDefaults.ShowPhysicsDebug = false;
                        UserDefaults.ShowARDebug = false;
                        UserDefaults.ShowWireframe = false;
                        UserDefaults.ShowSceneViewStats = false;
                        UserDefaults.ShowTrackingState = false;
                        UserDefaults.ShowSettingsInGame = false;
                        UserDefaults.ShowARRelocalizationHelp = false;
                        UserDefaults.ShowNetworkDebug = false;
                        UserDefaults.ShowLOD = false;
                }

                ConfigureUISwitches ();
        }

        partial void synchronizeMusicWithWallClockChanged (UISwitch sender)
        {
                UserDefaults.SynchronizeMusicWithWallClock = sender.On;
        }

        #region projectile trail

        partial void allowGameBoardAutoSizeChanged (UISwitch sender)
        {
                UserDefaults.AllowGameBoardAutoSize = sender.On;
        }

        partial void showProjectileTrailChanged (UISwitch sender)
        {
                UserDefaults.ShowProjectileTrail = sender.On;
        }

        partial void useCustomTrailChanged (UISwitch sender)
        {
                UserDefaults.UseCustomTrail = sender.On;
                ConfigureProjectileTrail ();
        }

        partial void taperTrailChanged (UISwitch sender)
        {
                UserDefaults.TrailShouldNarrow = sender.On;
                ConfigureProjectileTrail ();
        }

        partial void showThermalStateChanged (UISwitch sender)
        {
                UserDefaults.ShowThermalState = sender.On;
        }

        #endregion

        #region table delegate

        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
                TableView.DeselectRow (indexPath, true);
                var cell = TableView.CellAt (indexPath);
                if (cell is not null)
                {
                        if (cell == worldMapCell)
                        {
                                UserDefaults.BoardLocatingMode = BoardLocatingMode.WorldMap;
                        }
                        else if (cell == manualCell)
                        {
                                UserDefaults.BoardLocatingMode = BoardLocatingMode.Manual;
                        }

                        ConfigureBoardLocationCells ();
                }
        }

        #endregion

        #region IUITextFieldDelegate

        [Export ("textFieldShouldReturn:")]
        public bool ShouldReturn (UITextField textField)
        {
                textField.ResignFirstResponder ();
                return true;
        }

        [Export ("textFieldDidEndEditing:reason:")]
        public void EditingEnded (UITextField textField, UITextFieldDidEndEditingReason reason)
        {
                if (textField == trailWidthTextField)
                {
                        TrailWidthDidEndEditing (reason);
                }
                else if (textField == trailLengthTextField)
                {
                        TrailLengthDidEndEditing (reason);
                }
        }

        private void TrailWidthDidEndEditing (UITextFieldDidEndEditingReason reason)
        {
                if (!string.IsNullOrEmpty (trailWidthTextField.Text) &&
                        float.TryParse (trailWidthTextField.Text, out float newValue))
                {
                        UserDefaults.TrailWidth = newValue; // value stored in unit ball size (1.0 as trail width equal to ball size)
                } else {
                        UserDefaults.TrailWidth = null;
                }

                ConfigureProjectileTrail ();
        }

        private void TrailLengthDidEndEditing (UITextFieldDidEndEditingReason reason)
        {
                if (!string.IsNullOrEmpty (trailLengthTextField.Text) &&
                        int.TryParse (trailLengthTextField.Text, out int newValue))
                {
                        UserDefaults.TrailLength = newValue;
                } else {
                        UserDefaults.TrailLength = null;
                }

                ConfigureProjectileTrail ();
        }

        #endregion
}
