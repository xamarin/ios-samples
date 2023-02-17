
namespace XamarinShot.Models {
	using AVFoundation;
	using CoreFoundation;
	using Foundation;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MusicCoordinator : NSObject {
		private readonly Dictionary<string, MusicPlayer> musicPlayers = new Dictionary<string, MusicPlayer> ();

		private readonly Dictionary<string, MusicConfig> musicConfigurations;

		private const double DefaultFadeOut = 0.2d;

		private float musicGain = 1f;

		public MusicCoordinator () : base ()
		{
			this.UpdateMusicVolume ();

			var url = NSBundle.MainBundle.GetUrlForResource ("Sounds/music", "json");
			if (url == null) {
				throw new Exception ("Failed to load music config from Sounds/music.json");
			}

			// parse 
			var json = NSString.FromData (NSData.FromUrl (url), NSStringEncoding.UTF8).ToString ();
			this.musicConfigurations = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, MusicConfig>> (json);

			NSNotificationCenter.DefaultCenter.AddObserver (NSUserDefaults.DidChangeNotification, this.HandleDefaultsDidChange);
		}

		public MusicPlayer CurrentMusicPlayer { get; private set; }

		private void HandleDefaultsDidChange (NSNotification notification)
		{
			this.UpdateMusicVolume ();
		}

		private void UpdateMusicVolume ()
		{
			var volume = UserDefaults.MusicVolume;
			// Map the slider value from 0...1 to a more natural curve:
			this.musicGain = volume * volume;

			foreach (var (_, player) in this.musicPlayers.Where (player => player.Value.State == MusicState.Playing)) {
				var audioVolume = DigitExtensions.Clamp (this.musicGain * (float) Math.Pow (10f, player.Config.VolumeDB / 20f), 0f, 1f);
				player.AudioPlayer.SetVolume (audioVolume, 0.1d);
			}
		}

		/// <summary>
		/// Get the current play position for the currently playing music
		/// </summary>
		/// <returns>Time in seconds, or -1 if nothing is playing.</returns>
		public double CurrentMusicTime ()
		{
			if (this.CurrentMusicPlayer != null) {
				return this.CurrentMusicPlayer.AudioPlayer.CurrentTime;
			} else {
				return -1d;
			}
		}

		public MusicPlayer MusicPlayer (string name)
		{
			if (this.musicPlayers.TryGetValue (name, out MusicPlayer player)) {
				return player;
			}

			if (!this.musicConfigurations.TryGetValue (name, out MusicConfig config)) {
				throw new Exception ($"Missing music config for music event named '{name}'");
			}

			player = new MusicPlayer (name, config);
			this.musicPlayers [name] = player;
			return player;
		}

		public MusicPlayer PlayMusic (string name, double fadeIn = 0d)
		{
			return this.PlayMusic (name, 0, fadeIn);
		}

		public MusicPlayer PlayMusic (string name, double startTime, double fadeIn = 0d)
		{
			var player = this.MusicPlayer (name);
			var audioPlayer = player.AudioPlayer;

			if (this.CurrentMusicPlayer != null) {
				this.StopMusic (this.CurrentMusicPlayer);
			}

			switch (player.State) {
			case MusicState.Playing:
				// Nothing to do
				return player;

			case MusicState.Stopped:
				// Configure the audioPlayer, starting with volume at 0 and then fade in.
				audioPlayer.Volume = 0;
				audioPlayer.CurrentTime = 0;
				if (player.Config.Loops) {
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

			var volume = DigitExtensions.Clamp (this.musicGain * (float) Math.Pow (10f, player.Config.VolumeDB / 20f), 0f, 1f);
			audioPlayer.SetVolume (volume, fadeIn);

			player.State = MusicState.Playing;
			this.CurrentMusicPlayer = player;

			return player;
		}

		public void StopMusic (string name, double fadeOut = MusicCoordinator.DefaultFadeOut)
		{
			var player = this.MusicPlayer (name);
			this.StopMusic (player, fadeOut);
		}

		public void StopCurrentMusic (double fadeOut = MusicCoordinator.DefaultFadeOut)
		{
			if (this.CurrentMusicPlayer != null) {
				this.StopMusic (this.CurrentMusicPlayer, fadeOut);
			}
		}

		public void StopMusic (MusicPlayer player, double fadeOut = MusicCoordinator.DefaultFadeOut)
		{
			if (player.State == MusicState.Playing) {
				player.State = MusicState.Stopping;
				var audioPlayer = player.AudioPlayer;
				audioPlayer.SetVolume (0f, fadeOut);
				// TODO;
				//DispatchQueue.MainQueue.DispatchAsync(deadline: .now() + fadeOut) {
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (player.State == MusicState.Stopping) {
						audioPlayer.Stop ();
						player.State = MusicState.Stopped;
					}
				});
			}
		}

		/*  helpers  */

		public enum MusicState {
			Stopped,
			Playing,
			Stopping, // transition from play to stop, fading out.
		}

		public class MusicConfig {
			public string FileName { get; set; }

			public float VolumeDB { get; set; }

			public bool Loops { get; set; }
		}
	}

	public class MusicPlayer {
		public MusicPlayer (string name, MusicCoordinator.MusicConfig config)
		{
			this.Name = name;
			this.Config = config;
			this.State = MusicCoordinator.MusicState.Stopped;

			var splittedPath = config.FileName.Split (new char [] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			var url = NSBundle.MainBundle.GetUrlForResource (splittedPath [0], splittedPath [1]);
			if (url == null) {
				throw new Exception ($"Failed to load sound for: {name} expected at: {config.FileName}");
			}

			this.AudioPlayer = AVAudioPlayer.FromUrl (url, out NSError error);
			if (error != null) {
				throw new Exception ($"Failed to load sound for: {name} expected at: {config.FileName}");
			}
		}

		public double Duration => this.AudioPlayer.Duration;

		public string Name { get; set; }

		public MusicCoordinator.MusicState State { get; set; }

		public AVAudioPlayer AudioPlayer { get; private set; }

		public MusicCoordinator.MusicConfig Config { get; private set; }
	}
}
