using CoreFoundation;
using Foundation;
using SystemConfiguration;
using System.Net;

namespace Reachability
{
    public enum NetworkStatus
    {
        NotReachable,
        ReachableViaWWAN,
        ReachableViaWiFi
    }

    /// <summary>
    /// Basic demonstration of how to use the SystemConfiguration Reachablity APIs.
    /// </summary>
    public class Reachability : NSObject
    {
        ~Reachability()
        {
            StopNotifier();
            if (NetworkReachability != null)
            {
                NetworkReachability.Dispose();
            }
        }

        public static string ReachabilityChangedNotification { get; } = "kNetworkReachabilityChangedNotification";

        public NetworkReachability NetworkReachability { get; private set; }

        public static Reachability ReachabilityWithHostName(string hostName)
        {
            var reachability = new NetworkReachability(hostName);
            return new Reachability { NetworkReachability = reachability };
        }

        public static Reachability ReachabilityWithAddress(IPAddress hostAddress)
        {
            var reachability = new NetworkReachability(hostAddress);
            return new Reachability { NetworkReachability = reachability };
        }

        public static Reachability ReachabilityForInternetConnection()
        {
            var reachability = new NetworkReachability(new IPAddress(0));
            return new Reachability { NetworkReachability = reachability };
        }

        private void HandleNotification(NetworkReachabilityFlags flags)
        {
            // Post a notification to notify the client that the network reachability changed.
            NSNotificationCenter.DefaultCenter.PostNotificationName(ReachabilityChangedNotification, this);
        }

        #region Start and stop notifier

        public bool StartNotifier()
        {
            var result = false;

            var status = NetworkReachability.SetNotification(HandleNotification);
            if (status == StatusCode.OK)
            {
                result = NetworkReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }

            return result;
        }

        private void StopNotifier()
        {
            if (NetworkReachability != null)
            {
                NetworkReachability.Unschedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }
        }

        #endregion

        #region Network Flag Handling

        public NetworkStatus NetworkStatusForFlags(NetworkReachabilityFlags flags)
        {
            if ((flags & NetworkReachabilityFlags.Reachable) == 0)
            {
                // The target host is not reachable.
                return NetworkStatus.NotReachable;
            }

            NetworkStatus result = NetworkStatus.NotReachable;
            if ((flags & NetworkReachabilityFlags.ConnectionRequired) == 0)
            {
                // If the target host is reachable and no connection is required then we'll assume (for now) that you're on Wi-Fi...
                result = NetworkStatus.ReachableViaWiFi;
            }

            if ((flags & NetworkReachabilityFlags.ConnectionOnDemand) != 0 ||
                (flags & NetworkReachabilityFlags.ConnectionOnTraffic) != 0)
            {
                // ... and the connection is on-demand (or on-traffic) if the calling application is using the CFSocketStream or higher APIs...
                if ((flags & NetworkReachabilityFlags.InterventionRequired) == 0)
                {
                    // ... and no [user] intervention is needed...
                    result = NetworkStatus.ReachableViaWiFi;
                }
            }

            if ((flags & NetworkReachabilityFlags.IsWWAN) == NetworkReachabilityFlags.IsWWAN)
            {
                // ... but WWAN connections are OK if the calling application is using the CFNetwork APIs.
                result = NetworkStatus.ReachableViaWWAN;
            }

            return result;
        }

        public bool ConnectionRequired()
        {
            if (NetworkReachability.TryGetFlags(out NetworkReachabilityFlags flags))
            {
                return (flags & NetworkReachabilityFlags.ConnectionRequired) != 0;
            }

            return false;
        }

        public NetworkStatus CurrentReachabilityStatus()
        {
            var returnValue = NetworkStatus.NotReachable;
            if (NetworkReachability.TryGetFlags(out NetworkReachabilityFlags flags))
            {
                returnValue = this.NetworkStatusForFlags(flags);
            }

            return returnValue;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (NetworkReachability != null)
                {
                    NetworkReachability.Dispose();
                }
            }
        }
    }
}