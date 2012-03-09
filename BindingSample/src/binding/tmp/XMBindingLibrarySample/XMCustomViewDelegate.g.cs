//
// Auto-generated from generator.cs, do not edit
//
// We keep references to objects, so warning 414 is expected

#pragma warning disable 414

using System;

using System.Drawing;

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
	[Register("XMCustomViewDelegate", true)]
	[Model]
	public abstract partial class XMCustomViewDelegate : NSObject {
		
		static IntPtr class_ptr = Class.GetHandle ("NSObject");
		
		[Export ("init")]
		public  XMCustomViewDelegate () : base (NSObjectFlag.Empty)
		{
			IsDirectBinding = GetType ().Assembly == global::XMBindingLibrarySample.Messaging.this_assembly;
			if (IsDirectBinding) {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.Init);
			} else {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, Selector.Init);
			}
		}

		[Export ("initWithCoder:")]
		public XMCustomViewDelegate (NSCoder coder) : base (NSObjectFlag.Empty)
		{
			IsDirectBinding = GetType ().Assembly == global::XMBindingLibrarySample.Messaging.this_assembly;
			if (IsDirectBinding) {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.InitWithCoder, coder.Handle);
			} else {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.InitWithCoder, coder.Handle);
			}
		}

		public XMCustomViewDelegate (NSObjectFlag t) : base (t) {}

		public XMCustomViewDelegate (IntPtr handle) : base (handle) {}

		[Export ("viewWasTouched:")]
		public abstract void ViewWasTouched (XMCustomView view);
	} /* class XMCustomViewDelegate */
}
