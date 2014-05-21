//
// CCFWebRadioPlayer.cs: AudioQueueTap web based player
//
// Authors:
//   Chris Adamson (cadamson@subfurther.com)
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
using Foundation;
using AudioToolbox;
using CoreFoundation;
using AudioUnit;
using System.Diagnostics;

namespace AQTapDemo
{
	public class CCFWebRadioPlayer
	{
		class ConnectionDelegate : NSUrlConnectionDelegate
		{
			AudioFileStream audioFileStream;

			public ConnectionDelegate (AudioFileStream stream)
			{
				audioFileStream = stream;
			}

			public override void ReceivedData (NSUrlConnection connection, NSData data)
			{
				if (audioFileStream.ParseBytes ((int) data.Length, data.Bytes, false) != AudioFileStreamStatus.Ok)
					throw new ApplicationException ();
			}
		}

		AudioFileStream audioFileStream;
		AudioStreamBasicDescription dataFormat;
		OutputAudioQueue audioQueue;
		AudioQueueProcessingTap aqTap;
		IntPtr preRenderData;

	
		AudioUnit.AudioUnit genericOutputUnit;
		AudioUnit.AudioUnit effectUnit;
		AUGraph auGraph;

		int totalPacketsReceived;

		public CCFWebRadioPlayer (NSUrl stationURL)
		{
			this.StationURL = stationURL;
			Console.WriteLine ("URL: " + stationURL);
		}

		public NSUrl StationURL { get; private set; }

		public void SetPitch (float value)
		{
			if (effectUnit == null)
				return;
			Console.WriteLine ("Setting parameter PitchPitch: "+value);
			effectUnit.SetParameter (AudioUnitParameterType.NewTimePitchPitch, value, AudioUnitScopeType.Global);
		}

		public void Start ()
		{
			if (StationURL == null)
				return;

			audioFileStream = new AudioFileStream (AudioFileType.MP3);
			audioFileStream.PropertyFound += StreamPropertyListenerProc;
			audioFileStream.PacketDecoded += StreamPacketsProc;

			var request = new NSUrlRequest (StationURL);
			new NSUrlConnection (request, new ConnectionDelegate (audioFileStream));
		}

		void StreamPropertyListenerProc (object sender, PropertyFoundEventArgs args)
		{
			if (args.Property == AudioFileStreamProperty.DataFormat) {
				dataFormat = audioFileStream.DataFormat;
				return;
			}

			if (args.Property != AudioFileStreamProperty.ReadyToProducePackets) 
				return;

			if (audioQueue != null) {
				// TODO: Dispose
				throw new NotImplementedException ();
			}

			audioQueue = new OutputAudioQueue (dataFormat);
			audioQueue.OutputCompleted += HandleOutputCompleted;

			AudioQueueStatus status;
			aqTap = audioQueue.CreateProcessingTap (TapProc, AudioQueueProcessingTapFlags.PreEffects, out status);
			if (status != AudioQueueStatus.Ok)
				throw new ApplicationException ("Could not create AQ tap");

			// create an augraph to process in the tap. needs to convert from tapFormat to effect format and back
			/* note: this is invalidname's recipe to do an in-place effect when a format conversion is needed
			before and after the effect, usually because effects want floats, and everything else in iOS
			core audio works with ints (or, in rare cases, fixed-point).
			the graph looks like this:
			[render-callback] -> [converter] -> [effect] -> [converter] -> [generic-output]
			prior to calling AudioUnitRender() on generic-output the ioData to a pointer that render-callback
			knows about, and NULLs the ioData provided to AudioUnitRender(). the NULL tells generic-output to
			pull from its upstream units (ie, the augraph), and copying off the ioData pointer allows the
			render-callback	to provide it to the front of the stream. in some locales, this kind of shell game
			is described as "batshit crazy", but it seems to work pretty well in practice.
			*/

			auGraph = new AUGraph ();
			auGraph.Open ();
			var effectNode = auGraph.AddNode (AudioComponentDescription.CreateConverter (AudioTypeConverter.NewTimePitch));
			effectUnit = auGraph.GetNodeInfo (effectNode);

			var convertToEffectNode = auGraph.AddNode (AudioComponentDescription.CreateConverter (AudioTypeConverter.AU));
			var convertToEffectUnit = auGraph.GetNodeInfo (convertToEffectNode);

			var convertFromEffectNode = auGraph.AddNode (AudioComponentDescription.CreateConverter (AudioTypeConverter.AU));
			var convertFromEffectUnit = auGraph.GetNodeInfo (convertFromEffectNode);

			var genericOutputNode = auGraph.AddNode (AudioComponentDescription.CreateOutput (AudioTypeOutput.Generic));
			genericOutputUnit = auGraph.GetNodeInfo (genericOutputNode);

			// set the format conversions throughout the graph
			var effectFormat = effectUnit.GetAudioFormat (AudioUnitScopeType.Output);
			var tapFormat = aqTap.ProcessingFormat;

			convertToEffectUnit.SetAudioFormat (tapFormat, AudioUnitScopeType.Input);
			convertToEffectUnit.SetAudioFormat (effectFormat, AudioUnitScopeType.Output);

			convertFromEffectUnit.SetAudioFormat (effectFormat, AudioUnitScopeType.Input);
			convertFromEffectUnit.SetAudioFormat (tapFormat, AudioUnitScopeType.Output);

			genericOutputUnit.SetAudioFormat (tapFormat, AudioUnitScopeType.Input);
			genericOutputUnit.SetAudioFormat (tapFormat, AudioUnitScopeType.Output);

			// set maximum fames per slice higher (4096) so we don't get kAudioUnitErr_TooManyFramesToProcess
			const uint maxFramesPerSlice = 4096;
			if (convertToEffectUnit.SetMaximumFramesPerSlice (maxFramesPerSlice, AudioUnitScopeType.Global) != AudioUnitStatus.OK)
				throw new ApplicationException ();
			if (effectUnit.SetMaximumFramesPerSlice (maxFramesPerSlice, AudioUnitScopeType.Global) != AudioUnitStatus.OK)
				throw new ApplicationException ();
			if (convertFromEffectUnit.SetMaximumFramesPerSlice (maxFramesPerSlice, AudioUnitScopeType.Global) != AudioUnitStatus.OK)
				throw new ApplicationException ();
			if (genericOutputUnit.SetMaximumFramesPerSlice (maxFramesPerSlice, AudioUnitScopeType.Global) != AudioUnitStatus.OK)
				throw new ApplicationException ();

			// connect the nodes
			auGraph.ConnnectNodeInput (convertToEffectNode, 0, effectNode, 0);
			auGraph.ConnnectNodeInput (effectNode, 0, convertFromEffectNode, 0);
			auGraph.ConnnectNodeInput (convertFromEffectNode, 0, genericOutputNode, 0);

			// set up the callback into the first convert unit
			if (convertToEffectUnit.SetRenderCallback (ConvertInputRenderCallback, AudioUnitScopeType.Global) != AudioUnitStatus.NoError)
				throw new ApplicationException ();

			var res = auGraph.Initialize ();
			if (res != AUGraphError.OK)
				throw new ApplicationException ();
		}

		void HandleOutputCompleted (object sender, OutputCompletedEventArgs e)
		{
			audioQueue.FreeBuffer (e.IntPtrBuffer);
		}

		uint TapProc (AudioQueueProcessingTap audioQueueTap, uint numberOfFrames, ref AudioTimeStamp timeStamp, ref AudioQueueProcessingTapFlags flags, AudioBuffers data)
		{
			AudioQueueProcessingTapFlags source_flags;
			uint source_frames;

			if (audioQueueTap.GetSourceAudio (numberOfFrames, ref timeStamp, out source_flags, out source_frames, data) != AudioQueueStatus.Ok)
				throw new ApplicationException ();

			preRenderData = data [0].Data;
			data.SetData (0, IntPtr.Zero);

			var renderTimeStamp = new AudioTimeStamp ();
			renderTimeStamp.Flags = AudioTimeStamp.AtsFlags.SampleTimeValid;
			AudioUnitRenderActionFlags action_flags = 0;

			var res = genericOutputUnit.Render (ref action_flags, renderTimeStamp, 0, numberOfFrames, data);
			Console.WriteLine (res);
			if (res != AudioUnitStatus.NoError)
				throw new ApplicationException ();

			return source_frames;
		}

		AudioUnitStatus ConvertInputRenderCallback (AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, AudioBuffers data)
		{
			data.SetData (0, preRenderData);
			return AudioUnitStatus.NoError;
		}

		void StreamPacketsProc (object sender, PacketReceivedEventArgs args)
		{
			//Debug.WriteLine ("{0} packets", args.PacketDescriptions.Length);

			unsafe {
				AudioQueueBuffer* buffer;
				if (audioQueue.AllocateBuffer (args.Bytes, out buffer) != AudioQueueStatus.Ok)
					throw new ApplicationException ();

				buffer->AudioDataByteSize = (uint)args.Bytes;
				buffer->CopyToAudioData (args.InputData, args.Bytes);

				if (audioQueue.EnqueueBuffer (buffer, args.PacketDescriptions) != AudioQueueStatus.Ok)
					throw new ApplicationException ();
			}

			totalPacketsReceived += args.PacketDescriptions.Length;

			if (!audioQueue.IsRunning && totalPacketsReceived > 100) {
				var res = audioQueue.Start ();
				if (res != AudioQueueStatus.Ok)
					throw new ApplicationException (res.ToString ());
			}
		}
	}
}

