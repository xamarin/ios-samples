//
// AudioConverterFileConverterViewController.cs:
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using CoreGraphics;

using Foundation;
using UIKit;
using AudioToolbox;
using System.Linq;
using CoreFoundation;
using System.Diagnostics;
using AVFoundation;
using System.Runtime.InteropServices;

namespace AudioConverterFileConverter
{
    public partial class AudioConverterFileConverterViewController : UIViewController
    {
        class AudioFileIO
        {
            public AudioFileIO(int bufferSize)
            {
                this.SrcBufferSize = bufferSize;
                this.SrcBuffer = Marshal.AllocHGlobal(bufferSize);
            }

            ~AudioFileIO ()
            {
                Marshal.FreeHGlobal(SrcBuffer);
            }

            public AudioFile SourceFile { get; set; }

            public int SrcBufferSize { get; private set; }

            public IntPtr SrcBuffer { get; private set; }

            public int SrcFilePos { get; set; }

            public AudioStreamBasicDescription SrcFormat { get; set; }

            public int SrcSizePerPacket { get; set; }

            public int NumPacketsPerRead { get; set; }

            public AudioStreamPacketDescription[] PacketDescriptions { get; set; }
        }

        AudioFormatType outputFormat;
        double sampleRate;
        CFUrl sourceURL;
        NSUrl destinationURL;
        string destinationFilePath;
        AudioFileIO afio;

        public AudioConverterFileConverterViewController(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIImage greenImage = new UIImage("green_button.png").StretchableImage(12, 0);
            UIImage redImage = new UIImage("red_button.png").StretchableImage(12, 0);

            startButton.SetBackgroundImage(greenImage, UIControlState.Normal);
            startButton.SetBackgroundImage(redImage, UIControlState.Disabled);

            // default output format
            // sample rate of 0 indicates source file sample rate
            outputFormat = AudioFormatType.AppleLossless;
            sampleRate = 0;

            // can we encode to AAC?
            if (IsAACHardwareEncoderAvailable())
            {
                outputFormatSelector.SetEnabled(true, 0);
            }
            else
            {
                // even though not enabled in IB, this segment will still be enabled
                // if not specifically turned off here which we'll assume is a bug
                outputFormatSelector.SetEnabled(false, 0);
            }

            sourceURL = CFUrl.FromFile("sourcePCM.aif");
            var paths = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
            destinationFilePath = paths[0] + "/output.caf";
            destinationURL = NSUrl.FromFilename(destinationFilePath);

            UpdateFormatInfo(fileInfo, sourceURL);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        #endregion

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
        }

        partial void segmentedControllerValueChanged(UISegmentedControl sender)
        {
            switch (sender.Tag)
            {
                case 0:
                    switch (sender.SelectedSegment)
                    {
                        case 0:
                            outputFormat = AudioFormatType.MPEG4AAC;
                            outputSampleRateSelector.SetEnabled(true, 0);
                            outputSampleRateSelector.SetEnabled(true, 1);
                            outputSampleRateSelector.SetEnabled(true, 2);
                            outputSampleRateSelector.SetEnabled(true, 3);
                            break;
                        case 1:
                            outputFormat = AudioFormatType.AppleIMA4;
                            outputSampleRateSelector.SetEnabled(true, 0);
                            outputSampleRateSelector.SetEnabled(true, 1);
                            outputSampleRateSelector.SetEnabled(true, 2);
                            outputSampleRateSelector.SetEnabled(true, 3);
                            break;
                        case 2:
					// iLBC sample rate is 8K
                            outputFormat = AudioFormatType.iLBC;
                            sampleRate = 8000.0;
                            outputSampleRateSelector.SelectedSegment = 2;
                            outputSampleRateSelector.SetEnabled(false, 0);
                            outputSampleRateSelector.SetEnabled(false, 1);
                            outputSampleRateSelector.SetEnabled(true, 2);
                            outputSampleRateSelector.SetEnabled(false, 3);
                            break;
                        case 3:
                            outputFormat = AudioFormatType.AppleLossless;
                            outputSampleRateSelector.SetEnabled(true, 0);
                            outputSampleRateSelector.SetEnabled(true, 1);
                            outputSampleRateSelector.SetEnabled(true, 2);
                            outputSampleRateSelector.SetEnabled(true, 3);
                            break;
                    }
                    break;
                case 1:
                    switch (sender.SelectedSegment)
                    {
                        case 0:
                            sampleRate = 44100.0;
                            break;
                        case 1:
                            sampleRate = 22050.0;
                            break;
                        case 2:
                            sampleRate = 8000.0;
                            break;
                        case 3:
                            sampleRate = 0;
                            break;
                    }
                    break;
            }
        }

        partial void convertButtonPressed(NSObject sender)
        {
            // use AudioSessionCategory::AudioProcessing category for offline conversion when not playing or recording audio at the same time
            // if you are recording or playing audio at the same time you are encoding, use the same Audio Session category that you would normally

            try
            {
                AudioSession.Category = AudioSessionCategory.AudioProcessing;
            }
            catch
            {
                Debug.Print("Cannot set audio session category");
            }

            startButton.SetTitle("Converting...", UIControlState.Disabled);
            startButton.Enabled = false;

            activityIndicator.StartAnimating();

            // run audio file code in a background thread
            InvokeInBackground(ConvertAudio);
        }

        static bool IsAACHardwareEncoderAvailable()
        {
            AudioClassDescription[] encoders = AudioFormatAvailability.GetEncoders(AudioFormatType.MPEG4AAC);
            return encoders.Any(l => l.SubType == AudioFormatType.MPEG4AAC);
        }

        static void UpdateFormatInfo(UILabel label, NSUrl fileURL)
        {
            UpdateFormatInfo(label, AudioFile.Open(fileURL, AudioFilePermission.Read), System.IO.Path.GetFileName(fileURL.Path));
        }

        static void UpdateFormatInfo(UILabel label, CFUrl fileURL)
        {
            UpdateFormatInfo(label, AudioFile.Open(fileURL, AudioFilePermission.Read), fileURL.FileSystemPath);
        }

        static void UpdateFormatInfo(UILabel label, AudioFile fileID, string fileName)
        {
            var asbd = fileID.DataFormat.Value;
            label.Text = string.Format("{0} {1} {2} Hz ({3} ch.)",
                fileName, asbd.Format, asbd.SampleRate, asbd.ChannelsPerFrame);
        }

        void UpdateUI()
        {
            startButton.Enabled = true;
            UpdateFormatInfo(fileInfo, sourceURL);
        }

        void ConvertAudio()
        {
            var success = DoConvertFile(sourceURL, destinationURL, outputFormat, sampleRate);

            BeginInvokeOnMainThread(delegate
            {
                activityIndicator.StopAnimating();
            });

            if (!success)
            {
                var fm = new NSFileManager();
                if (fm.FileExists(destinationFilePath))
                {
                    NSError error;
                    fm.Remove(destinationFilePath, out error);
                }
                BeginInvokeOnMainThread(UpdateUI);
            }
            else
            {
                BeginInvokeOnMainThread(playAudio);
            }
        }

        bool DoConvertFile(CFUrl sourceURL, NSUrl destinationURL, AudioFormatType outputFormatType, double outputSampleRate)
        {
            // in this sample we should never be on the main thread here
            Debug.Assert(!NSThread.IsMain);

            // transition thread state to State::Running before continuing
            AppDelegate.ThreadStateSetRunning();

            Debug.WriteLine("DoConvertFile");

            // get the source file
            AudioFile sourceFile = AudioFile.Open(sourceURL, AudioFilePermission.Read);

            var srcFormat = (AudioStreamBasicDescription)sourceFile.DataFormat;
			var dstFormat = new AudioStreamBasicDescription();

            // setup the output file format
            dstFormat.SampleRate = (outputSampleRate == 0 ? srcFormat.SampleRate : outputSampleRate); // set sample rate
            if (outputFormatType == AudioFormatType.LinearPCM)
            {
                // if the output format is PCM create a 16-bit int PCM file format description as an example
				dstFormat.Format = AudioFormatType.LinearPCM;
                dstFormat.ChannelsPerFrame = srcFormat.ChannelsPerFrame;
                dstFormat.BitsPerChannel = 16;
                dstFormat.BytesPerPacket = dstFormat.BytesPerFrame = 2 * dstFormat.ChannelsPerFrame;
                dstFormat.FramesPerPacket = 1;
                dstFormat.FormatFlags = AudioFormatFlags.LinearPCMIsPacked | AudioFormatFlags.LinearPCMIsSignedInteger;
            }
            else
            {
                // compressed format - need to set at least format, sample rate and channel fields for kAudioFormatProperty_FormatInfo
                dstFormat.Format = outputFormatType;
                dstFormat.ChannelsPerFrame = (outputFormatType == AudioFormatType.iLBC ? 1 : srcFormat.ChannelsPerFrame); // for iLBC num channels must be 1

                // use AudioFormat API to fill out the rest of the description
				AudioFormatError afe = AudioStreamBasicDescription.GetFormatInfo(ref dstFormat);
                if (afe != AudioFormatError.None)
                {
                    Debug.Print("Cannot create destination format {0:x}", afe);

                    AppDelegate.ThreadStateSetDone();
                    return false;
                }
            }

            // create the AudioConverter
            AudioConverterError ce;
            var converter = AudioConverter.Create(srcFormat, dstFormat, out ce);
            Debug.Assert(ce == AudioConverterError.None);

            converter.InputData += EncoderDataProc;

            // if the source has a cookie, get it and set it on the Audio Converter
            ReadCookie(sourceFile, converter);

            // get the actual formats back from the Audio Converter
            srcFormat = converter.CurrentInputStreamDescription;
            dstFormat = converter.CurrentOutputStreamDescription;

            // if encoding to AAC set the bitrate to 192k which is a nice value for this demo
            // kAudioConverterEncodeBitRate is a UInt32 value containing the number of bits per second to aim for when encoding data
            if (dstFormat.Format == AudioFormatType.MPEG4AAC)
            {
                uint outputBitRate = 192000; // 192k

                // ignore errors as setting may be invalid depending on format specifics such as samplerate
                try
                {
                    converter.EncodeBitRate = outputBitRate;
                }
                catch
                {
                }

                // get it back and print it out
                outputBitRate = converter.EncodeBitRate;
                Debug.Print("AAC Encode Bitrate: {0}", outputBitRate);
            }

            // can the Audio Converter resume conversion after an interruption?
            // this property may be queried at any time after construction of the Audio Converter after setting its output format
            // there's no clear reason to prefer construction time, interruption time, or potential resumption time but we prefer
            // construction time since it means less code to execute during or after interruption time
            bool canResumeFromInterruption;
            try
            {
                canResumeFromInterruption = converter.CanResumeFromInterruption;
                Debug.Print("Audio Converter {0} continue after interruption!", canResumeFromInterruption ? "CAN" : "CANNOT");
            }
            catch (Exception e)
            {
                // if the property is unimplemented (kAudioConverterErr_PropertyNotSupported, or paramErr returned in the case of PCM),
                // then the codec being used is not a hardware codec so we're not concerned about codec state
                // we are always going to be able to resume conversion after an interruption

                canResumeFromInterruption = false;
                Debug.Print("CanResumeFromInterruption: {0}", e.Message);
            }

            // create the destination file
            var destinationFile = AudioFile.Create(destinationURL, AudioFileType.CAF, dstFormat, AudioFileFlags.EraseFlags);

            // set up source buffers and data proc info struct
            afio = new AudioFileIO(32 * 1024); // 32Kb
            afio.SourceFile = sourceFile;
            afio.SrcFormat = srcFormat;

            if (srcFormat.BytesPerPacket == 0)
            {
                // if the source format is VBR, we need to get the maximum packet size
                // use kAudioFilePropertyPacketSizeUpperBound which returns the theoretical maximum packet size
                // in the file (without actually scanning the whole file to find the largest packet,
                // as may happen with kAudioFilePropertyMaximumPacketSize)
                afio.SrcSizePerPacket = sourceFile.PacketSizeUpperBound;

                // how many packets can we read for our buffer size?
                afio.NumPacketsPerRead = afio.SrcBufferSize / afio.SrcSizePerPacket;

                // allocate memory for the PacketDescription structures describing the layout of each packet
                afio.PacketDescriptions = new AudioStreamPacketDescription [afio.NumPacketsPerRead];
            }
            else
            {
                // CBR source format
                afio.SrcSizePerPacket = srcFormat.BytesPerPacket;
                afio.NumPacketsPerRead = afio.SrcBufferSize / afio.SrcSizePerPacket;
            }

            // set up output buffers
            int outputSizePerPacket = dstFormat.BytesPerPacket; // this will be non-zero if the format is CBR
            const int theOutputBufSize = 32 * 1024; // 32Kb
            var outputBuffer = Marshal.AllocHGlobal(theOutputBufSize);
            AudioStreamPacketDescription[] outputPacketDescriptions = null;

            if (outputSizePerPacket == 0)
            {
                // if the destination format is VBR, we need to get max size per packet from the converter
                outputSizePerPacket = (int)converter.MaximumOutputPacketSize;

                // allocate memory for the PacketDescription structures describing the layout of each packet
                outputPacketDescriptions = new AudioStreamPacketDescription [theOutputBufSize / outputSizePerPacket];
            }
            int numOutputPackets = theOutputBufSize / outputSizePerPacket;

            // if the destination format has a cookie, get it and set it on the output file
            WriteCookie(converter, destinationFile);

            // write destination channel layout
            if (srcFormat.ChannelsPerFrame > 2)
            {
                WriteDestinationChannelLayout(converter, sourceFile, destinationFile);
            }

            long totalOutputFrames = 0; // used for debugging
            long outputFilePos = 0;
            AudioBuffers fillBufList = new AudioBuffers(1);
            bool error = false;

            // loop to convert data
            Debug.WriteLine("Converting...");
            while (true)
            {
                // set up output buffer list
                fillBufList[0] = new AudioBuffer()
                {
                    NumberChannels = dstFormat.ChannelsPerFrame,
                    DataByteSize = theOutputBufSize,
                    Data = outputBuffer
                };

                // this will block if we're interrupted
                var wasInterrupted = AppDelegate.ThreadStatePausedCheck();

                if (wasInterrupted && !canResumeFromInterruption)
                {
                    // this is our interruption termination condition
                    // an interruption has occured but the Audio Converter cannot continue
                    Debug.WriteLine("Cannot resume from interruption");
                    error = true;
                    break;
                }

                // convert data
                int ioOutputDataPackets = numOutputPackets;
                var fe = converter.FillComplexBuffer(ref ioOutputDataPackets, fillBufList, outputPacketDescriptions);
                // if interrupted in the process of the conversion call, we must handle the error appropriately
                if (fe != AudioConverterError.None)
                {
                    Debug.Print("FillComplexBuffer: {0}", fe);
                    error = true;
                    break;
                }

                if (ioOutputDataPackets == 0)
                {
                    // this is the EOF conditon
                    break;
                }

                // write to output file
                var inNumBytes = fillBufList[0].DataByteSize;

                var we = destinationFile.WritePackets(false, inNumBytes, outputPacketDescriptions, outputFilePos, ref ioOutputDataPackets, outputBuffer);
                if (we != 0)
                {
                    Debug.Print("WritePackets: {0}", we);
                    error = true;
                    break;
                }

                // advance output file packet position
                outputFilePos += ioOutputDataPackets;

                if (dstFormat.FramesPerPacket != 0)
                {
                    // the format has constant frames per packet
                    totalOutputFrames += (ioOutputDataPackets * dstFormat.FramesPerPacket);
                }
                else
                {
                    // variable frames per packet require doing this for each packet (adding up the number of sample frames of data in each packet)
                    for (var i = 0; i < ioOutputDataPackets; ++i)
                        totalOutputFrames += outputPacketDescriptions[i].VariableFramesInPacket;
                }

            }

            Marshal.FreeHGlobal(outputBuffer);

            if (!error)
            {
                // write out any of the leading and trailing frames for compressed formats only
                if (dstFormat.BitsPerChannel == 0)
                {
                    // our output frame count should jive with
                    Debug.Print("Total number of output frames counted: {0}", totalOutputFrames);
                    WritePacketTableInfo(converter, destinationFile);
                }

                // write the cookie again - sometimes codecs will update cookies at the end of a conversion
                WriteCookie(converter, destinationFile);
            }

            converter.Dispose();
            destinationFile.Dispose();
            sourceFile.Dispose();

            // transition thread state to State.Done before continuing
            AppDelegate.ThreadStateSetDone();

            return !error;
        }

        // Input data proc callback
        AudioConverterError EncoderDataProc(ref int numberDataPackets, AudioBuffers data, ref AudioStreamPacketDescription[] dataPacketDescription)
        {
            // figure out how much to read
            int maxPackets = afio.SrcBufferSize / afio.SrcSizePerPacket;
            if (numberDataPackets > maxPackets)
                numberDataPackets = maxPackets;

            // read from the file
            int outNumBytes = 16384;

            // modified for iOS7 (ReadPackets depricated)
			afio.PacketDescriptions = afio.SourceFile.ReadPacketData(false, afio.SrcFilePos, ref numberDataPackets, afio.SrcBuffer, ref outNumBytes);

			if (afio.PacketDescriptions.Length == 0 && numberDataPackets > 0)
				throw new ApplicationException(afio.PacketDescriptions.ToString());

            // advance input file packet position
            afio.SrcFilePos += numberDataPackets;

            // put the data pointer into the buffer list
            data.SetData(0, afio.SrcBuffer, outNumBytes);

            // don't forget the packet descriptions if required
            if (dataPacketDescription != null)
                dataPacketDescription = afio.PacketDescriptions;

            return AudioConverterError.None;
        }

        // Some audio formats have a magic cookie associated with them which is required to decompress audio data
        // When converting audio data you must check to see if the format of the data has a magic cookie
        // If the audio data format has a magic cookie associated with it, you must add this information to anAudio Converter
        // using AudioConverterSetProperty and kAudioConverterDecompressionMagicCookie to appropriately decompress the data
        // http://developer.apple.com/mac/library/qa/qa2001/qa1318.html
        static void ReadCookie(AudioFile sourceFile, AudioConverter converter)
        {
            // grab the cookie from the source file and set it on the converter
            var cookie = sourceFile.MagicCookie;

            // if there is an error here, then the format doesn't have a cookie - this is perfectly fine as some formats do not
            if (cookie != null && cookie.Length != 0)
            {
                converter.DecompressionMagicCookie = cookie;
            }
        }

        // Some audio formats have a magic cookie associated with them which is required to decompress audio data
        // When converting audio, a magic cookie may be returned by the Audio Converter so that it may be stored along with
        // the output data -- This is done so that it may then be passed back to the Audio Converter at a later time as required
        static void WriteCookie(AudioConverter converter, AudioFile destinationFile)
        {
            var cookie = converter.CompressionMagicCookie;
            if (cookie != null && cookie.Length != 0)
            {
                destinationFile.MagicCookie = cookie;
            }
        }

        // Write output channel layout to destination file
        static void WriteDestinationChannelLayout(AudioConverter converter, AudioFile sourceFile, AudioFile destinationFile)
        {
            // if the Audio Converter doesn't have a layout see if the input file does
            var layout = converter.OutputChannelLayout ?? sourceFile.ChannelLayout;

            if (layout != null)
            {
                // TODO:
                throw new NotImplementedException();
                //destinationFile.ChannelLayout = layout;
            }
        }

        // Sets the packet table containing information about the number of valid frames in a file and where they begin and end
        // for the file types that support this information.
        // Calling this function makes sure we write out the priming and remainder details to the destination file
        static void WritePacketTableInfo(AudioConverter converter, AudioFile destinationFile)
        {
            if (!destinationFile.IsPropertyWritable(AudioFileProperty.PacketTableInfo))
                return;

            // retrieve the leadingFrames and trailingFrames information from the converter,
            AudioConverterPrimeInfo primeInfo = converter.PrimeInfo;

            // we have some priming information to write out to the destination file
            // The total number of packets in the file times the frames per packet (or counting each packet's
            // frames individually for a variable frames per packet format) minus mPrimingFrames, minus
            // mRemainderFrames, should equal mNumberValidFrames.

            AudioFilePacketTableInfo? pti_n = destinationFile.PacketTableInfo;
            if (pti_n == null)
                return;

            AudioFilePacketTableInfo pti = pti_n.Value;

            // there's priming to write out to the file
            // get the total number of frames from the output file
            long totalFrames = pti.ValidFrames + pti.PrimingFrames + pti.RemainderFrames;
            Debug.WriteLine("Total number of frames from output file: {0}", totalFrames);

            pti.PrimingFrames = primeInfo.LeadingFrames;
            pti.RemainderFrames = primeInfo.TrailingFrames;
            pti.ValidFrames = totalFrames - pti.PrimingFrames - pti.RemainderFrames;

            destinationFile.PacketTableInfo = pti;
        }

        AVAudioPlayer player;

        void playAudio()
        {
            Debug.WriteLine("Playing converted audio...");

            UpdateFormatInfo(fileInfo, destinationURL);
            startButton.SetTitle("Playing Output File...", UIControlState.Disabled);

            // set category back to something that will allow us to play audio since kAudioSessionCategory_AudioProcessing will not
            AudioSession.Category = AudioSessionCategory.MediaPlayback;

            // play the result
            NSError error;
            player = AVAudioPlayer.FromUrl(destinationURL, out error);
            if (player == null)
            {
                Debug.Print("AVAudioPlayer failed: {0}", error);
                UpdateUI();
                return;
            }

            player.DecoderError += delegate
            {
                Debug.WriteLine("DecoderError");
                UpdateUI();
            };

            player.BeginInterruption += delegate
            {
                Debug.WriteLine("BeginInterruption");
                player.Stop();
                UpdateUI();
            };

            player.FinishedPlaying += delegate(object sender, AVStatusEventArgs e)
            {
                Debug.WriteLine("FinishedPlaying");
                UpdateUI();
                player = null;
            };

            if (!player.Play())
            {
                Debug.WriteLine("ERROR: Cannot play the file");
                UpdateUI();
                player = null;
            }
        }
    }
}

