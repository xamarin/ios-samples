using System;
namespace Example_SharedResources.Code.Network
{
	/// <summary>
	/// Contains the potential network availability status
	/// </summary>
	public enum NetworkStatus
	{
		NotReachable,
		ReachableViaCarrierDataNetwork,
		ReachableViaWiFiNetwork
	}
}

