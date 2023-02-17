
namespace XamarinShot.Models {
	using Foundation;
	using MultipeerConnectivity;
	using System.Collections.Generic;
	using System.Linq;
	using XamarinShot.Utils;
	using Newtonsoft.Json;
	using CoreFoundation;

	public interface INetworkSessionDelegate {
		void NetworkSessionReceived (NetworkSession session, GameCommand command);

		void NetworkSessionJoining (NetworkSession session, Player player);

		void NetworkSessionLeaving (NetworkSession session, Player player);
	}

	public class NetworkSession : NSObject, IMCSessionDelegate, IMCNearbyServiceAdvertiserDelegate {
		private const string LocationKey = "LocationAttributeName";

		private readonly List<Player> peers = new List<Player> ();

		private readonly Player myself;

		private GameTableLocation location;

		private MCNearbyServiceAdvertiser serviceAdvertiser;

		private readonly string appIdentifier;

		public NetworkSession (Player myself, bool asServer, GameTableLocation location, Player host) : base ()
		{
			this.myself = myself;
			this.Session = new MCSession (this.myself.PeerId, null, MCEncryptionPreference.Required);
			this.IsServer = asServer;
			this.location = location;
			this.Host = host;

			this.appIdentifier = NSBundle.MainBundle.BundleIdentifier;
			this.Session.Delegate = this;
		}

		public MCSession Session { get; private set; }

		public Player Host { get; private set; }

		public bool IsServer { get; private set; }

		public INetworkSessionDelegate Delegate { get; set; }

		public bool IsAnyActivePeers => this.peers.Any ();

		/// <summary>
		/// For use when acting as game server
		/// </summary>
		public void StartAdvertising ()
		{
			if (this.serviceAdvertiser == null) {
				var discoveryInfo = NSDictionary.FromObjectAndKey (new NSString (appIdentifier), new NSString (XamarinShotGameAttribute.AppIdentifier));
				if (this.location != null) {
					discoveryInfo [LocationKey] = new NSString (this.location.Identifier.ToString ());
				}

				var advertiser = new MCNearbyServiceAdvertiser (this.myself.PeerId, discoveryInfo, XamarinShotGameService.PlayerService);
				advertiser.Delegate = this;
				advertiser.StartAdvertisingPeer ();
				this.serviceAdvertiser = advertiser;
			}
		}

		public void StopAdvertising ()
		{
			// stop advertising
			this.serviceAdvertiser?.StopAdvertisingPeer ();
			this.serviceAdvertiser = null;
		}

		/// <summary>
		/// for beacon use
		/// </summary>
		/// <param name="newLocation">New location.</param>
		public void UpdateLocation (GameTableLocation newLocation)
		{
			this.location = newLocation;
		}

		#region Actions

		private readonly DispatchQueue messagesQueue = new DispatchQueue ("messages", true);

		private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings {
			NullValueHandling = NullValueHandling.Ignore,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			Converters = new List<JsonConverter>
			{
				new Formattings.BoolFormatting(),
				new Formattings.SCNVector3Formatting(),
				new Formattings.SCNVector4Formatting(),
				new Formattings.SCNMatrix4Formatting(),
				new Formattings.SCNQuaternionFormatting(),
			}
		};

		public void Send (GAction action)
		{
			if (this.peers.Any ()) {
				var peerIds = this.peers.Select (peer => peer.PeerId).ToArray ();
				this.Send (action, peerIds);
			}
		}

		public void Send (GAction action, Player player)
		{
			this.Send (action, new MCPeerID [] { player.PeerId });
		}

		private void Send (GAction action, MCPeerID [] peerIds)
		{
			var json = JsonConvert.SerializeObject (action, this.serializerSettings);
			this.messagesQueue.DispatchAsync (() => {
				using (var data = NSData.FromString (json)) {
					this.Session.SendData (data, peerIds, MCSessionSendDataMode.Reliable, out NSError error);
				}
			});
		}

		public void Receive (NSData data, MCPeerID peerID)
		{
			var player = this.peers.FirstOrDefault (peer => peer.PeerId == peerID);
			if (player != null) {
				string json;
				using (data) {
					json = NSString.FromData (data, NSStringEncoding.UTF8)?.ToString ();
				}

				if (!string.IsNullOrEmpty (json)) {
					var action = JsonConvert.DeserializeObject<GAction> (json);
					var command = new GameCommand (player, action);
					this.Delegate?.NetworkSessionReceived (this, command);
				}
			}
		}

		#endregion

		#region IMCSessionDelegate

		public void DidChangeState (MCSession session, MCPeerID peerID, MCSessionState state)
		{
			var player = new Player (peerID);
			switch (state) {
			case MCSessionState.Connected:
				this.peers.Add (player);
				this.Delegate?.NetworkSessionJoining (this, player);
				break;

			case MCSessionState.Connecting:
				break;

			case MCSessionState.NotConnected:
				this.peers.Remove (player);
				this.Delegate?.NetworkSessionLeaving (this, player);
				break;
			}
		}

		public void DidReceiveData (MCSession session, NSData data, MCPeerID peerID)
		{
			this.Receive (data, peerID);
		}

		public void DidReceiveStream (MCSession session, NSInputStream stream, string streamName, MCPeerID peerID)
		{
			// this app doesn't use streams.
		}

		public void DidStartReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSProgress progress)
		{
			// this app doesn't use named resources.
		}

		public void DidFinishReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSUrl localUrl, NSError error)
		{
			if (error == null && localUrl != null) {
				// .mappedIfSafe makes the initializer attempt to map the file directly into memory
				// using mmap(2), rather than serially copying the bytes into memory.
				// this is faster and our app isn't charged for the memory usage.
				var data = NSData.FromUrl (localUrl, NSDataReadingOptions.Mapped, out NSError readError);
				if (readError != null) {
					this.Receive (data, fromPeer);
				}

				// removing the file is done by the session, so long as we're done with it before the
				// delegate method returns.
			}
		}

		#endregion

		#region IMCNearbyServiceAdvertiserDelegate

		public void DidReceiveInvitationFromPeer (MCNearbyServiceAdvertiser advertiser, MCPeerID peerID, NSData context, MCNearbyServiceAdvertiserInvitationHandler invitationHandler)
		{
			invitationHandler (true, Session);
		}

		#endregion
	}
}
