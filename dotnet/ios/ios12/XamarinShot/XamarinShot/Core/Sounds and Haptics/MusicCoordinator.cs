namespace XamarinShot.Models;

public class MusicCoordinator : NSObject
{
        readonly Dictionary<string, MusicPlayer> musicPlayers = new Dictionary<string, MusicPlayer> ();

        readonly Dictionary<string, MusicConfig> musicConfigurations;

        const double DefaultFadeOut = 0.2d;

        float musicGain = 1f;

        public MusicCoordinator () : base ()
        {
                UpdateMusicVolume ();

                var url = NSBundle.MainBundle.GetUrlForResource ("Sounds/music", "json");
                if (url is null)
                {
                        throw new Exception ("Failed to load music config from Sounds/music.json");
                }

                // parse 
                var json = NSString.FromData (NSData.FromUrl (url), NSStringEncoding.UTF8)!.ToString ();
                var configs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MusicConfig>> (json);
                if (configs is null)
                        throw new Exception ("Failed to load music configuration");
                musicConfigurations = configs;
                NSNotificationCenter.DefaultCenter.AddObserver (NSUserDefaults.DidChangeNotification, HandleDefaultsDidChange);
        }

        public MusicPlayer? CurrentMusicPlayer { get; private set; }

        void HandleDefaultsDidChange (NSNotification notification)
        {
                UpdateMusicVolume ();
        }

        private void UpdateMusicVolume ()
        {
                var volume = UserDefaults.MusicVolume;
                // Map the slider value from 0...1 to a more natural curve:
                musicGain = volume * volume;

                foreach (var (_, player) in musicPlayers.Where (player => player.Value.State == MusicState.Playing))
                {
                        var audioVolume = DigitExtensions.Clamp (musicGain * (float)Math.Pow (10f, player.Config.VolumeDB / 20f), 0f, 1f);
                        player.AudioPlayer.SetVolume (audioVolume, 0.1d);
                }
        }

        /// <summary>
        /// Get the current play position for the currently playing music
        /// </summary>
        /// <returns>Time in seconds, or -1 if nothing is playing.</returns>
        public double CurrentMusicTime ()
        {
                if (CurrentMusicPlayer is not null)
                {
                        return CurrentMusicPlayer.AudioPlayer.CurrentTime;
                } else {
                        return -1d;
                }
        }

        public MusicPlayer MusicPlayer (string name)
        {
                if (musicPlayers.TryGetValue (name, out MusicPlayer? player))
                {
                        return player;
                }

                if (!musicConfigurations.TryGetValue (name, out MusicConfig? config))
                {
                        throw new Exception ($"Missing music config for music event named '{name}'");
                }

                player = new MusicPlayer (name, config);
                musicPlayers [name] = player;
                return player;
        }

        public MusicPlayer PlayMusic (string name, double fadeIn = 0d)
        {
                return PlayMusic (name, 0, fadeIn);
        }

        public MusicPlayer PlayMusic (string name, double startTime, double fadeIn = 0d)
        {
                var player = MusicPlayer (name);
                var audioPlayer = player.AudioPlayer;

                if (CurrentMusicPlayer is not null)
                {
                        StopMusic (CurrentMusicPlayer);
                }

                switch (player.State)
                {
                        case MusicState.Playing:
                                // Nothing to do
                                return player;

                        case MusicState.Stopped:
                                // Configure the audioPlayer, starting with volume at 0 and then fade in.
                                audioPlayer.Volume = 0;
                                audioPlayer.CurrentTime = 0;
                                if (player.Config.Loops)
                                {
                                        audioPlayer.NumberOfLoops = -1;
                                } else {
                                        audioPlayer.NumberOfLoops = 0;
                                }

                                audioPlayer.CurrentTime = startTime;
                                audioPlayer.Play ();
                                break;

                        case MusicState.Stopping:
                                // Leave it playing. Update the volume and play state below.
                                break;
                }

                var volume = DigitExtensions.Clamp (musicGain * (float)Math.Pow (10f, player.Config.VolumeDB / 20f), 0f, 1f);
                audioPlayer.SetVolume (volume, fadeIn);

                player.State = MusicState.Playing;
                CurrentMusicPlayer = player;

                return player;
        }

        public void StopMusic (string name, double fadeOut = MusicCoordinator.DefaultFadeOut)
        {
                var player = MusicPlayer (name);
                StopMusic (player, fadeOut);
        }

        public void StopCurrentMusic (double fadeOut = MusicCoordinator.DefaultFadeOut)
        {
                if (CurrentMusicPlayer is not null)
                {
                        StopMusic (CurrentMusicPlayer, fadeOut);
                }
        }

        public void StopMusic (MusicPlayer player, double fadeOut = MusicCoordinator.DefaultFadeOut)
        {
                if (player.State == MusicState.Playing)
                {
                        player.State = MusicState.Stopping;
                        var audioPlayer = player.AudioPlayer;
                        audioPlayer.SetVolume (0f, fadeOut);
                        // TODO;
                        //DispatchQueue.MainQueue.DispatchAsync(deadline: .now() + fadeOut) {
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                if (player.State == MusicState.Stopping)
                                {
                                        audioPlayer.Stop ();
                                        player.State = MusicState.Stopped;
                                }
                        });
                }
        }

        /*  helpers  */

        public enum MusicState
        {
                Stopped,
                Playing,
                Stopping, // transition from play to stop, fading out.
        }

        public class MusicConfig
        {
                public string FileName { get; set; } = "";

                public float VolumeDB { get; set; }

                public bool Loops { get; set; }
        }
}

public class MusicPlayer
{
        public MusicPlayer (string name, MusicCoordinator.MusicConfig config)
        {
                Name = name;
                Config = config;
                State = MusicCoordinator.MusicState.Stopped;

                var splittedPath = config.FileName.Split (new char [] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var url = NSBundle.MainBundle.GetUrlForResource (splittedPath [0], splittedPath [1]);
                if (url is null)
                {
                        throw new Exception ($"Failed to load sound for: {name} expected at: {config.FileName}");
                }

                AudioPlayer = AVAudioPlayer.FromUrl (url, out NSError error);
                if (error is not null)
                {
                        throw new Exception ($"Failed to load sound for: {name} expected at: {config.FileName}");
                }
        }

        public double Duration => AudioPlayer.Duration;

        public string Name { get; set; }

        public MusicCoordinator.MusicState State { get; set; }

        public AVAudioPlayer AudioPlayer { get; private set; }

        public MusicCoordinator.MusicConfig Config { get; private set; }
}
