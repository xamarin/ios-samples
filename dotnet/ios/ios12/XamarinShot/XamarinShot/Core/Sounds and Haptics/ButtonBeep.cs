namespace XamarinShot.Models;

/// <summary>
/// Simple wrapper for UI audio effects.
/// </summary>
public class ButtonBeep
{
        static Dictionary<string, AVAudioPlayer> players;// = new Dictionary<string, AVAudioPlayer>();

        readonly float volume;

        static ButtonBeep ()
        {
                players = new Dictionary<string, AVAudioPlayer> ();
        }

        public ButtonBeep (float volume)
        {
                this.volume = volume;
        }

        public AVAudioPlayer? Player { get; set; }

        public static ButtonBeep? Create (string name, float volume)
        {
                var result = new ButtonBeep (volume);
                if (ButtonBeep.players.TryGetValue (name, out AVAudioPlayer? player))
                {
                        result.Player = player;
                }
                else
                {
                        var splitted = name.Split (new char [] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
                        var url = NSBundle.MainBundle.GetUrlForResource ($"Sounds/{splitted [0]}", splitted [1]);
                        if (url is null)
                        {
                                return null;
                        }

                        player = AVAudioPlayer.FromUrl (url, out NSError error);
                        if (player is not null)
                        {
                                player.PrepareToPlay ();
                                result.Player = player;
                                ButtonBeep.players [name] = player;
                        }
                        else
                        {
                                return null;
                        }
                }

                return result;
        }

        public void Play ()
        {
                if (Player is null)
                        return;
                Player.Volume = volume * SFXCoordinator.EffectsGain ();
                Player.Play ();
        }
}
