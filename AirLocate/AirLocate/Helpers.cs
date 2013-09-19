using System;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;

namespace AirLocate {

	public static class Helpers {

		// create the CLBeaconRegion using the right contructor, returns null if input is invalid (no exceptions)
		public static CLBeaconRegion CreateRegion (NSUuid uuid, NSNumber major, NSNumber minor)
		{
			if (uuid == null)
				return null;
			if (minor == null) {
				if (major == null)
					return new CLBeaconRegion (uuid, Defaults.Identifier);
				else
					return new CLBeaconRegion (uuid, major.UInt16Value, Defaults.Identifier);
			} else if (major != null) {
				return new CLBeaconRegion (uuid, major.UInt16Value, minor.UInt16Value, Defaults.Identifier);
			}
			return null;
		}
	}
}