using System;
using System.Runtime.InteropServices;

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;

namespace InputStreamTest
{
	[Preserve (AllMembers = true)]
	public class ZInputStream : NSInputStream
	{
		NSStreamStatus status;
		long read_length;
		long total_length;
		
		public ZInputStream (long total_length) : base ()
		{
			status = NSStreamStatus.NotOpen;
			this.total_length = total_length;
		}
		
		public override NSStreamStatus Status {
			get {
				return status;
			}
		}
		
		public override void Open ()
		{
			status = NSStreamStatus.Open;
			Notify (CFStreamEventType.OpenCompleted);
		}
		
		public override void Close ()
		{
			status = NSStreamStatus.Closed;
		}
		
		public override bool HasBytesAvailable ()
		{
			return total_length > read_length;
		}
	
		public override int Read (IntPtr buffer, uint len) 
		{
			int actual = Math.Min ((int) len, (int) (total_length - read_length));
			
			byte [] bytes = new byte [actual];
			for (int i = 0; i < actual; i++)
				bytes [i] = (byte) 'z';
			
			read_length += actual;
			
			Marshal.Copy (bytes, 0, buffer, actual);
			
			if (actual == 0)
				Notify (CFStreamEventType.EndEncountered);
			
			return actual;
		}
		
		protected unsafe override bool GetBuffer (out IntPtr buffer, out uint len)
		{
			// Just call the base implemention (which will return false)
			return base.GetBuffer (out buffer, out len);
		}
		
		protected override bool SetCFClientFlags (CFStreamEventType inFlags, IntPtr inCallback, IntPtr inContextPtr)
		{
			// Just call the base implementation, which knows how to handle everything.
			return base.SetCFClientFlags (inFlags, inCallback, inContextPtr);
		}
		
		public override void ScheduleInCFRunLoop (CFRunLoop runloop, NSString mode)
		{
			Notify (CFStreamEventType.HasBytesAvailable);
		}
		
		public override void UnscheduleInCFRunLoop (CFRunLoop runloop, NSString mode)
		{
			// Nothing to do here
		}
	}
}
