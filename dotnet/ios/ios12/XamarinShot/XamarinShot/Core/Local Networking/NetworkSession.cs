namespace XamarinShot.Models;

public interface INetworkSessionDelegate
{
        void NetworkSessionReceived (NetworkSession session, GameCommand command);

        void NetworkSessionJoining (NetworkSession session, Player player);

        void NetworkSessionLeaving (NetworkSession session, Player player);
}

public class NetworkSession : NSObject, IMCSessionDelegate, IMCNearbyServiceAdvertiserDelegate
{
        const string LocationKey = "LocationAttributeName";

        readonly List<Player> peers = new List<Player> ();

        readonly Player? myself;

        GameTableLocation? location;

        MCNearbyServiceAdvertiser? serviceAdvertiser;

        readonly string appIdentifier;

        public NetworkSession (Player myself, bool asServer, GameTableLocation? location, Player host) : base ()
        {
                this.myself = myself;
                Session = new MCSession (this.myself.PeerId, null, MCEncryptionPreference.Required);
                IsServer = asServer;
                this.location = location;
                Host = host;

                appIdentifier = NSBundle.MainBundle.BundleIdentifier;
                Session.Delegate = this;
        }

        public MCSession Session { get; private set; }

        public Player Host { get; private set; }

        public bool IsServer { get; private set; }

        public INetworkSessionDelegate? Delegate { get; set; }

        public bool IsAnyActivePeers => peers.Any ();

        /// <summary>
        /// For use when acting as game server
        /// </summary>
        public void StartAdvertising ()
        {
                if (serviceAdvertiser is null)
                {
                        var discoveryInfo = NSDictionary.FromObjectAndKey (new NSString (appIdentifier), new NSString (XamarinShotGameAttribute.AppIdentifier));
                        if (location is not null)
                        {
                                discoveryInfo [LocationKey] = new NSString (location.Identifier.ToString ());
                        }

                        var advertiser = new MCNearbyServiceAdvertiser (myself!.PeerId, discoveryInfo, XamarinShotGameService.PlayerService);
                        advertiser.Delegate = this;
                        advertiser.StartAdvertisingPeer ();
                        serviceAdvertiser = advertiser;
                }
        }

        public void StopAdvertising ()
        {
                // stop advertising
                serviceAdvertiser?.StopAdvertisingPeer ();
                serviceAdvertiser = null;
        }

        /// <summary>
        /// for beacon use
        /// </summary>
        /// <param name="newLocation">New location.</param>
        public void UpdateLocation (GameTableLocation newLocation)
        {
                location = newLocation;
        }

        #region Actions

        readonly DispatchQueue messagesQueue = new DispatchQueue ("messages", true);

        readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter> {
                        new Formattings.BoolFormatting(),
                        new Formattings.SCNVector3Formatting(),
                        new Formattings.SCNVector4Formatting(),
                        new Formattings.SCNMatrix4Formatting(),
                        new Formattings.SCNQuaternionFormatting(),
                }
        };

        public void Send (GAction action)
        {
                if (peers.Any ())
                {
                        var peerIds = peers.Select (peer => peer.PeerId).ToArray ();
                        Send (action, peerIds);
                }
        }

        public void Send (GAction action, Player player)
        {
                Send (action, new MCPeerID [] { player.PeerId });
        }

        void Send (GAction action, MCPeerID [] peerIds)
        {
                var json = JsonConvert.SerializeObject (action, serializerSettings);
                messagesQueue.DispatchAsync (() =>
                 {
                         using (var data = NSData.FromString (json))
                         {
                                 Session.SendData (data, peerIds, MCSessionSendDataMode.Reliable, out NSError error);
                         }
                 });
        }

        public void Receive (NSData data, MCPeerID peerID)
        {
                var player = peers.FirstOrDefault (peer => peer.PeerId == peerID);
                if (player is not null)
                {
                        string? json;
                        using (data)
                        {
                                json = NSString.FromData (data, NSStringEncoding.UTF8)?.ToString ();
                        }

                        if (!string.IsNullOrEmpty (json))
                        {
                                var action = JsonConvert.DeserializeObject<GAction> (json);
                                var command = new GameCommand (player, action!);
                                Delegate?.NetworkSessionReceived (this, command);
                        }
                }
        }

        #endregion

        #region IMCSessionDelegate

        public void DidChangeState (MCSession session, MCPeerID peerID, MCSessionState state)
        {
                var player = new Player (peerID);
                switch (state)
                {
                        case MCSessionState.Connected:
                                peers.Add (player);
                                Delegate?.NetworkSessionJoining (this, player);
                                break;

                        case MCSessionState.Connecting:
                                break;

                        case MCSessionState.NotConnected:
                                peers.Remove (player);
                                Delegate?.NetworkSessionLeaving (this, player);
                                break;
                }
        }

        public void DidReceiveData (MCSession session, NSData data, MCPeerID peerID)
        {
                Receive (data, peerID);
        }

        public void DidReceiveStream (MCSession session, NSInputStream stream, string streamName, MCPeerID peerID)
        {
                // this app doesn't use streams.
        }

        public void DidStartReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSProgress progress)
        {
                // this app doesn't use named resources.
        }

        public void DidFinishReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSUrl? localUrl, NSError? error)
        {
                if (error is null && localUrl is not null)
                {
                        // .mappedIfSafe makes the initializer attempt to map the file directly into memory
                        // using mmap(2), rather than serially copying the bytes into memory.
                        // this is faster and our app isn't charged for the memory usage.
                        var data = NSData.FromUrl (localUrl, NSDataReadingOptions.Mapped, out NSError readError);
                        if (readError is not null)
                        {
                                Receive (data, fromPeer);
                        }

                        // removing the file is done by the session, so long as we're done with it before the
                        // delegate method returns.
                }
        }

        #endregion

        #region IMCNearbyServiceAdvertiserDelegate

        public void DidReceiveInvitationFromPeer (MCNearbyServiceAdvertiser advertiser, MCPeerID peerID, NSData? context, MCNearbyServiceAdvertiserInvitationHandler invitationHandler)
        {
                invitationHandler (true, Session);
        }

        #endregion
}
