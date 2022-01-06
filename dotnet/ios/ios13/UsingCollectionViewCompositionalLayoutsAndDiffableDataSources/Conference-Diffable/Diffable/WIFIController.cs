/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Controller object which notifies our application when availalbe Wi-Fi APs are available
*/

using System;
using System.Collections.Generic;

using CoreFoundation;
using Foundation;

namespace Conference_Diffable.Diffable {
	public partial class WIFIController {
		public class Network : NSObject, IEquatable<Network> {
			public string Id { get; private set; }
			public string Name { get; private set; }

			public Network (string name)
			{
				Name = name;
				Id = new NSUuid ().ToString ();
			}

			public static bool operator == (Network left, Network right)
			{
				if (ReferenceEquals (left, right))
					return true;

				if (ReferenceEquals (left, null))
					return false;

				if (ReferenceEquals (right, null))
					return false;

				return left.Equals (right);
			}

			public static bool operator != (Network left, Network right) => !(left == right);
			public override bool Equals (object obj) => this == (Network)obj;
			public bool Equals (Network other) => Id == other.Id;
			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);
		}

		UpdateHandler updateHandler;
		int updateInterval = 2000;
		Dictionary<string, Network> availableNetworksDict;

		public delegate void UpdateHandler (WIFIController wifiController);

		public bool ScanForNetworks { get; set; }
		public bool WifiEnabled { get; set; }
		public Network [] AvailableNetworks { get; private set; }

		public WIFIController (UpdateHandler updateHandler)
		{
			availableNetworksDict = new Dictionary<string, Network> ();
			this.updateHandler = updateHandler;
			UpdateAvailableNetworks (allNetworks);
			PerformRandomUpdate ();

		}

		public Network GetNetwork (NSUuid uuid) => GetNetwork (uuid.ToString ());
		private Network GetNetwork (string uuid) => availableNetworksDict [uuid];

		void PerformRandomUpdate ()
		{
			if (WifiEnabled && ScanForNetworks) {
				var updatedNetworks = new List<Network> ();

				if (AvailableNetworks != null) updatedNetworks.AddRange (AvailableNetworks);

				if (updatedNetworks.Count == 0) {
					AvailableNetworks = allNetworks;
				} else {
					var random = new Random (DateTime.Now.Millisecond);

					var shouldRemove = random.Next (0, 3) == 0;
					if (shouldRemove) {
						var removeCount = random.Next (0, updatedNetworks.Count);
						for (int i = 0; i < removeCount; i++) {
							var removeIndex = random.Next (0, updatedNetworks.Count);
							updatedNetworks.RemoveAt (removeIndex);
						}
					}

					var shouldAdd = random.Next (0, 3) == 0;
					if (shouldAdd) {
						var allNetworksSet = new List<Network> (allNetworks);
						var updatedNetworksSet = new List<Network> (updatedNetworks);
						var notPresentNetworksSet = new List<Network> (allNetworksSet);

						foreach (var network in updatedNetworksSet)
							notPresentNetworksSet.Remove (network);

						if (notPresentNetworksSet.Count > 0) {
							var addCount = random.Next (0, notPresentNetworksSet.Count);

							for (int i = 0; i < addCount; i++) {
								var removeIndex = random.Next (0, notPresentNetworksSet.Count);
								var networkToAdd = notPresentNetworksSet [removeIndex];
								notPresentNetworksSet.Remove (networkToAdd);
								updatedNetworksSet.Add (networkToAdd);
							}
						}
						updatedNetworks = updatedNetworksSet;
					}
					UpdateAvailableNetworks (updatedNetworks.ToArray ());
				}

				// notify
				updateHandler (this);
			}

			var deadline = new DispatchTime (DispatchTime.Now, new TimeSpan (0, 0, 4));
			DispatchQueue.MainQueue.DispatchAfter (deadline, () => PerformRandomUpdate ());
		}

		void UpdateAvailableNetworks (Network [] networks)
		{
			AvailableNetworks = networks;
			availableNetworksDict.Clear ();
			foreach (var network in AvailableNetworks)
				availableNetworksDict [network.Id] = network;
		}

		Network [] allNetworks = {
			new Network ("AirSpace1"),
			new Network ("Living Room"),
			new Network ("Courage"),
			new Network ("Nacho WiFi"),
			new Network ("FBI Surveillance Van"),
			new Network ("Peacock-Swagger"),
			new Network ("GingerGymnist"),
			new Network ("Second Floor"),
			new Network ("Evergreen"),
			new Network ("__hidden_in_plain__sight__"),
			new Network ("MarketingDropBox"),
			new Network ("HamiltonVille"),
			new Network ("404NotFound"),
			new Network ("SNAGVille"),
			new Network ("Overland101"),
			new Network ("TheRoomWiFi"),
			new Network ("PrivateSpace")
		};
	}
}

