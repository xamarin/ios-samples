using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SystemConfiguration;

public class Reachability {

	public bool IsReachableWithoutRequiringConnection (NetworkReachabilityFlags flags)
	{
		// Is it reachable with the current network configuration?
		bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

		// Do we need a connection to reach it?
		bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

		// Since the network stack will automatically try to get the WAN up,
		// probe that
		if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
			noConnectionRequired = true;

		return isReachable && noConnectionRequired;
	}

	// Is the host reachable with the current network configuration
	public bool IsHostReachable (string host)
	{
		if (host == null || host.Length == 0)
			return false;

		using (var r = new NetworkReachability (host)){
			NetworkReachabilityFlags flags;
			
			if (r.TryGetFlags (out flags)){
				return IsReachableWithoutRequiringConnection (flags);
			}
		}
		return false;
	}

	// Is 169.254.0.0 reachable (ad-hoc wifi
	// TODO: need to implement socket address probing
	
	// 
	
}

