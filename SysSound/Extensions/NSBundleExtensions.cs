using System;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;

namespace SysSound.Extensions {

	//helper methods for withing with bundles
	public static class NSBundleExtensions {
		private static readonly IntPtr _Selector_URLForResource = Selector.GetHandle ("URLForResource:withExtension:");
		
		/// <summary>
		/// Returns the file URL for the resource identified by the specified name and file extension.
		/// </summary>
		public static NSUrl URLForResource (this NSBundle source, string for_resource, string with_extension) { 
			var ns_for = new NSString(for_resource);
			var ns_extension = new NSString(with_extension);
			
			//try and get the resource
			var result = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(
				source.Handle, 
				NSBundleExtensions._Selector_URLForResource, 
				ns_for.Handle, 
				ns_extension.Handle); 
			
			//return back as a NSUrl
			return Runtime.GetNSObject(result) as NSUrl;
		}
		
	}
	
}

