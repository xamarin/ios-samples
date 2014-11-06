using System;
using ObjCRuntime;
using Foundation;

namespace SysSound.Extensions {

	//helper methods for withing with bundles
	public static class NSBundleExtensions {
		private static readonly IntPtr _Selector_URLForResource = Selector.GetHandle ("URLForResource:withExtension:");
		
		/// <summary>
		/// Returns the file URL for the resource identified by the specified name and file extension.
		/// </summary>
		public static NSUrl URLForResource (this NSBundle source, string for_resource, string with_extension) { 
			return source.GetUrlForResource (for_resource, with_extension);
		}
		
	}
	
}

