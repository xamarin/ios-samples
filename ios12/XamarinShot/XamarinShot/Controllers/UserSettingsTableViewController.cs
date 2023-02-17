
namespace XamarinShot {
	using Foundation;
	using XamarinShot.Models;
	using XamarinShot.Models.Enums;
	using XamarinShot.Utils;
	using System;
	using UIKit;

	/// <summary>
	/// View controller for user settings.
	/// </summary>
	public partial class UserSettingsTableViewController : UITableViewController,
														   IUITextFieldDelegate,
														   ILevelSelectorViewControllerDelegate {
		private ButtonBeep effectsLevelReleased;

		public UserSettingsTableViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.playerNameTextField.Delegate = this;

			this.UpdateSelectedLevel ();

			this.playerNameTextField.Text = UserDefaults.Myself.Username;
			this.spectatorSwitch.On = UserDefaults.Spectator;
			this.musicVolume.Value = UserDefaults.MusicVolume;
			this.effectsVolume.Value = UserDefaults.EffectsVolume;
			this.effectsLevelReleased = ButtonBeep.Create ("catapult_highlight_on_02.wav", UserDefaults.EffectsVolume);
			this.appVersionLabel.Text = NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleShortVersionString") +
										$" ({NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleVersion")})";
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var segueIdentifier = segue.Identifier;
			if (!string.IsNullOrEmpty (segueIdentifier) &&
				Enum.TryParse<GameSegue> (segueIdentifier, true, out GameSegue segueType)) {
				switch (segueType) {
				case GameSegue.LevelSelector:
					if (segue.DestinationViewController is LevelSelectorViewController levelSelector) {
						levelSelector.Delegate = this;
					}

					break;
				}
			}
		}

		partial void spectatorChanged (UISwitch sender)
		{
			UserDefaults.Spectator = sender.On;
		}

		partial void musicVolumeChanged (UISlider sender)
		{
			UserDefaults.MusicVolume = sender.Value;
		}

		partial void effectVolumeChanged (UISlider sender)
		{
			UserDefaults.EffectsVolume = sender.Value;
		}

		partial void effectVolumeReleased (UISlider sender)
		{
			this.effectsLevelReleased.Play ();
		}

		partial void doneTapped (UIBarButtonItem sender)
		{
			this.DismissViewController (true, null);
		}

		private void UpdateSelectedLevel ()
		{
			var selectedLevel = UserDefaults.SelectedLevel;
			this.selectedLevelLabel.Text = selectedLevel.Name;
		}

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
			if (reason == UITextFieldDidEndEditingReason.Committed &&
				!string.IsNullOrEmpty (this.playerNameTextField.Text)) {
				UserDefaults.Myself = new Player (this.playerNameTextField.Text);
			} else {
				this.playerNameTextField.Text = UserDefaults.Myself.Username;
			}
		}

		#endregion

		#region ILevelSelectorViewControllerDelegate

		public void OnLevelSelected (LevelSelectorViewController levelSelectorViewController, GameLevel level)
		{
			this.UpdateSelectedLevel ();
		}

		#endregion
	}
}
