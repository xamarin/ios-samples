
namespace XamarinShot.Models
{
    using CoreFoundation;
    using Foundation;
    using SceneKit;
    using XamarinShot.Utils;
    using System;

    public class AudioSampler : IDisposable
    {
        // serial queue for loading the sampler presets at background priority
        private static DispatchQueue LoadingQueue;

        private readonly NSConditionLock loaded = new NSConditionLock(0);

        private SCNNode node;

        private NSUrl presetUrl;

        private float pitchBend = 0f;

        private float modWheel = 0f;

        static AudioSampler()
        {
            LoadingQueue = new DispatchQueue("AudioSampler.loading");
        }

        public AudioSampler(string name, SCNNode node, SFXCoordinator sfxCoordinator)
        {
            this.node = node;
            this.AudioNode = new AUSamplerNode();
            this.AudioPlayer = new SCNAudioPlayer(this.AudioNode);

            this.presetUrl = NSBundle.MainBundle.GetUrlForResource($"Sounds/{name}", "aupreset");
            if (this.presetUrl == null)
            {
                throw new System.IO.FileNotFoundException("Failed to load preset.");
            }

            AudioSampler.LoadingQueue.DispatchAsync(() =>
            {
                this.loaded.LockWhenCondition(0);

                this.AudioNode.LoadAudioUnitPreset(this.presetUrl, out NSError error);

                sfxCoordinator.AttachSampler(this, this.node);

                // now this sampler is ready to play.
                this.loaded.UnlockWithCondition(1);
            });
        }

        public SCNAudioPlayer AudioPlayer { get; private set; }

        public AUSamplerNode AudioNode { get; private set; }

        public float PitchBend
        {
            get
            {
                return this.pitchBend;
            }

            set
            {
                this.pitchBend = value;
                // MIDI pitch bend is a 14-bit value from 0..16383, with zero pitch bend
                // applied at 8192.
                var shortValue = (ushort)(8192 + DigitExtensions.Clamp(this.pitchBend, -1, 1) * 8191);
                this.AudioNode.SendPitchBend(shortValue, 0);
            }
        }

        public float ModWheel
        {
            get
            {
                return this.modWheel;
            }

            set
            {
                this.modWheel = value;
                // MIDI mod wheel is controller #1 and in range 0..127
                var byteValue = (byte)(DigitExtensions.Clamp(this.modWheel, 0, 1) * 127);
                this.AudioNode.SendController(1, byteValue, 0);
            }
        }

        protected void After(Action action, int interval = 1)
        {
            // DispatchQueue.main.asyncAfter(deadline: .now() + interval, execute: action)
            System.Threading.Thread.Sleep(interval);
            DispatchQueue.MainQueue.DispatchAsync(action);
        }

        public void ReloadPreset()
        {
            this.AudioNode.LoadAudioUnitPreset(this.presetUrl, out NSError error);
        }

        public void Play(byte note, byte velocity, bool autoStop = true)
        {
            if (this.loaded.Condition == 1)
            {
                this.AudioNode.StartNote(note, velocity, 0);

                if (autoStop)
                {
                    this.After(() => this.AudioNode.StopNote(note, 0));
                }
            }
        }

        public void Stop(byte note)
        {
            this.AudioNode.StopNote(note, 0);
        }

        public void StopAllNotes()
        {
            // Send All Notes Off control message.
            this.AudioNode.SendController(123, 0, 0);
        }

        #region IDisposable

        private bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.loaded.Dispose();
                    this.presetUrl.Dispose();

                    this.AudioNode.Dispose();
                    this.AudioNode = null;

                    this.AudioPlayer.Dispose();
                    this.AudioPlayer = null;
                }

                this.isDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(true);
        }

        #endregion
    }
}