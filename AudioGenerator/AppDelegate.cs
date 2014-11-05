using System;

using Foundation;
using UIKit;
using AudioToolbox;
using AVFoundation;

namespace tone
{
    [Register("AppDelegate")]
    unsafe public partial class AppDelegate : UIApplicationDelegate
    {
        double sampleRate;
        const int numBuffers = 3;
        bool alternate;
        private NSError error;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //
            // Setup audio system
            //
            var session = AVAudioSession.SharedInstance();
            session.SetCategory(new NSString("AVAudioSessionCategoryPlayback"), AVAudioSessionCategoryOptions.DefaultToSpeaker, out error);


            // 
            // Format description, we generate LinearPCM as short integers
            //
            sampleRate = session.SampleRate;
            var format = new AudioStreamBasicDescription
            {
                SampleRate = sampleRate,
                Format = AudioFormatType.LinearPCM,
                FormatFlags = AudioFormatFlags.LinearPCMIsSignedInteger | AudioFormatFlags.LinearPCMIsPacked,
                BitsPerChannel = 16,
                ChannelsPerFrame = 1,
                BytesPerFrame = 2,
                BytesPerPacket = 2, 
                FramesPerPacket = 1,
            };

            // 
            // Create an output queue
            //
            var queue = new OutputAudioQueue(format);
            var bufferByteSize = (sampleRate > 16000) ? 2176 : 512; // 40.5 Hz : 31.25 Hz 

            // 
            // Create three buffers, generate a tone, and output the tones
            //
            var buffers = new AudioQueueBuffer* [numBuffers];
            for (int i = 0; i < numBuffers; i++)
            {
                queue.AllocateBuffer(bufferByteSize, out buffers[i]);
                GenerateTone(buffers[i]);
                queue.EnqueueBuffer(buffers[i], null);
            }

            //
            // Output callback: invoked when the audio system is done with the
            // buffer, this implementation merely recycles it.
            //
            queue.OutputCompleted += (object sender, OutputCompletedEventArgs e) =>
            {
                if (alternate)
                {
                    outputWaveForm += 1;
                    if (outputWaveForm > WaveForm.Square)
                        outputWaveForm = WaveForm.Sine;
                    GenerateTone(e.UnsafeBuffer);
                }
                queue.EnqueueBuffer(e.UnsafeBuffer, null);
            };

            queue.Start();
            return true;
        }


        // Configuration options for the audio output
        const float outputFrequency = 220;

        enum WaveForm
        {
            Sine,
            Triangle,
            Sawtooth,
            Square
        }

        WaveForm outputWaveForm;

        //
        // Simple tone generator
        //
        void GenerateTone(AudioQueueBuffer*buffer)
        {
            // Make the buffer length a multiple of the wavelength for the output frequency.
            uint sampleCount = buffer->AudioDataBytesCapacity / 2;
            double bufferLength = sampleCount;
            double wavelength = sampleRate / outputFrequency;
            double repetitions = Math.Floor(bufferLength / wavelength);
            if (repetitions > 0)
                sampleCount = (uint)Math.Round(wavelength * repetitions);

            double x, y;
            double sd = 1.0 / sampleRate;
            double amp = 0.9;
            double max16bit = Int16.MaxValue;
            int i;
            short* p = (short*)buffer->AudioData;
				
            for (i = 0; i < sampleCount; i++)
            {
                x = i * sd * outputFrequency;
                switch (outputWaveForm)
                {
                    case WaveForm.Sine: 
                        y = Math.Sin(x * 2.0 * Math.PI);
                        break;
                    case WaveForm.Triangle:
                        x = x % 1.0;
                        if (x < 0.25)
                            y = x * 4.0; // up 0.0 to 1.0
						else if (x < 0.75)
                            y = (1.0 - x) * 4.0 - 2.0; // down 1.0 to -1.0
						else
                            y = (x - 1.0) * 4.0; // up -1.0 to 0.0
                        break;
                    case WaveForm.Sawtooth:
                        y = 0.8 - (x % 1.0) * 1.8;
                        break;
                    case WaveForm.Square:
                        y = ((x % 1.0) < 0.5) ? 0.7 : -0.7;
                        break;
                    default:
                        y = 0;
                        break;
                }
                p[i] = (short)(y * max16bit * amp);
            }
			
            buffer->AudioDataByteSize = sampleCount * 2;
        }

        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}

