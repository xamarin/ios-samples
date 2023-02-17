
namespace ARMultiuser {
	using Foundation;
	using MultipeerConnectivity;
	using System;
	using System.Collections.Generic;
	using UIKit;

	/// <summary>
	/// A simple abstraction of the MultipeerConnectivity API as used in this app.
	/// </summary>
	public class MultipeerSession : NSObject,
									IMCSessionDelegate,
									IMCNearbyServiceBrowserDelegate,
									IMCNearbyServiceAdvertiserDelegate {
		private const string ServiceType = "ar-multi-sample";

		private readonly MCPeerID myPeerID = new MCPeerID (UIDevice.CurrentDevice.Name);
		private readonly MCNearbyServiceAdvertiser serviceAdvertiser;
		private readonly MCNearbyServiceBrowser serviceBrowser;
		private readonly MCSession session;

		private readonly Action<NSData, MCPeerID> receivedDataHandler;

		public MultipeerSession (Action<NSData, MCPeerID> receivedDataHandler) : base ()
		{
			this.receivedDataHandler = receivedDataHandler;

			this.session = new MCSession (this.myPeerID, null, MCEncryptionPreference.Required);
			this.session.Delegate = this;

			this.serviceAdvertiser = new MCNearbyServiceAdvertiser (this.myPeerID, null, ServiceType);
			this.serviceAdvertiser.Delegate = this;
			this.serviceAdvertiser.StartAdvertisingPeer ();

			this.serviceBrowser = new MCNearbyServiceBrowser (this.myPeerID, ServiceType);
			this.serviceBrowser.Delegate = this;
			this.serviceBrowser.StartBrowsingForPeers ();
		}

		public IList<MCPeerID> ConnectedPeers => this.session.ConnectedPeers;

		public void SendToAllPeers (NSData data)
		{
			this.session.SendData (data, this.session.ConnectedPeers, MCSessionSendDataMode.Reliable, out NSError error);
			if (error != null) {
				Console.WriteLine ($"error sending data to peers: {error.LocalizedDescription}");
			}
		}

		#region IMCSessionDelegate

		public void DidChangeState (MCSession session, MCPeerID peerID, MCSessionState state)
		{
			// not used
		}

		public void DidFinishReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSUrl localUrl, NSError error)
		{
			throw new Exception ("This service does not send/receive resources.");
		}

		public void DidReceiveData (MCSession session, NSData data, MCPeerID peerID)
		{
			this.receivedDataHandler (data, peerID);
		}

		public void DidReceiveStream (MCSession session, NSInputStream stream, string streamName, MCPeerID peerID)
		{
			throw new Exception ("This service does not send/receive streams.");
		}

		public void DidStartReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSProgress progress)
		{
			throw new Exception ("This service does not send/receive resources.");
		}

		#endregion

		#region IMCNearbyServiceBrowserDelegate

		public void FoundPeer (MCNearbyServiceBrowser browser, MCPeerID peerID, NSDictionary info)
		{
			// Invite the new peer to the session.
			browser.InvitePeer (peerID, this.session, null, 10);
		}

		public void LostPeer (MCNearbyServiceBrowser browser, MCPeerID peerID)
		{
			// This app doesn't do anything with non-invited peers, so there's nothing to do here.
		}

		#endregion

		#region IMCNearbyServiceAdvertiserDelegate

		public void DidReceiveInvitationFromPeer (MCNearbyServiceAdvertiser advertiser, MCPeerID peerID, NSData context, MCNearbyServiceAdvertiserInvitationHandler invitationHandler)
		{
			// Call handler to accept invitation and join the session.
			invitationHandler (true, this.session);
		}

		#endregion
	}
}
