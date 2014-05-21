using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


using AVFoundation;
using Foundation;
using UIKit;
using MonoTouch.Dialog;
using AudioToolbox;
using AudioUnit;

using CoreFoundation;

using AudioFileID = System.IntPtr;

namespace AudioQueueOfflineRenderDemo
{
	[Register ("AppDelegate")]
	public unsafe partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		DialogViewController dvc;
		StyledStringElement element;
		bool busy;
		AVAudioPlayer player;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			dvc = new DialogViewController (new RootElement ("Audio Queue Offline Render Demo") {
				new Section ("Audio Queue Offline Render Demo") {
				(element = new StyledStringElement ("Render audio", RenderAudioAsync) { Alignment = UITextAlignment.Center }),
				}
			});
			
			element.Alignment = UITextAlignment.Center;
			
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = dvc;
			window.MakeKeyAndVisible ();
			
			return true;
		}
		
		void SetCaption (string caption)
		{
			BeginInvokeOnMainThread (() =>
			{
				element.Caption = caption;
				dvc.ReloadData ();
			});
		}
		
		void RenderAudioAsync ()
		{
			CFUrl sourceUrl = CFUrl.FromFile (NSBundle.MainBundle.PathForResource ("composeaudio", "mp3"));
			CFUrl destinationUrl = CFUrl.FromFile (System.IO.Path.Combine (System.Environment.GetFolderPath (Environment.SpecialFolder.Personal), "output.caf"));
			
			if (busy)
				return;

			busy = true;
			SetCaption ("Rendering...");
			
			var thread = new System.Threading.Thread (() =>
			{
				try {
					RenderAudio (sourceUrl, destinationUrl);
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
				BeginInvokeOnMainThread (() =>
				{
					SetCaption ("Playing...");

					using (var playUrl = new NSUrl (destinationUrl.Handle)) {
						player = AVAudioPlayer.FromUrl (playUrl);
					}
					
					player.Play ();
					player.FinishedPlaying += (sender, e) =>
					{
						BeginInvokeOnMainThread (() =>
						{
							player.Dispose ();
							player = null;
							SetCaption ("Render audio");
							busy = false;
						});
					};
				});
			});
			thread.IsBackground = true;
			thread.Start ();
		}

		static void CalculateBytesForTime (AudioStreamBasicDescription desc, int maxPacketSize, double seconds, out int bufferSize, out int packetCount)
		{
			const int maxBufferSize = 0x10000;
			const int minBufferSize = 0x4000;
			
			if (desc.FramesPerPacket > 0) {
				bufferSize = (int) (desc.SampleRate / desc.FramesPerPacket * seconds * maxPacketSize);
			} else {
				bufferSize = maxBufferSize > maxPacketSize ? maxBufferSize : maxPacketSize;
			}
			
			if (bufferSize > maxBufferSize && bufferSize > maxPacketSize) {
				bufferSize = maxBufferSize;
			} else if (bufferSize < minBufferSize) {
				bufferSize = minBufferSize;
			}
			
			packetCount = bufferSize / maxPacketSize;
		}
		
		unsafe static void HandleOutput (AudioFile audioFile, AudioQueue queue, AudioQueueBuffer *audioQueueBuffer, ref int packetsToRead, ref long currentPacket, ref bool done, ref bool flushed, ref AudioStreamPacketDescription [] packetDescriptions)
		{
			int bytes;
			int packets;
			
			if (done)
				return;
			
			packets = packetsToRead;
			bytes = (int) audioQueueBuffer->AudioDataBytesCapacity;

			packetDescriptions = audioFile.ReadPacketData (false, currentPacket, ref packets, audioQueueBuffer->AudioData, ref bytes);
			
			if (packets > 0) {
				audioQueueBuffer->AudioDataByteSize = (uint) bytes;
				queue.EnqueueBuffer (audioQueueBuffer, packetDescriptions);
				currentPacket += packets;
			} else {
				if (!flushed) {
					queue.Flush ();
					flushed = true;
				}
				
				queue.Stop (false);
				done = true;
			}
		}
		
		unsafe static void RenderAudio (CFUrl sourceUrl, CFUrl destinationUrl)
		{
			AudioStreamBasicDescription dataFormat;
			AudioQueueBuffer *buffer = null;
			long currentPacket = 0;
			int packetsToRead = 0;
			AudioStreamPacketDescription [] packetDescs = null;
			bool flushed = false;
			bool done = false;
			int bufferSize;
			
			using (var audioFile = AudioFile.Open (sourceUrl, AudioFilePermission.Read, (AudioFileType) 0)) {
				dataFormat = audioFile.StreamBasicDescription;
				
				using (var queue = new OutputAudioQueue (dataFormat, CFRunLoop.Current, CFRunLoop.ModeCommon)) {
					queue.OutputCompleted += (sender, e) => 
					{
						HandleOutput (audioFile, queue, buffer, ref packetsToRead, ref currentPacket, ref done, ref flushed, ref packetDescs);
					};
					
					// we need to calculate how many packets we read at a time and how big a buffer we need
					// we base this on the size of the packets in the file and an approximate duration for each buffer
					bool isVBR = dataFormat.BytesPerPacket == 0 || dataFormat.FramesPerPacket == 0;
					
					// first check to see what the max size of a packet is - if it is bigger
					// than our allocation default size, that needs to become larger
					// adjust buffer size to represent about a second of audio based on this format 
					CalculateBytesForTime (dataFormat, audioFile.MaximumPacketSize, 1.0, out bufferSize, out packetsToRead);
				
					if (isVBR) {
						packetDescs = new AudioStreamPacketDescription [packetsToRead];
					} else {
						packetDescs = null; // we don't provide packet descriptions for constant bit rate formats (like linear PCM)
					}
				
					if (audioFile.MagicCookie.Length != 0)
						queue.MagicCookie = audioFile.MagicCookie;
		
					// allocate the input read buffer
					queue.AllocateBuffer (bufferSize, out buffer);
					
					// prepare the capture format
					var captureFormat = AudioStreamBasicDescription.CreateLinearPCM (dataFormat.SampleRate, (uint) dataFormat.ChannelsPerFrame, 32);
					captureFormat.BytesPerFrame = captureFormat.BytesPerPacket = dataFormat.ChannelsPerFrame * 4;

					queue.SetOfflineRenderFormat (captureFormat, audioFile.ChannelLayout);
					
					// prepare the target format
					var dstFormat = AudioStreamBasicDescription.CreateLinearPCM (dataFormat.SampleRate, (uint) dataFormat.ChannelsPerFrame);

					using (var captureFile = ExtAudioFile.CreateWithUrl (destinationUrl, AudioFileType.CAF, dstFormat, AudioFileFlags.EraseFlags)) {
						captureFile.ClientDataFormat = captureFormat;
						
						int captureBufferSize = bufferSize / 2;
						AudioBuffers captureABL = new AudioBuffers (1);
						
						AudioQueueBuffer *captureBuffer;
						queue.AllocateBuffer (captureBufferSize, out captureBuffer);
						
						captureABL[0] = new AudioBuffer () {
							Data = captureBuffer->AudioData,
							NumberChannels = captureFormat.ChannelsPerFrame
						};

						queue.Start ();

						double ts = 0;
						queue.RenderOffline (ts, captureBuffer, 0);
						
						HandleOutput (audioFile, queue, buffer, ref packetsToRead, ref currentPacket, ref done, ref flushed, ref packetDescs);
						
						while (true) {
							int reqFrames = captureBufferSize / captureFormat.BytesPerFrame;
							
							queue.RenderOffline (ts, captureBuffer, reqFrames);

							captureABL.SetData (0, captureBuffer->AudioData, (int) captureBuffer->AudioDataByteSize);
							var writeFrames = captureABL[0].DataByteSize / captureFormat.BytesPerFrame;
							
							// Console.WriteLine ("ts: {0} AudioQueueOfflineRender: req {1} frames / {2} bytes, got {3} frames / {4} bytes", 
							//	ts, reqFrames, captureBufferSize, writeFrames, captureABL.Buffers [0].DataByteSize);
							
							captureFile.WriteAsync ((uint) writeFrames, captureABL);
							
							if (flushed)
								break;
							
							ts += writeFrames;
						}
						CFRunLoop.Current.RunInMode (CFRunLoop.ModeDefault, 1, false);
					}
				}
			}
		}
	}
}

