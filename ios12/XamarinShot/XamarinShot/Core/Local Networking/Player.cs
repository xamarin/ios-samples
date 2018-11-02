
namespace XamarinShot.Models
{
    using System;
    using MultipeerConnectivity;

    public class Player
    {
        private readonly int hashValue;

        public Player(MCPeerID peerID)
        {
            this.PeerId = peerID;
            this.hashValue = peerID.GetHashCode();
        }

        public Player(string username)
        {
            this.PeerId = new MCPeerID(username);
            this.hashValue = this.PeerId.GetHashCode();
        }

        public string Username => this.PeerId.DisplayName;

        public MCPeerID PeerId { get; private set; }

        public override bool Equals(object obj)
        {
            return this.PeerId == (obj as Player)?.PeerId;
        }

        public override int GetHashCode()
        {
            return this.PeerId.GetHashCode();
        }
    }
}