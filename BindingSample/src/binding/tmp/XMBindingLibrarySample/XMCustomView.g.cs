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
	[Register("XMCustomView", true)]
	public partial class XMCustomView : MonoTouch.UIKit.UIView {
		static IntPtr selName = Selector.GetHandle ("name");
		static IntPtr selSetName_ = Selector.GetHandle ("setName:");
		static IntPtr selDelegate = Selector.GetHandle ("delegate");
		static IntPtr selSetDelegate_ = Selector.GetHandle ("setDelegate:");
		static IntPtr selCustomizeViewWithText_ = Selector.GetHandle ("customizeViewWithText:");
		
		static IntPtr class_ptr = Class.GetHandle ("XMCustomView");
		
		public override IntPtr ClassHandle { get { return class_ptr; } }
		
		[CompilerGenerated]
		[Export ("init")]
		public  XMCustomView () : base (NSObjectFlag.Empty)
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
		public XMCustomView (NSCoder coder) : base (NSObjectFlag.Empty)
		{
			IsDirectBinding = GetType ().Assembly == global::XMBindingLibrarySample.Messaging.this_assembly;
			if (IsDirectBinding) {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.InitWithCoder, coder.Handle);
			} else {
				Handle = MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.InitWithCoder, coder.Handle);
			}
		}

		[CompilerGenerated]
		public XMCustomView (NSObjectFlag t) : base (t) {}

		[CompilerGenerated]
		public XMCustomView (IntPtr handle) : base (handle) {}

		[Export ("customizeViewWithText:")]
		[CompilerGenerated]
		public virtual void CustomizeViewWithText (string message)
		{
			if (message == null)
				throw new ArgumentNullException ("message");
			var nsmessage = new NSString (message);
			
			if (IsDirectBinding) {
				MonoTouch.ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selCustomizeViewWithText_, nsmessage.Handle);
			} else {
				MonoTouch.ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selCustomizeViewWithText_, nsmessage.Handle);
			}
			nsmessage.Dispose ();
			
		}
		
		[CompilerGenerated]
		public virtual string Name {
			[Export ("name")]
			get {
				if (IsDirectBinding) {
					return NSString.FromHandle (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, selName));
				} else {
					return NSString.FromHandle (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, selName));
				}
			}
			
			[Export ("setName:")]
			set {
				var nsvalue = value == null ? null : new NSString (value);
				
				if (IsDirectBinding) {
					MonoTouch.ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selSetName_, nsvalue == null ? IntPtr.Zero : nsvalue.Handle);
				} else {
					MonoTouch.ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selSetName_, nsvalue == null ? IntPtr.Zero : nsvalue.Handle);
				}
				if (nsvalue != null)
					nsvalue.Dispose ();
			}
		}
		
		object __mt_WeakDelegate_var;
		[CompilerGenerated]
		public virtual NSObject WeakDelegate {
			[Export ("delegate", ArgumentSemantic.Assign)]
			get {
				NSObject ret;
				if (IsDirectBinding) {
					ret = (NSObject) Runtime.GetNSObject (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, selDelegate));
				} else {
					ret = (NSObject) Runtime.GetNSObject (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, selDelegate));
				}
				MarkDirty ();
				__mt_WeakDelegate_var = ret;
				return ret;
			}
			
			[Export ("setDelegate:", ArgumentSemantic.Assign)]
			set {
				if (IsDirectBinding) {
					MonoTouch.ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selSetDelegate_, value == null ? IntPtr.Zero : value.Handle);
				} else {
					MonoTouch.ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selSetDelegate_, value == null ? IntPtr.Zero : value.Handle);
				}
				MarkDirty ();
				__mt_WeakDelegate_var = value;
			}
		}
		
		[CompilerGenerated]
		public XMCustomViewDelegate Delegate {
			get { return WeakDelegate as XMCustomViewDelegate; }
			set { WeakDelegate = value; }
		}
		
		//
		// Events and properties from the delegate
		//
		
		_XMCustomViewDelegate EnsureXMCustomViewDelegate ()
		{
			var del = WeakDelegate;
			if (del == null || (!(del is _XMCustomViewDelegate))){
				del = new _XMCustomViewDelegate ();
				WeakDelegate = del;
			}
			return (_XMCustomViewDelegate) del;
		}
		
		[Register]
		class _XMCustomViewDelegate : XMBindingLibrarySample.XMCustomViewDelegate { 
			public _XMCustomViewDelegate () {}
			
			internal EventHandler viewWasTouched;
			[Preserve (Conditional = true)]
			public override Void ViewWasTouched (XMBindingLibrarySample.XMCustomView view)
			{
				if (viewWasTouched != null){
					viewWasTouched (view, EventArgs.Empty);
				}
			}
			
		}
		
		public event EventHandler ViewWasTouched {
			add { EnsureXMCustomViewDelegate ().viewWasTouched += value; }
			remove { EnsureXMCustomViewDelegate ().viewWasTouched -= value; }
		}
		
		protected override void Dispose (bool disposing)
		{
			__mt_WeakDelegate_var = null;
			base.Dispose (disposing);
		}
	} /* class XMCustomView */
}
