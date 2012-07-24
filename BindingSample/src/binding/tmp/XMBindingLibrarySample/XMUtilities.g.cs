//
// Auto-generated from generator.cs, do not edit
//
// We keep references to objects, so warning 414 is expected

#pragma warning disable 414

using System;

using System.Drawing;

using System.Runtime.CompilerServices;

using System.Runtime.InteropServices;

using MonoTouch;

using MonoTouch.CoreFoundation;

using MonoTouch.CoreMedia;

using MonoTouch.CoreMotion;

using MonoTouch.Foundation;

using MonoTouch.ObjCRuntime;

using MonoTouch.CoreAnimation;

using MonoTouch.CoreLocation;

using MonoTouch.MapKit;

using MonoTouch.UIKit;

using MonoTouch.CoreGraphics;

using MonoTouch.NewsstandKit;

using MonoTouch.GLKit;

using OpenTK;

namespace XMBindingLibrarySample {
	[Register("XMUtilities", true)]
	public partial class XMUtilities : NSObject {
		static IntPtr selEcho_ = Selector.GetHandle ("echo:");
		static IntPtr selHello_ = Selector.GetHandle ("hello:");
		static IntPtr selAddAnd_ = Selector.GetHandle ("add:and:");
		static IntPtr selMultiplyAnd = Selector.GetHandle ("multiply:and");
		static IntPtr selSetCallback_ = Selector.GetHandle ("setCallback:");
		static IntPtr selInvokeCallback_ = Selector.GetHandle ("invokeCallback:");
		
		static IntPtr class_ptr = Class.GetHandle ("XMUtilities");
		
		public override IntPtr ClassHandle { get { return class_ptr; } }
		
		[CompilerGenerated]
		[Export ("init")]
		public  XMUtilities () : base (NSObjectFlag.Empty)
		{
			IsDirectBinding = GetType ().Assembly == global::XMBindingLibrarySample.Messaging.this_assembly;
			if (IsDirectBinding) {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.Init);
			} else {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, Selector.Init);
			}
		}

		[CompilerGenerated]
		[Export ("initWithCoder:")]
		public XMUtilities (NSCoder coder) : base (NSObjectFlag.Empty)
		{
			IsDirectBinding = GetType ().Assembly == global::XMBindingLibrarySample.Messaging.this_assembly;
			if (IsDirectBinding) {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.InitWithCoder, coder.Handle);
			} else {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.InitWithCoder, coder.Handle);
			}
		}

		[CompilerGenerated]
		public XMUtilities (NSObjectFlag t) : base (t) {}

		[CompilerGenerated]
		public XMUtilities (IntPtr handle) : base (handle) {}

		[Export ("echo:")]
		[CompilerGenerated]
		public static string Echo (string message)
		{
			if (message == null)
				throw new ArgumentNullException ("message");
			var nsmessage = new NSString (message);
			
			string ret;
			ret = NSString.FromHandle (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (class_ptr, selEcho_, nsmessage.Handle));
			nsmessage.Dispose ();
			
			return ret;
		}
		
		[Export ("hello:")]
		[CompilerGenerated]
		public virtual string Hello (string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			var nsname = new NSString (name);
			
			string ret;
			if (IsDirectBinding) {
				ret = NSString.FromHandle (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, selHello_, nsname.Handle));
			} else {
				ret = NSString.FromHandle (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, selHello_, nsname.Handle));
			}
			nsname.Dispose ();
			
			return ret;
		}
		
		[Export ("add:and:")]
		[CompilerGenerated]
		public virtual int Add (int operandUn, int operandDeux)
		{
			if (IsDirectBinding) {
				return XMBindingLibrarySample.Messaging.int_objc_msgSend_int_int (this.Handle, selAddAnd_, operandUn, operandDeux);
			} else {
				return XMBindingLibrarySample.Messaging.int_objc_msgSendSuper_int_int (this.SuperHandle, selAddAnd_, operandUn, operandDeux);
			}
		}
		
		[Export ("multiply:and")]
		[CompilerGenerated]
		public virtual int Multiply (int operandUn, int operandDeux)
		{
			if (IsDirectBinding) {
				return XMBindingLibrarySample.Messaging.int_objc_msgSend_int_int (this.Handle, selMultiplyAnd, operandUn, operandDeux);
			} else {
				return XMBindingLibrarySample.Messaging.int_objc_msgSendSuper_int_int (this.SuperHandle, selMultiplyAnd, operandUn, operandDeux);
			}
		}
		
		[Export ("setCallback:")]
		[CompilerGenerated]
		public unsafe virtual void SetCallback (XMUtilityCallback callback)
		{
			if (callback == null)
				throw new ArgumentNullException ("callback");
			BlockLiteral *block_ptr_callback;
			BlockLiteral block_callback;
			block_callback = new BlockLiteral ();
			block_ptr_callback = &block_callback;
			block_callback.SetupBlock (static_InnerXMUtilityCallback, callback);
			
			if (IsDirectBinding) {
				MonoTouch.ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selSetCallback_, (IntPtr) block_ptr_callback);
			} else {
				MonoTouch.ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selSetCallback_, (IntPtr) block_ptr_callback);
			}
			block_ptr_callback->CleanupBlock ();
			
		}
		
		[Export ("invokeCallback:")]
		[CompilerGenerated]
		public virtual void InvokeCallback (NSString message)
		{
			if (message == null)
				throw new ArgumentNullException ("message");
			if (IsDirectBinding) {
				MonoTouch.ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selInvokeCallback_, message.Handle);
			} else {
				MonoTouch.ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selInvokeCallback_, message.Handle);
			}
		}
		
		internal delegate void InnerXMUtilityCallback (IntPtr block, IntPtr message);
		static InnerXMUtilityCallback static_InnerXMUtilityCallback = new InnerXMUtilityCallback (TrampolineXMUtilityCallback);
		[MonoPInvokeCallback (typeof (InnerXMUtilityCallback))]
		static unsafe void TrampolineXMUtilityCallback (IntPtr block, IntPtr message) {
			var descriptor = (BlockLiteral *) block;
			var del = (XMBindingLibrarySample.XMUtilityCallback) (descriptor->global_handle != IntPtr.Zero ? GCHandle.FromIntPtr (descriptor->global_handle).Target : GCHandle.FromIntPtr (descriptor->local_handle).Target);
			del ((MonoTouch.Foundation.NSString) Runtime.GetNSObject (message));
		}
		
	} /* class XMUtilities */
	public delegate void XMUtilityCallback (NSString message);
}
