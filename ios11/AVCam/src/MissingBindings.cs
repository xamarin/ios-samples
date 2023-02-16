using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AVFoundation;
using Foundation;
using ObjCRuntime;

namespace AVCam {
	public unsafe static partial class AVCapturePhoto_Ext {
		[CompilerGenerated]
		static readonly IntPtr class_ptr = Class.GetHandle ("AVCapturePhoto");

		[Export ("fileDataRepresentation")]
		[CompilerGenerated]
		public static NSData FileDataRepresentation (this AVCapturePhoto This)
		{
			return Runtime.GetNSObject<NSData> (global::ApiDefinition.Messaging.IntPtr_objc_msgSend (This.Handle, Selector.GetHandle ("fileDataRepresentation")));
		}

	} /* class AVCapturePhoto_Ext */

	public unsafe static partial class AVCapturePhotoOutput_AVCapturePhotoOutputDepthDataDeliverySupport {
		[CompilerGenerated]
		static readonly IntPtr class_ptr = Class.GetHandle ("AVCapturePhotoOutput");

		[Export ("isDepthDataDeliveryEnabled")]
		[CompilerGenerated]
		public static bool IsDepthDataDeliveryEnabled (this AVCapturePhotoOutput This)
		{
			return global::ApiDefinition.Messaging.bool_objc_msgSend (This.Handle, Selector.GetHandle ("isDepthDataDeliveryEnabled"));
		}

		[Export ("setDepthDataDeliveryEnabled:")]
		[CompilerGenerated]
		public static void IsDepthDataDeliveryEnabled (this AVCapturePhotoOutput This, bool enabled)
		{
			global::ApiDefinition.Messaging.void_objc_msgSend_bool (This.Handle, Selector.GetHandle ("setDepthDataDeliveryEnabled:"), enabled);
		}

		[Export ("isDepthDataDeliverySupported")]
		[CompilerGenerated]
		public static bool IsDepthDataDeliverySupported (this AVCapturePhotoOutput This)
		{
			return global::ApiDefinition.Messaging.bool_objc_msgSend (This.Handle, Selector.GetHandle ("isDepthDataDeliverySupported"));
		}

	} /* class AVCapturePhotoOutput_AVCapturePhotoOutputDepthDataDeliverySupport */

	public unsafe static partial class AVCapturePhotoSettings_AVCapturePhotoSettings {
		[CompilerGenerated]
		static readonly IntPtr class_ptr = Class.GetHandle ("AVCapturePhotoSettings");

		[Export ("isDepthDataDeliveryEnabled")]
		[CompilerGenerated]
		public static bool IsDepthDataDeliveryEnabled (this AVCapturePhotoSettings This)
		{
			return global::ApiDefinition.Messaging.bool_objc_msgSend (This.Handle, Selector.GetHandle ("isDepthDataDeliveryEnabled"));
		}

		[Export ("setDepthDataDeliveryEnabled:")]
		[CompilerGenerated]
		public static void IsDepthDataDeliveryEnabled (this AVCapturePhotoSettings This, bool enabled)
		{
			global::ApiDefinition.Messaging.void_objc_msgSend_bool (This.Handle, Selector.GetHandle ("setDepthDataDeliveryEnabled:"), enabled);
		}

	} /* class AVCapturePhotoSettings_AVCapturePhotoSettings */

	public unsafe static partial class AVCapturePhotoSettings_AVCapturePhotoSettingsConversions {
		[CompilerGenerated]
		static readonly IntPtr class_ptr = Class.GetHandle ("AVCapturePhotoSettings");

		[Export ("processedFileType")]
		[CompilerGenerated]
		public static string ProcessedFileType (this AVCapturePhotoSettings This)
		{
			return NSString.FromHandle (global::ApiDefinition.Messaging.IntPtr_objc_msgSend (This.Handle, Selector.GetHandle ("processedFileType")));
		}

	} /* class AVCapturePhotoSettings_AVCapturePhotoSettingsConversions */

	public unsafe static partial class AVVideo2 {
		[CompilerGenerated]
		static NSString _CodecHEVC;
		[Field ("AVVideoCodecTypeHEVC", "__Internal")]
		public static NSString CodecHEVC {
			get {
				if (_CodecHEVC == null)
					_CodecHEVC = Dlfcn.GetStringConstant (Libraries.__Internal.Handle, "AVVideoCodecTypeHEVC");
				return _CodecHEVC;
			}
		}
	} /* class AVVideo2 */

	public unsafe static partial class NSString_NSStringExt {

		[CompilerGenerated]
		static readonly IntPtr class_ptr = Class.GetHandle ("NSString");

		[Export ("boolValue")]
		[CompilerGenerated]
		public static bool BoolValue (this NSString This)
		{
			return global::ApiDefinition.Messaging.bool_objc_msgSend (This.Handle, Selector.GetHandle ("boolValue"));
		}

	} /* class NSString_NSStringExt */
}

namespace ApiDefinition {
	partial class Messaging {
		static internal System.Reflection.Assembly this_assembly = typeof (Messaging).Assembly;

		const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend (IntPtr receiever, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public extern static IntPtr IntPtr_objc_msgSendSuper (IntPtr receiever, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static bool bool_objc_msgSend (IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public extern static bool bool_objc_msgSendSuper (IntPtr receiver, IntPtr selector);
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static void void_objc_msgSend_bool (IntPtr receiver, IntPtr selector, bool arg1);
		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public extern static void void_objc_msgSendSuper_bool (IntPtr receiver, IntPtr selector, bool arg1);
	}
}

namespace ObjCRuntime {
	[CompilerGenerated]
	static partial class Libraries {
		static public class __Internal {
			static public readonly IntPtr Handle = Dlfcn.dlopen (null, 0);
		}
	}
}
