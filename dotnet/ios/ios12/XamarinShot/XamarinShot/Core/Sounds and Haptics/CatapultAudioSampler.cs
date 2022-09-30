namespace XamarinShot.Models;

public class CatapultAudioSampler : AudioSampler
{
        const float StretchMinimumExpression = 75f; // in Midi controller value range 0..127
        const float StretchMaximumExpression = 127f; // in Midi controller value range 0..127
        const float StretchMaximumRate = 2.5f;

        float stretchDistance = 0f;
        float stretchRate = 0f;

        public CatapultAudioSampler (SCNNode node, SFXCoordinator sfxCoordinator) : base ("catapult", node, sfxCoordinator) { }

        public float StretchDistance
        {
                get
                {
                        return stretchDistance;
                }

                set
                {
                        stretchDistance = value;
                        if (!float.IsNaN (stretchDistance))
                        {
                                // apply stretch distance as pitch bend from 0...1, using pitch variation defined
                                // in the AUSampler preset.
                                PitchBend = DigitExtensions.Clamp (stretchDistance, 0, 1);
                        }
                }
        }

        public float StretchRate
        {
                get
                {
                        return stretchRate;
                }

                set
                {
                        stretchRate = value;
                        if (!float.IsNaN (stretchRate))
                        {
                                var normalizedStretch = stretchRate / StretchMaximumRate;
                                var controllerValue = (byte)(DigitExtensions.Clamp (StretchMinimumExpression + normalizedStretch * (StretchMaximumExpression - StretchMinimumExpression), 0, 127));
                                // midi expression, controller change# 11
                                AudioNode?.SendController (11, controllerValue, 0);
                        }
                }
        }

        public void StartStretch ()
        {
                AudioNode?.SendController (11, 127, 0);
                Play (Note.Stretch, 105, false);
                PitchBend = 0;
        }

        public void StopStretch ()
        {
                Stop (note: Note.Stretch);
                PitchBend = 0;
        }

        public void PlayLaunch (GameVelocity velocity)
        {
                // For the launch, we will play two sounds: a launch twang and a swish
                var length = velocity.Vector.Length;
                length = float.IsNaN (length) ? 0 : DigitExtensions.Clamp (length, 0, 1);

                var launchVel = (byte)(length * 30 + 80);
                Play (Note.Launch, launchVel);

                var swishVel = (byte)(length * 63 + 64);
                After (() => Play (Note.LaunchSwish, swishVel), 1);
        }

        public void PlayHighlightOn ()
        {
                Play (Note.HighlightOn, 90);
        }

        public void PlayHighlightOff ()
        {
                Play (Note.HighlightOff, 90);
        }

        public void PlayGrabBall ()
        {
                // reset pitch bend to 0 on grab
                PitchBend = 0;
                Play (Note.GrabBall, 110);
        }

        public void PlayBreak ()
        {
                Play (Note.Broken, 85);
        }

        static class Note
        {
                public static byte Stretch { get; set; } = 24; // Midi note for C1
                public static byte Launch { get; set; } = 26; // Midi note for D1
                public static byte LaunchSwish { get; set; } = 28; // Midi note for E1
                public static byte GrabBall { get; set; } = 31; // Midi note for G1
                public static byte HighlightOn { get; set; } = 38;// Midi note for D2
                public static byte HighlightOff { get; set; } = 41;// Midi note for F2
                public static byte Broken { get; set; } = 33; // Midi note for A1
        }
}
