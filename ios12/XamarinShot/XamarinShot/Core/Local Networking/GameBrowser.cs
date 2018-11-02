
namespace XamarinShot.Models
{
    using CoreFoundation;
    using Foundation;
    using MultipeerConnectivity;
    using XamarinShot.Utils;
    using System.Collections.Generic;
    using System.Linq;

    public interface IGameBrowserDelegate
    {
        void SawGames(GameBrowser browser, IList<NetworkGame> games);
    }

    public class GameBrowser : NSObject, IMCNearbyServiceBrowserDelegate
    {
        private readonly IList<NetworkGame> games = new List<NetworkGame>();

        private readonly MCNearbyServiceBrowser serviceBrowser;

        private readonly Player myself;
         
        public GameBrowser(Player myself) : base()
        {
            this.myself = myself;
            this.serviceBrowser = new MCNearbyServiceBrowser(this.myself.PeerId, XamarinShotGameService.PlayerService) { Delegate = this };
            this.serviceBrowser.Delegate = this;
        }

        public IGameBrowserDelegate Delegate { get; set; }

        public void Start()
        {
            // looking for peers
            this.serviceBrowser.StartBrowsingForPeers();
        }

        public void Stop()
        {
            // stopping the search for peers
            this.serviceBrowser.StopBrowsingForPeers();
        }

        public NetworkSession Join(NetworkGame game)
        {
            NetworkSession result = null;
            if (this.games.Contains(game))
            {
                result = new NetworkSession(this.myself, false, game.Location, game.Host);
                this.serviceBrowser.InvitePeer(game.Host.PeerId, result.Session, null, 30d);
            }

            return result;
        }

        public void Refresh()
        {
            this.Delegate?.SawGames(this, new List<NetworkGame>(this.games));
        }

        #region IMCNearbyServiceBrowserDelegate

        public void FoundPeer(MCNearbyServiceBrowser browser, MCPeerID peerID, NSDictionary info)
        {
            if (peerID != this.myself.PeerId)
            {
                var appIdentifier = info?[XamarinShotGameAttribute.AppIdentifier];
                if (appIdentifier?.ToString() == NSBundle.MainBundle.BundleIdentifier)
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        var player = new Player(peerID);
                        var gameName = info?[XamarinShotGameAttribute.Name] as NSString;

                        GameTableLocation location = null;
                        var locationIdString = info?[XamarinShotGameAttribute.Location] as NSString;
                        if (!string.IsNullOrEmpty(locationIdString) && int.TryParse(locationIdString, out int locationId))
                        {
                            location = GameTableLocation.GetLocation(locationId);
                        }

                        var game = new NetworkGame(player, gameName, location?.Identifier ?? 0);
                        this.games.Add(game);
                        this.Delegate?.SawGames(this, new List<NetworkGame>(this.games));
                    });
                }
            }
        }

        public void LostPeer(MCNearbyServiceBrowser browser, MCPeerID peerID)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var removed = this.games.FirstOrDefault(game => game.Host.PeerId != peerID);
                if (removed != null)
                {
                    this.games.Remove(removed);
                    this.Delegate?.SawGames(this, new List<NetworkGame>(this.games));
                }
            });
        }

        #endregion
    }
}