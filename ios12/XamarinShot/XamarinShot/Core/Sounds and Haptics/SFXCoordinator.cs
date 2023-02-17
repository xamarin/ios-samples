
namespace XamarinShot.Models {
	using AVFoundation;
	using CoreFoundation;
	using Foundation;
	using SceneKit;
	using XamarinShot.Models.GameplayState;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;
	using UIKit;

	public class SFXCoordinator : NSObject {
		private readonly string [] preloadAudioFiles =
		{
			"vortex_04",
			"catapult_highlight_on_02",
			"catapult_highlight_off_02"
		};

		private readonly Dictionary<string, AVAudioPlayer> audioPlayers = new Dictionary<string, AVAudioPlayer> ();
		private readonly DispatchQueue playerQueue = new DispatchQueue ("SFXCoordinator");
		private readonly DispatchQueue loadQueue = new DispatchQueue ("SFXCoordinator.loading");

		private readonly List<AudioSampler> audioSamplers = new List<AudioSampler> ();

		private readonly HapticsGenerator hapticsGenerator = new HapticsGenerator ();
		private bool isStretchPlaying;
		private DateTime? timeSinceLastHaptic;
		private float? prevStretchDistance;
		private Catapult highlightedCatapult;
		private bool firstVortexCatapultBreak = true;

		private float effectsGain = 1f;

		public SFXCoordinator (AVAudioEnvironmentNode audioEnvironment) : base ()
		{
			this.AudioEnvironment = audioEnvironment;

			this.loadQueue.DispatchAsync (() => {
				foreach (var file in preloadAudioFiles) {
					this.PrepareAudioFile (file);
				}
			});

			// Because the coordinate space is scaled, let's apply some adjustments to the
			// distance attenuation parameters to make the distance rolloff sound more
			// realistic.

			this.AudioEnvironment.DistanceAttenuationParameters.ReferenceDistance = 5f;
			this.AudioEnvironment.DistanceAttenuationParameters.MaximumDistance = 40f;
			this.AudioEnvironment.DistanceAttenuationParameters.RolloffFactor = 1f;
			this.AudioEnvironment.DistanceAttenuationParameters.DistanceAttenuationModel = AVAudioEnvironmentDistanceAttenuationModel.Inverse;

			this.UpdateEffectsVolume ();

			// When the route changes, we need to reload our audio samplers because sometimes those
			// audio units are being reset.
			NSNotificationCenter.DefaultCenter.AddObserver (AVAudioSession.RouteChangeNotification, this.HandleRouteChange);

			// Subscribe to notifications of user defaults changing so that we can apply them to
			// sound effects.
			NSNotificationCenter.DefaultCenter.AddObserver (NSUserDefaults.DidChangeNotification, this.HandleDefaultsDidChange);

			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidEnterBackgroundNotification, this.HandleAppDidEnterBackground);
		}

		public AVAudioEnvironmentNode AudioEnvironment { get; private set; }

		public SCNMatrix4 RenderToSimulationTransform { get; set; } = SCNMatrix4.Identity;

		private void HandleRouteChange (NSNotification notification)
		{
			this.loadQueue.DispatchAsync (() => {
				foreach (var sampler in this.audioSamplers) {
					sampler.ReloadPreset ();
				}
			});
		}

		private void HandleDefaultsDidChange (NSNotification notification)
		{
			this.UpdateEffectsVolume ();
		}

		private void HandleAppDidEnterBackground (NSNotification notification)
		{
			foreach (var sampler in this.audioSamplers) {
				sampler.StopAllNotes ();
			}
		}

		public static float EffectsGain ()
		{
			var effectsVolume = UserDefaults.EffectsVolume;
			// Map the slider value from 0...1 to a more natural curve:
			return effectsVolume * effectsVolume;
		}

		private void UpdateEffectsVolume ()
		{
			this.effectsGain = SFXCoordinator.EffectsGain ();
			this.AudioEnvironment.OutputVolume = effectsGain;
		}

		public void SetupGameAudioComponent (GameAudioComponent component)
		{
			component.SfxCoordinator = this;
		}

		public void AttachSampler (AudioSampler audioSampler, SCNNode node)
		{
			// Add the audio Player to the scenekit node so that it gets correct positional
			// adjustments
			node.AddAudioPlayer (audioSampler.AudioPlayer);

			// NOTE: AVAudioNodes that are not AVAudioPlayerNodes are not
			// automatically added to an AVAudioEngine by SceneKit. So we add
			// this audio node to the SCNNode so that it can get position updates
			// but we connect it manually to the AVAudioEnvironmentNode that we
			// get passed to us. This comes from ARSCNView in GameSceneViewController.
			if (this.AudioEnvironment.Engine != null) {
				var engine = this.AudioEnvironment.Engine;

				// attach the node
				var audioNode = audioSampler.AudioNode;
				engine.AttachNode (audioNode);

				// connect
				var engineFormat = engine.OutputNode.GetBusInputFormat (0);
				var format = new AVAudioFormat (engineFormat.SampleRate, 1);
				engine.Connect (audioNode, AudioEnvironment, format);

				// addd to the local collection
				this.audioSamplers.Add (audioSampler);
			}
		}

		public void RemoveAllAudioSamplers ()
		{
			if (this.AudioEnvironment.Engine != null) {
				this.playerQueue.DispatchAsync (() => {
					foreach (var sampler in this.audioSamplers) {
						this.AudioEnvironment.Engine.DetachNode (sampler.AudioNode);
						sampler.Dispose ();
					}

					this.audioPlayers.Clear ();
					this.audioSamplers.Clear ();
				});
			}
		}

		private NSUrl UrlForSound (string name)
		{
			NSUrl result = null;
			foreach (var fileExtension in new string [] { "wav", "m4a" }) {
				var filename = $"Sounds/{name}";
				result = NSBundle.MainBundle.GetUrlForResource (filename, fileExtension);
				if (result != null) {
					break;
				}
			}

			return result;
		}

		public AVAudioPlayer CreatePlayer (string name)
		{
			var url = this.UrlForSound (name);
			if (url == null) {
				throw new Exception ($"Failed to load sound for: {name}");
			}

			var player = AVAudioPlayer.FromUrl (url, out NSError error);
			if (error != null) {
				throw new Exception ($"Failed to load sound for: {name}.\n{error?.LocalizedDescription}");
			}

			return player;
		}

		public void PrepareAudioFile (string name)
		{
			var needsToLoad = false;
			this.playerQueue.DispatchSync (() => {
				needsToLoad = !this.audioPlayers.ContainsKey (name) || this.audioPlayers [name] == null;
			});

			if (needsToLoad) {
				var player = this.CreatePlayer (name);
				player.PrepareToPlay ();
				this.playerQueue.DispatchSync (() => {
					this.audioPlayers [name] = player;
				});
			}
		}

		public void PlayAudioFile (string name, float volume = 1f, bool loop = false)
		{
			this.playerQueue.DispatchSync (() => {
				this.audioPlayers.TryGetValue (name, out AVAudioPlayer player);
				if (player == null) {
					player = this.CreatePlayer (name);
					this.audioPlayers [name] = player;
				}

				if (player != null) {
					player.Volume = volume * this.effectsGain;
					player.Play ();
					if (loop) {
						player.NumberOfLoops = -1;
					}
				}
			});
		}

		public void StopAudioFile (string name, double fadeDuration)
		{
			this.playerQueue.DispatchSync (() => {
				if (this.audioPlayers.TryGetValue (name, out AVAudioPlayer player)) {
					player.SetVolume (0f, fadeDuration);
					// TODO:  DispatchQueue.main.asyncAfter(deadline: .now() + fadeDur)
					DispatchQueue.MainQueue.DispatchAsync (player.Stop);
				}
			});
		}

		public void PlayStretch (Catapult catapult, float stretchDistance, float stretchRate, bool playHaptic)
		{
			var normalizedDistance = DigitExtensions.Clamp ((stretchDistance - 0.1f) / 2f, 0f, 1f);

			if (this.isStretchPlaying) {
				// Set the stretch distance and rate on the audio
				// player to module the strech sound effect.
				catapult.AudioPlayer.StretchDistance = normalizedDistance;
				catapult.AudioPlayer.StretchRate = stretchRate;
			} else {
				catapult.AudioPlayer.StartStretch ();
				this.isStretchPlaying = true;
			}

			if (playHaptic) {
				double interval;
				if (normalizedDistance >= 0.25 && normalizedDistance < 0.5) {
					interval = 0.5;
				} else if (normalizedDistance >= 0.5) {
					interval = 0.25;
				} else {
					interval = 1.0;
				}

				if (this.prevStretchDistance.HasValue) {
					var delta = Math.Abs (stretchDistance - this.prevStretchDistance.Value);
					this.prevStretchDistance = stretchDistance;
					if (delta < 0.0075f) {
						return;
					}
				} else {
					this.prevStretchDistance = stretchDistance;
				}

				if (this.timeSinceLastHaptic.HasValue) {
					if ((DateTime.UtcNow - this.timeSinceLastHaptic.Value).TotalSeconds > interval) {
						this.hapticsGenerator.GenerateImpactFeedback ();
						this.timeSinceLastHaptic = DateTime.UtcNow;
					}
				} else {
					this.hapticsGenerator.GenerateImpactFeedback ();
					this.timeSinceLastHaptic = DateTime.UtcNow;
				}
			}
		}

		public void StopStretch (Catapult catapult)
		{
			catapult.AudioPlayer.StopStretch ();
			catapult.AudioPlayer.StretchDistance = 0f;
			this.isStretchPlaying = false;
			this.timeSinceLastHaptic = null;
		}

		public void PlayLaunch (Catapult catapult, GameVelocity velocity, bool playHaptic)
		{
			if (playHaptic) {
				this.hapticsGenerator.GenerateNotificationFeedback (UINotificationFeedbackType.Success);
			}

			catapult.AudioPlayer.PlayLaunch (velocity);
		}

		public void PlayGrabBall (Catapult catapult)
		{
			catapult.AudioPlayer.PlayGrabBall ();
			// clear the highlight state so we don't play the highlight off
			// sound after the player has grabbed the ball.
			this.highlightedCatapult = null;
		}

		public void CatapultDidChangeHighlight (Catapult catapult, bool highlighted)
		{
			if (highlighted) {
				if (this.highlightedCatapult != catapult) {
					catapult.AudioPlayer.PlayHighlightOn ();
					this.highlightedCatapult = catapult;
				}
			} else {
				if (this.highlightedCatapult == catapult) {
					catapult.AudioPlayer.PlayHighlightOff ();
					this.highlightedCatapult = null;
				}
			}
		}

		public void PlayCatapultBreak (Catapult catapult, bool vortex)
		{
			// os_log(.info, "play catapult break for catapultID = %d", catapult.catapultID)
			var shouldPlay = true;
			if (vortex) {
				if (this.firstVortexCatapultBreak) {
					this.firstVortexCatapultBreak = false;
				} else {
					shouldPlay = false;
				}
			}

			if (shouldPlay) {
				catapult.AudioPlayer.PlayBreak ();
			}
		}

		public void PlayLeverHighlight (bool highlighted)
		{
			if (highlighted) {
				this.PlayAudioFile ("catapult_highlight_on_02", 0.2f);
			} else {
				this.PlayAudioFile ("catapult_highlight_off_02", 0.2f);
			}
		}
	}
}
