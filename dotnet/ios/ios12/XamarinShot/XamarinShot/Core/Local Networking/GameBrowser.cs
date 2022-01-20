
namespace XamarinShot.Models;


public interface IGameBrowserDelegate
{
void SawGames(GameBrowser browser, IList<NetworkGame> games);
}

public class GameBrowser : NSObject, IMCNearbyServiceBrowserDelegate
{
        private readonly IList<NetworkGame> games = new List<NetworkGame> ();

        private readonly MCNearbyServiceBrowser serviceBrowser;

        private readonly Player myself;

        public GameBrowser (Player myself) : base ()
        {
                this.myself = myself;
                serviceBrowser = new MCNearbyServiceBrowser (this.myself.PeerId, XamarinShotGameService.PlayerService) { Delegate = this };
                serviceBrowser.Delegate = this;
        }

        public IGameBrowserDelegate? Delegate { get; set; }

        public void Start ()
        {
                // looking for peers
                serviceBrowser.StartBrowsingForPeers ();
        }

        public void Stop ()
        {
                // stopping the search for peers
                serviceBrowser.StopBrowsingForPeers ();
        }

        public NetworkSession? Join (NetworkGame game)
        {
                NetworkSession? result = null;
                if (games.Contains (game))
                {
                        result = new NetworkSession (myself, false, game.Location, game.Host);
                        serviceBrowser.InvitePeer (game.Host.PeerId, result.Session, null, 30d);
                }

                return result;
        }

        public void Refresh ()
        {
                Delegate?.SawGames (this, new List<NetworkGame> (games));
        }

        #region IMCNearbyServiceBrowserDelegate

        public void FoundPeer (MCNearbyServiceBrowser browser, MCPeerID peerID, NSDictionary? info)
        {
                if (peerID != myself.PeerId)
                {
                        var appIdentifier = info? [XamarinShotGameAttribute.AppIdentifier];
                        if (appIdentifier?.ToString () == NSBundle.MainBundle.BundleIdentifier)
                        {
                                DispatchQueue.MainQueue.DispatchAsync (() =>
                                {
                                        var player = new Player (peerID);
                                        var gameName = info? [XamarinShotGameAttribute.Name] as NSString;
                                        
                                        GameTableLocation? location = null;
                                        var locationIdString = info? [XamarinShotGameAttribute.Location] as NSString;
                                        if (!string.IsNullOrEmpty (locationIdString) && int.TryParse (locationIdString, out int locationId))
                                        {
                                                location = GameTableLocation.GetLocation (locationId);
                                        }
                                        
                                        var game = new NetworkGame (player, gameName, location?.Identifier ?? 0);
                                        games.Add (game);
                                        Delegate?.SawGames (this, new List<NetworkGame> (games));
                                });
                        }
                }
        }

        public void LostPeer (MCNearbyServiceBrowser browser, MCPeerID peerID)
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        var removed = games.FirstOrDefault (game => game.Host.PeerId != peerID);
                        if (removed is not null)
                        {
                                games.Remove (removed);
                                Delegate?.SawGames (this, new List<NetworkGame> (games));
                        }
                });
        }

        #endregion
}
