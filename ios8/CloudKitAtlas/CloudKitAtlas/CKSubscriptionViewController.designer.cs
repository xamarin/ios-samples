// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace CloudKitAtlas
{
	[Register ("CKSubscriptionViewController")]
	partial class CKSubscriptionViewController
	{
		[Outlet]
		MonoTouch.UIKit.UISwitch subscriptionSwitch { get; set; }

		[Action ("SubscriptionPreferenceUpdated:")]
		partial void SubscriptionPreferenceUpdated (MonoTouch.UIKit.UISwitch sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (subscriptionSwitch != null) {
				subscriptionSwitch.Dispose ();
				subscriptionSwitch = null;
			}
		}
	}
}
