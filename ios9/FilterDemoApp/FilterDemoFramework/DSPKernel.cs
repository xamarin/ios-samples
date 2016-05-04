using System;

using AudioToolbox;
using AudioUnit;

namespace FilterDemoFramework {
	public abstract class DSPKernel {
		void HandleOneEvent (AURenderEvent theEvent)
		{
			switch (theEvent.Head.EventType) {
			case AURenderEventType.Parameter:
			case AURenderEventType.ParameterRamp:
				AUParameterEvent paramEvent = theEvent.Parameter;

				StartRamp (paramEvent.ParameterAddress, paramEvent.Value, (int)paramEvent.RampDurationSampleFrames);
				break;
			case AURenderEventType.Midi:
				throw new NotImplementedException ();
			}
		}

// There are two APIs for getting all of the events.
// - The "raw" pointers from the linked list
// - The EnumeratorCurrentEvents wrapper
//#define USE_RAW_EVENT_ENUMERATION
#if USE_RAW_EVENT_ENUMERATION
		unsafe void PerformAllSimultaneousEvents (nint now, AURenderEvent** theEvent)
		{
			do {
				HandleOneEvent (**theEvent);
				*theEvent = (*theEvent)->Head.UnsafeNext;
			} while (*theEvent != null && ((*theEvent)->Head.EventSampleTime == now));
		}

		public unsafe void ProcessWithEvents (AudioTimeStamp timestamp, int frameCount, AURenderEventEnumerator events)
		{
			var now = (nint)timestamp.SampleTime;
			int framesRemaining = frameCount;
			AURenderEvent* theEvent = events.UnsafeFirst;
			while (framesRemaining > 0) {
				if (theEvent == null) {
					int bufferOffset = frameCount - framesRemaining;
					Process (framesRemaining, bufferOffset);
					return;
				}

				int framesThisSegment = (int)(theEvent->Head.EventSampleTime - now);

				if (framesThisSegment > 0) {
					int bufferOffset = frameCount - framesRemaining;
					Process (framesThisSegment, bufferOffset);
					framesRemaining -= framesThisSegment;
					now += framesThisSegment;
				}

				PerformAllSimultaneousEvents (now, &theEvent);
			}
		}
#else
		public unsafe void ProcessWithEvents (AudioTimeStamp timestamp, int frameCount, AURenderEventEnumerator events)
		{
			var now = (nint)timestamp.SampleTime;
			int framesRemaining = frameCount;
			while (framesRemaining > 0) {
				if (events.IsAtEnd) {
					int bufferOffset = frameCount - framesRemaining;
					Process (framesRemaining, bufferOffset);
					return;
				}

				var framesThisSegment = (int)(events.Current.Head.EventSampleTime - now);

				if (framesThisSegment > 0) {
					int bufferOffset = frameCount - framesRemaining;
					Process (framesThisSegment, bufferOffset);
					framesRemaining -= framesThisSegment;
					now += framesThisSegment;
				}

				foreach (AURenderEvent e in events.EnumeratorCurrentEvents (now))
					HandleOneEvent (e);
				events.MoveNext ();
			}
		}

#endif

		public abstract void Process (int frameCount, int bufferOffset);

		public abstract void StartRamp (ulong address, float value, int duration);
	}
}

