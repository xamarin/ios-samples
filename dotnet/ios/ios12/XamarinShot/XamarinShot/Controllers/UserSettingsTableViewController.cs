namespace XamarinShot;


/// <summary>
/// View controller for user settings.
/// </summary>
public partial class UserSettingsTableViewController : UITableViewController,
                                                   IUITextFieldDelegate,
                                                   ILevelSelectorViewControllerDelegate
{
        ButtonBeep? effectsLevelReleased;

        public UserSettingsTableViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();
                playerNameTextField.Delegate = this;

                UpdateSelectedLevel ();

                playerNameTextField.Text = UserDefaults.Myself.Username;
                spectatorSwitch.On = UserDefaults.Spectator;
                musicVolume.Value = UserDefaults.MusicVolume;
                effectsVolume.Value = UserDefaults.EffectsVolume;
                effectsLevelReleased = ButtonBeep.Create ("catapult_highlight_on_02.wav", UserDefaults.EffectsVolume);
                appVersionLabel.Text = NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleShortVersionString") +
                                            $" ({NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleVersion")})";
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject? sender)
        {
                var segueIdentifier = segue.Identifier;
                if (!string.IsNullOrEmpty (segueIdentifier) &&
                        Enum.TryParse<GameSegue> (segueIdentifier, true, out GameSegue segueType))
                {
                        switch (segueType)
                        {
                                case GameSegue.LevelSelector:
                                        if (segue.DestinationViewController is LevelSelectorViewController levelSelector)
                                        {
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
                effectsLevelReleased?.Play ();
        }

        partial void doneTapped (UIBarButtonItem sender)
        {
                DismissViewController (true, null);
        }

        void UpdateSelectedLevel ()
        {
                var selectedLevel = UserDefaults.SelectedLevel;
                selectedLevelLabel.Text = selectedLevel.Name;
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
                        !string.IsNullOrEmpty (playerNameTextField.Text))
                {
                        UserDefaults.Myself = new Player (playerNameTextField.Text);
                } else {
                        playerNameTextField.Text = UserDefaults.Myself.Username;
                }
        }

        #endregion

        #region ILevelSelectorViewControllerDelegate

        public void OnLevelSelected (LevelSelectorViewController levelSelectorViewController, GameLevel level)
        {
                UpdateSelectedLevel ();
        }

        #endregion
}
