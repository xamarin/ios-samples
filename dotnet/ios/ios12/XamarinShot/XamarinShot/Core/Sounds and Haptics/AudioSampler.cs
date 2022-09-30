namespace XamarinShot.Models;

public class AudioSampler : IDisposable
{
        // serial queue for loading the sampler presets at background priority
        static DispatchQueue LoadingQueue;

        readonly NSConditionLock loaded = new NSConditionLock (0);

        SCNNode node;

        NSUrl presetUrl;

        float pitchBend = 0f;

        float modWheel = 0f;

        static AudioSampler ()
        {
                LoadingQueue = new DispatchQueue ("AudioSampler.loading");
        }

        public AudioSampler (string name, SCNNode node, SFXCoordinator sfxCoordinator)
        {
                this.node = node;
                AudioNode = new AUSamplerNode ();
                AudioPlayer = new SCNAudioPlayer (AudioNode);

                presetUrl = NSBundle.MainBundle.GetUrlForResource ($"Sounds/{name}", "aupreset");
                if (presetUrl is null)
                {
                        throw new System.IO.FileNotFoundException ("Failed to load preset.");
                }

                AudioSampler.LoadingQueue.DispatchAsync (() =>
                {
                        loaded.LockWhenCondition (0);

                        AudioNode.LoadAudioUnitPreset (presetUrl, out NSError error);

                        sfxCoordinator.AttachSampler (this, this.node);

                        // now this sampler is ready to play.
                        loaded.UnlockWithCondition (1);
                });
        }

        public SCNAudioPlayer? AudioPlayer { get; private set; }

        public AUSamplerNode? AudioNode { get; private set; }

        public float PitchBend
        {
                get
                {
                        return pitchBend;
                }

                set
                {
                        pitchBend = value;
                        // MIDI pitch bend is a 14-bit value from 0..16383, with zero pitch bend
                        // applied at 8192.
                        var shortValue = (ushort)(8192 + DigitExtensions.Clamp (pitchBend, -1, 1) * 8191);
                        AudioNode?.SendPitchBend (shortValue, 0);
                }
        }

        public float ModWheel
        {
                get
                {
                        return modWheel;
                }

                set
                {
                        modWheel = value;
                        // MIDI mod wheel is controller #1 and in range 0..127
                        var byteValue = (byte)(DigitExtensions.Clamp (modWheel, 0, 1) * 127);
                        AudioNode?.SendController (1, byteValue, 0);
                }
        }

        protected void After (Action action, int interval = 1)
        {
                // DispatchQueue.main.asyncAfter(deadline: .now() + interval, execute: action)
                System.Threading.Thread.Sleep (interval);
                DispatchQueue.MainQueue.DispatchAsync (action);
        }

        public void ReloadPreset ()
        {
                AudioNode?.LoadAudioUnitPreset (presetUrl, out NSError error);
        }

        public void Play (byte note, byte velocity, bool autoStop = true)
        {
                if (loaded.Condition == 1)
                {
                        AudioNode?.StartNote (note, velocity, 0);

                        if (autoStop)
                        {
                                After (() => AudioNode?.StopNote (note, 0));
                        }
                }
        }

        public void Stop (byte note)
        {
                AudioNode?.StopNote (note, 0);
        }

        public void StopAllNotes ()
        {
                // Send All Notes Off control message.
                AudioNode?.SendController (123, 0, 0);
        }

        #region IDisposable

        bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
                if (!isDisposed)
                {
                        if (disposing)
                        {
                                loaded.Dispose ();
                                presetUrl.Dispose ();

                                AudioNode?.Dispose ();
                                AudioNode = null;

                                AudioPlayer?.Dispose ();
                                AudioPlayer = null;
                        }

                        isDisposed = true;
                }
        }

        public void Dispose ()
        {
                Dispose (true);
                GC.SuppressFinalize (this);
        }

        #endregion
}
