using System;
using System.Collections.Generic;
using Foundation;

namespace AirLocate
{
	public static class Defaults
	{
		static NSUuid[] supportedProximityUuids;
		static NSNumber defaultPower;

		static Defaults ()
		{
			supportedProximityUuids = new NSUuid [] {
				new NSUuid ("E2C56DB5-DFFB-48D2-B060-D0F5A71096E0"),
				new NSUuid ("5A4BCFCE-174E-4BAC-A814-092E77F6B7E5"),
				new NSUuid ("74278BDA-B644-4520-8F0C-720EAF059935")
			};
			defaultPower = new NSNumber (-59);
		}

		static public string Identifier {
			get { return "com.apple.AirLocate"; }
		}

		static public NSUuid DefaultProximityUuid {
			get { return supportedProximityUuids [0]; }
		}

		static public IList<NSUuid> SupportedProximityUuids {
			get { return supportedProximityUuids; }
		}

		static public NSNumber DefaultPower {
			get { return defaultPower; }
		}
	}
}
