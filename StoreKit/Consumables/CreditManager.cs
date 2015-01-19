using System;
using Foundation;

namespace Consumables {
	// WARNING: this is a trivial example of tracking a
	// consumable in-app purchase balance. In reality this
	// should be encrypted and possibly even managed remotely
	// on your server (with a strategy for offline use).
	// NSUserDefaults are EASY for iOS users to edit with a little bit of knowledge,
	// plus this value is backed-up and restored to other iOS devices so the
	// user could easily be spending them twice or something, or worse if they delete
	// the app before backing up, they'd lose the credits altogether!
	// Basically, this is ONLY intended as a demo of the StoreKit code,
	// NOT how you should build a credits system for iOS.
	public static class CreditManager {
		static NSString defaultKey = new NSString("monkeyDollarsBalance");
		static nint monkeyCredits = 0;
		static CreditManager ()
		{
			monkeyCredits = NSUserDefaults.StandardUserDefaults.IntForKey(defaultKey);
		}

		public static int Balance() {
			NSUserDefaults.StandardUserDefaults.Synchronize ();
			return (int)NSUserDefaults.StandardUserDefaults.IntForKey(defaultKey);
		}
		public static void Add (int moreDollars) {
			monkeyCredits += moreDollars;
			NSUserDefaults.StandardUserDefaults.SetInt(monkeyCredits, defaultKey);
			NSUserDefaults.StandardUserDefaults.Synchronize ();
		}
		public static bool Spend (int lessDollars) {
			if (monkeyCredits >= lessDollars) {
				monkeyCredits -= lessDollars;
				NSUserDefaults.StandardUserDefaults.SetInt(monkeyCredits, defaultKey);
				NSUserDefaults.StandardUserDefaults.Synchronize ();
				return true;
			} else {
				return false;
			}
		}
	}
}
