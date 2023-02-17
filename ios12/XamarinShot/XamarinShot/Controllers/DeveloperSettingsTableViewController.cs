
namespace XamarinShot {
	using Foundation;
	using XamarinShot.Models;
	using XamarinShot.Models.Enums;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;
	using UIKit;

	/// <summary>
	/// View controller for development & debugging settings.
	/// </summary>
	public partial class DeveloperSettingsTableViewController : UITableViewController, IUITextFieldDelegate {
		private readonly ProximityManager proximityManager = ProximityManager.Shared;

		private List<UISwitch> switches;

		public DeveloperSettingsTableViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// set default delegates

			this.TableView.Delegate = this;
			this.TableView.DataSource = this;
			this.trailWidthTextField.Delegate = this;
			this.trailLengthTextField.Delegate = this;

			// initialize

			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidBecomeActiveNotification, this.DidApplicationBecomeActive);
			this.antialiasingMode.On = UserDefaults.AntialiasingMode;
			this.useEncryptionSwitch.On = UserDefaults.UseEncryption;

			// if user turns of location permissions in settings after enabling them, we should turn off gameRoomMode
			if (!this.proximityManager.IsAuthorized) {
				UserDefaults.GameRoomMode = false;
			}

			this.gameRoomModeSwitch.On = UserDefaults.GameRoomMode;
			this.useAutofocusSwitch.On = UserDefaults.AutoFocus;
			this.allowGameBoardAutoSizeSwitch.On = UserDefaults.AllowGameBoardAutoSize;

			// level
			this.showResetLevelSwitch.On = UserDefaults.ShowResetLever;
			this.showFlags.On = UserDefaults.ShowFlags;
			this.showClouds.On = UserDefaults.ShowClouds;
			this.showRopeSimulation.On = UserDefaults.ShowRopeSimulation;

			// happens here so the switches have been loaded from the storyboard
			this.switches = new List<UISwitch>
			{
				this.showARDebugSwitch, this.showRenderStatsSwitch,
				this.showTrackingStateSwitch, this.showWireframe, this.showLOD,
				this.showPhysicsDebugSwitch, this.showSettingsSwitch, this.showARRelocalizationHelp,
				this.showNetworkDebugSwitch, this.showThermalStateSwitch
			};

			this.ConfigureUISwitches ();
			this.ConfigureBoardLocationCells ();
			this.ConfigureProjectileTrail ();
		}

		private void DidApplicationBecomeActive (NSNotification notification)
		{
			// check for permission changes after becoming active again
			if (!this.proximityManager.IsAuthorized) {
				UserDefaults.GameRoomMode = false;
			}

			this.gameRoomModeSwitch.On = UserDefaults.GameRoomMode;
		}

		private void ConfigureUISwitches ()
		{
			this.disableInGameUISwitch.On = UserDefaults.DisableInGameUI;
			this.showARDebugSwitch.On = UserDefaults.ShowARDebug;
			this.showRenderStatsSwitch.On = UserDefaults.ShowSceneViewStats;
			this.showTrackingStateSwitch.On = UserDefaults.ShowTrackingState;
			this.showWireframe.On = UserDefaults.ShowWireframe;
			this.showLOD.On = UserDefaults.ShowLOD;
			this.showPhysicsDebugSwitch.On = UserDefaults.ShowPhysicsDebug;
			this.showSettingsSwitch.On = UserDefaults.ShowSettingsInGame;
			this.showARRelocalizationHelp.On = UserDefaults.ShowARRelocalizationHelp;
			this.showNetworkDebugSwitch.On = UserDefaults.ShowNetworkDebug;
			this.showProjectileTrailSwitch.On = UserDefaults.ShowProjectileTrail;
			this.synchronizeMusicWithWallClockSwitch.On = UserDefaults.SynchronizeMusicWithWallClock;
			this.showThermalStateSwitch.On = UserDefaults.ShowThermalState;

			foreach (var @switch in this.switches) {
				@switch.Enabled = !UserDefaults.DisableInGameUI;
			}
		}

		private void ConfigureBoardLocationCells ()
		{
			var boardLocationMode = UserDefaults.BoardLocatingMode;
			this.worldMapCell.Accessory = boardLocationMode == BoardLocatingMode.WorldMap ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			this.manualCell.Accessory = boardLocationMode == BoardLocatingMode.Manual ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
		}

		private void ConfigureProjectileTrail ()
		{
			this.useCustomTrailSwitch.On = UserDefaults.UseCustomTrail;
			this.taperTrailSwitch.On = UserDefaults.TrailShouldNarrow;

			if (UserDefaults.UseCustomTrail) {
				var width = UserDefaults.TrailWidth ?? TrailBallProjectile.DefaultTrailWidth;
				this.trailWidthTextField.Text = width.ToString ();
				this.trailWidthTextField.Enabled = true;
				this.trailLengthTextField.Text = $"{UserDefaults.TrailLength ?? TrailBallProjectile.DefaultTrailLength}";
				this.trailLengthTextField.Enabled = true;
			} else {
				var defaultText = NSBundle.MainBundle.GetLocalizedString ("Default"); // when no custom selected
				this.trailWidthTextField.Text = defaultText;
				this.trailWidthTextField.Enabled = false;
				this.trailLengthTextField.Text = defaultText;
				this.trailLengthTextField.Enabled = false;
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
			if (!this.proximityManager.IsAuthorized && sender.On) {
				// if trying to enable beacons without location permissions, display alert
				var alertController = UIAlertController.Create (NSBundle.MainBundle.GetLocalizedString ("Insufficient Location Permissions For Beacons"),  // "User didn't enable location services"
															   NSBundle.MainBundle.GetLocalizedString ("Please go to Settings and enable location services for XamarinShot to look for nearby beacons"), // "Steps the user can take to activate beacon"
															   UIAlertControllerStyle.Alert);
				alertController.AddAction (UIAlertAction.Create (NSBundle.MainBundle.GetLocalizedString ("Dismiss"),
															   UIAlertActionStyle.Default,
															   null));
				this.PresentViewController (alertController, true, null);
				UserDefaults.GameRoomMode = false;
				this.gameRoomModeSwitch.SetState (false, true);
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
			if (sender.On) {
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

			this.ConfigureUISwitches ();
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
			this.ConfigureProjectileTrail ();
		}

		partial void taperTrailChanged (UISwitch sender)
		{
			UserDefaults.TrailShouldNarrow = sender.On;
			this.ConfigureProjectileTrail ();
		}

		partial void showThermalStateChanged (UISwitch sender)
		{
			UserDefaults.ShowThermalState = sender.On;
		}

		#endregion

		#region table delegate

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			this.TableView.DeselectRow (indexPath, true);
			var cell = this.TableView.CellAt (indexPath);
			if (cell != null) {
				if (cell == this.worldMapCell) {
					UserDefaults.BoardLocatingMode = BoardLocatingMode.WorldMap;
				} else if (cell == this.manualCell) {
					UserDefaults.BoardLocatingMode = BoardLocatingMode.Manual;
				}

				this.ConfigureBoardLocationCells ();
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
			if (textField == this.trailWidthTextField) {
				this.TrailWidthDidEndEditing (reason);
			} else if (textField == this.trailLengthTextField) {
				this.TrailLengthDidEndEditing (reason);
			}
		}

		private void TrailWidthDidEndEditing (UITextFieldDidEndEditingReason reason)
		{
			if (!string.IsNullOrEmpty (this.trailWidthTextField.Text) &&
			   float.TryParse (this.trailWidthTextField.Text, out float newValue)) {
				UserDefaults.TrailWidth = newValue; // value stored in unit ball size (1.0 as trail width equal to ball size)
			} else {
				UserDefaults.TrailWidth = null;
			}

			this.ConfigureProjectileTrail ();
		}

		private void TrailLengthDidEndEditing (UITextFieldDidEndEditingReason reason)
		{
			if (!string.IsNullOrEmpty (this.trailLengthTextField.Text) &&
			   int.TryParse (this.trailLengthTextField.Text, out int newValue)) {
				UserDefaults.TrailLength = newValue;
			} else {
				UserDefaults.TrailLength = null;
			}

			this.ConfigureProjectileTrail ();
		}

		#endregion
	}
}
