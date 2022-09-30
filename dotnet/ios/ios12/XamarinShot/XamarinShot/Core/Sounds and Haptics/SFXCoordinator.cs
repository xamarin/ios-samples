namespace XamarinShot.Models;

public class SFXCoordinator : NSObject
{
        readonly string [] preloadAudioFiles =
        {
                "vortex_04",
                "catapult_highlight_on_02",
                "catapult_highlight_off_02"
        };

        readonly Dictionary<string, AVAudioPlayer> audioPlayers = new Dictionary<string, AVAudioPlayer> ();
        readonly DispatchQueue playerQueue = new DispatchQueue ("SFXCoordinator");
        readonly DispatchQueue loadQueue = new DispatchQueue ("SFXCoordinator.loading");

        readonly List<AudioSampler> audioSamplers = new List<AudioSampler> ();

        readonly HapticsGenerator hapticsGenerator = new HapticsGenerator ();
        bool isStretchPlaying;
        DateTime? timeSinceLastHaptic;
        float? prevStretchDistance;
        Catapult? highlightedCatapult;
        bool firstVortexCatapultBreak = true;

        float effectsGain = 1f;

        public SFXCoordinator (AVAudioEnvironmentNode audioEnvironment) : base ()
        {
                AudioEnvironment = audioEnvironment;

                loadQueue.DispatchAsync (() =>
                {
                        foreach (var file in preloadAudioFiles)
                        {
                                PrepareAudioFile (file);
                        }
                });

                // Because the coordinate space is scaled, let's apply some adjustments to the
                // distance attenuation parameters to make the distance rolloff sound more
                // realistic.

                AudioEnvironment.DistanceAttenuationParameters.ReferenceDistance = 5f;
                AudioEnvironment.DistanceAttenuationParameters.MaximumDistance = 40f;
                AudioEnvironment.DistanceAttenuationParameters.RolloffFactor = 1f;
                AudioEnvironment.DistanceAttenuationParameters.DistanceAttenuationModel = AVAudioEnvironmentDistanceAttenuationModel.Inverse;

                UpdateEffectsVolume ();

                // When the route changes, we need to reload our audio samplers because sometimes those
                // audio units are being reset.
                NSNotificationCenter.DefaultCenter.AddObserver (AVAudioSession.RouteChangeNotification, HandleRouteChange);

                // Subscribe to notifications of user defaults changing so that we can apply them to
                // sound effects.
                NSNotificationCenter.DefaultCenter.AddObserver (NSUserDefaults.DidChangeNotification, HandleDefaultsDidChange);

                NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidEnterBackgroundNotification, HandleAppDidEnterBackground);
        }

        public AVAudioEnvironmentNode AudioEnvironment { get; private set; }

        public SCNMatrix4 RenderToSimulationTransform { get; set; } = SCNMatrix4.Identity;

        void HandleRouteChange (NSNotification notification)
        {
                loadQueue.DispatchAsync (() =>
                {
                        foreach (var sampler in audioSamplers)
                        {
                                sampler.ReloadPreset ();
                        }
                });
        }

        void HandleDefaultsDidChange (NSNotification notification)
        {
                UpdateEffectsVolume ();
        }

        void HandleAppDidEnterBackground (NSNotification notification)
        {
                foreach (var sampler in audioSamplers)
                {
                        sampler.StopAllNotes ();
                }
        }

        public static float EffectsGain ()
        {
                var effectsVolume = UserDefaults.EffectsVolume;
                // Map the slider value from 0...1 to a more natural curve:
                return effectsVolume * effectsVolume;
        }

        void UpdateEffectsVolume ()
        {
                effectsGain = SFXCoordinator.EffectsGain ();
                AudioEnvironment.OutputVolume = effectsGain;
        }

        public void SetupGameAudioComponent (GameAudioComponent component)
        {
                component.SfxCoordinator = this;
        }

        public void AttachSampler (AudioSampler audioSampler, SCNNode node)
        {
                // Add the audio Player to the scenekit node so that it gets correct positional
                // adjustments
                node.AddAudioPlayer (audioSampler.AudioPlayer!);

                // NOTE: AVAudioNodes that are not AVAudioPlayerNodes are not
                // automatically added to an AVAudioEngine by SceneKit. So we add
                // this audio node to the SCNNode so that it can get position updates
                // but we connect it manually to the AVAudioEnvironmentNode that we
                // get passed to us. This comes from ARSCNView in GameSceneViewController.
                if (AudioEnvironment.Engine is not null)
                {
                        var engine = AudioEnvironment.Engine;

                        // attach the node
                        var audioNode = audioSampler.AudioNode;
                        if (audioNode is null)
                                throw new Exception ("no audio node to attach");
                        engine.AttachNode (audioNode);

                        // connect
                        var engineFormat = engine.OutputNode.GetBusInputFormat (0);
                        var format = new AVAudioFormat (engineFormat.SampleRate, 1);
                        engine.Connect (audioNode, AudioEnvironment, format);

                        // addd to the local collection
                        audioSamplers.Add (audioSampler);
                }
        }

        public void RemoveAllAudioSamplers ()
        {
                if (AudioEnvironment.Engine is not null)
                {
                        playerQueue.DispatchAsync (() =>
                        {
                                foreach (var sampler in audioSamplers)
                                {
                                        if (sampler.AudioNode is not null)
                                                AudioEnvironment.Engine.DetachNode (sampler.AudioNode);
                                        sampler.Dispose ();
                                }

                                audioPlayers.Clear ();
                                audioSamplers.Clear ();
                        });
                }
        }

        NSUrl? UrlForSound (string name)
        {
                NSUrl? result = null;
                foreach (var fileExtension in new string [] { "wav", "m4a" })
                {
                        var filename = $"Sounds/{name}";
                        result = NSBundle.MainBundle.GetUrlForResource (filename, fileExtension);
                        if (result is not null)
                        {
                                break;
                        }
                }

                return result;
        }

        public AVAudioPlayer CreatePlayer (string name)
        {
                var url = UrlForSound (name);
                if (url is null)
                {
                        throw new Exception ($"Failed to load sound for: {name}");
                }

                var player = AVAudioPlayer.FromUrl (url, out NSError error);
                if (error is not null)
                {
                        throw new Exception ($"Failed to load sound for: {name}.\n{error?.LocalizedDescription}");
                }

                return player;
        }

        public void PrepareAudioFile (string name)
        {
                var needsToLoad = false;
                playerQueue.DispatchSync (() =>
                {
                        needsToLoad = !audioPlayers.ContainsKey (name) || audioPlayers [name] is null;
                });

                if (needsToLoad)
                {
                        var player = CreatePlayer (name);
                        player.PrepareToPlay ();
                        playerQueue.DispatchSync (() =>
                        {
                                audioPlayers [name] = player;
                        });
                }
        }

        public void PlayAudioFile (string name, float volume = 1f, bool loop = false)
        {
                playerQueue.DispatchSync (() =>
                {
                        audioPlayers.TryGetValue (name, out AVAudioPlayer? player);
                        if (player is null)
                        {
                                player = CreatePlayer (name);
                                audioPlayers [name] = player;
                        }

                        if (player is not null)
                        {
                                player.Volume = volume * effectsGain;
                                player.Play ();
                                if (loop)
                                {
                                        player.NumberOfLoops = -1;
                                }
                        }
                });
        }

        public void StopAudioFile (string name, double fadeDuration)
        {
                playerQueue.DispatchSync (() =>
                {
                        if (audioPlayers.TryGetValue (name, out AVAudioPlayer? player))
                        {
                                player.SetVolume (0f, fadeDuration);
                                // TODO:  DispatchQueue.main.asyncAfter(deadline: .now() + fadeDur)
                                DispatchQueue.MainQueue.DispatchAsync (player.Stop);
                        }
                });
        }

        public void PlayStretch (Catapult catapult, float stretchDistance, float stretchRate, bool playHaptic)
        {
                var normalizedDistance = DigitExtensions.Clamp ((stretchDistance - 0.1f) / 2f, 0f, 1f);

                if (isStretchPlaying)
                {
                        // Set the stretch distance and rate on the audio
                        // player to module the strech sound effect.
                        catapult.AudioPlayer.StretchDistance = normalizedDistance;
                        catapult.AudioPlayer.StretchRate = stretchRate;
                } else {
                        catapult.AudioPlayer.StartStretch ();
                        isStretchPlaying = true;
                }

                if (playHaptic)
                {
                        double interval;
                        if (normalizedDistance >= 0.25 && normalizedDistance < 0.5)
                        {
                                interval = 0.5;
                        } else if (normalizedDistance >= 0.5) {
                                interval = 0.25;
                        } else {
                                interval = 1.0;
                        }

                        if (prevStretchDistance.HasValue)
                        {
                                var delta = Math.Abs (stretchDistance - prevStretchDistance.Value);
                                prevStretchDistance = stretchDistance;
                                if (delta < 0.0075f)
                                {
                                        return;
                                }
                        } else {
                                prevStretchDistance = stretchDistance;
                        }

                        if (timeSinceLastHaptic.HasValue)
                        {
                                if ((DateTime.UtcNow - timeSinceLastHaptic.Value).TotalSeconds > interval)
                                {
                                        hapticsGenerator.GenerateImpactFeedback ();
                                        timeSinceLastHaptic = DateTime.UtcNow;
                                }
                        } else {
                                hapticsGenerator.GenerateImpactFeedback ();
                                timeSinceLastHaptic = DateTime.UtcNow;
                        }
                }
        }

        public void StopStretch (Catapult catapult)
        {
                catapult.AudioPlayer.StopStretch ();
                catapult.AudioPlayer.StretchDistance = 0f;
                isStretchPlaying = false;
                timeSinceLastHaptic = null;
        }

        public void PlayLaunch (Catapult catapult, GameVelocity velocity, bool playHaptic)
        {
                if (playHaptic)
                {
                        hapticsGenerator.GenerateNotificationFeedback (UINotificationFeedbackType.Success);
                }

                catapult.AudioPlayer.PlayLaunch (velocity);
        }

        public void PlayGrabBall (Catapult catapult)
        {
                catapult.AudioPlayer.PlayGrabBall ();
                // clear the highlight state so we don't play the highlight off
                // sound after the player has grabbed the ball.
                highlightedCatapult = null;
        }

        public void CatapultDidChangeHighlight (Catapult catapult, bool highlighted)
        {
                if (highlighted)
                {
                        if (highlightedCatapult != catapult)
                        {
                                catapult.AudioPlayer.PlayHighlightOn ();
                                highlightedCatapult = catapult;
                        }
                } else {
                        if (highlightedCatapult == catapult)
                        {
                                catapult.AudioPlayer.PlayHighlightOff ();
                                highlightedCatapult = null;
                        }
                }
        }

        public void PlayCatapultBreak (Catapult catapult, bool vortex)
        {
                // os_log(.info, "play catapult break for catapultID = %d", catapult.catapultID)
                var shouldPlay = true;
                if (vortex)
                {
                        if (firstVortexCatapultBreak)
                        {
                                firstVortexCatapultBreak = false;
                        } else {
                                shouldPlay = false;
                        }
                }

                if (shouldPlay)
                {
                        catapult.AudioPlayer.PlayBreak ();
                }
        }

        public void PlayLeverHighlight (bool highlighted)
        {
                if (highlighted)
                {
                        PlayAudioFile ("catapult_highlight_on_02", 0.2f);
                } else {
                        PlayAudioFile ("catapult_highlight_off_02", 0.2f);
                }
        }
}
