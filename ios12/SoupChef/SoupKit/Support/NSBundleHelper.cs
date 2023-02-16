using System;
using Foundation;
using SoupKit.Data;
using ObjCRuntime;
namespace SoupKit.Support {
	public static class NSBundleHelper {
		public static NSBundle SoupKitBundle {
			get {
				return NSBundle.MainBundle;
			}
		}
	}
}
