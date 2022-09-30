using XamarinShot.Models.Interactions;

namespace XamarinShot.Models;

public interface IGrabInteractionDelegate
{
        bool ShouldForceRelease (IGrabbable grabbable);
        void OnServerGrab (IGrabbable grabbable, CameraInfo cameraInfo, Player player);
        void OnGrabStart (IGrabbable grabbable, CameraInfo cameraInfo, Player player);
        void OnServerRelease (IGrabbable grabbable, CameraInfo cameraInfo, Player player);
        void OnUpdateGrabStatus (IGrabbable grabbable, CameraInfo cameraInfo);
}

public class GrabInteraction : IInteraction
{
        private bool isTouching;

        // Index used to assign to new object
        private int currentIndex = 0;

        public GrabInteraction (IInteractionDelegate @delegate)
        {
                Delegate = @delegate;
        }

        public IInteractionDelegate? Delegate { get; private set; }

        public IGrabInteractionDelegate? GrabDelegate { get; set; }

        public IGrabbable? GrabbedGrabbable { get; set; }

        public Dictionary<int, IGrabbable> Grabbables { get; private set; } = new Dictionary<int, IGrabbable> ();

        /// <summary>
        /// Should be added only from the classes using GrabInteraction to prevent duplicates
        /// </summary>
        public void AddGrabbable (IGrabbable grabbable)
        {
                grabbable.GrabbableId = currentIndex;
                Grabbables [currentIndex] = grabbable;
                currentIndex += 1;
        }

        public void HandleTouch (TouchType type, Ray camera)
        {
                if (type == TouchType.Began)
                {
                        if (GrabbableToGrab (camera) is not null)
                        {
                                isTouching = true;
                        }
                }
                else if (type == TouchType.Ended)
                {
                        isTouching = false;
                }
        }

        public void Update (CameraInfo cameraInfo)
        {
                if (Delegate is null || GrabDelegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                // Dispatch grab action to server so that server can test if grab succeed.
                // If grab succeed, server would update the grabbable's player for all clients
                // Note:
                // This check is done in update to counter the case of network lag
                // if touch down and up is very quick, the shouldTryRelease might not trigger right at touch up,
                // because information on grabbed grabbableID might not arrived yet
                if (isTouching && GrabbedGrabbable is null)
                {
                        if (GameTime.FrameCount % 3 == 0) // Only send messages at 20 fps to save bandwidth
                        {
                                // Send grab message to server if player can grab something
                                var grabbable = GrabbableToGrab (cameraInfo.Ray);
                                if (grabbable is not null)
                                {
                                        var grab = new GrabInfo (grabbable.GrabbableId, cameraInfo);
                                        Delegate.DispatchActionToServer (new GameActionType { TryGrab = grab, Type = GameActionType.GActionType.TryGrab });
                                        return;
                                }
                        }
                }

                if (GrabbedGrabbable is not null)
                {
                        if (!Delegate.IsServer)
                        {
                                // Client move the sling locally, ignore server's physics data to prevent lag
                                GrabbedGrabbable.Move (cameraInfo);
                        }

                        // If touch is up or the sling is pulled too far, release the ball
                        if (!isTouching || GrabDelegate.ShouldForceRelease (GrabbedGrabbable))
                        {
                                if (GameTime.FrameCount % 3 == 0)// Only send messages at 20 fps to save bandwidth
                                {
                                        var grab = new GrabInfo (GrabbedGrabbable.GrabbableId, cameraInfo);
                                        Delegate.DispatchActionToServer (new GameActionType { TryRelease = grab, Type = GameActionType.GActionType.TryRelease });
                                        return;
                                }
                        }

                        // Dispatch slingMove to server.
                        var data = new GrabInfo (GrabbedGrabbable.GrabbableId, cameraInfo);
                        Delegate.DispatchActionToServer (new GameActionType { GrabMove = data, Type = GameActionType.GActionType.GrabMove });
                }
        }

        IGrabbable? NearestVisibleGrabbable (Ray cameraRay)
        {
                // Find closest visible grabbable
                IGrabbable? closestGrabbable = null;
                var closestDist = 0f;
                foreach (var grabbable in Grabbables.Values)
                {
                        if (grabbable.IsVisible)
                        {
                                var distance = grabbable.DistanceFrom (cameraRay.Position);
                                if (closestGrabbable is null || distance < closestDist)
                                {
                                        closestGrabbable = grabbable;
                                        closestDist = distance;
                                }
                        }
                }

                return closestGrabbable;
        }

        public IGrabbable? GrabbableToGrab (Ray cameraRay)
        {
                IGrabbable? result = null;

                var grabbable = NearestVisibleGrabbable (cameraRay);
                if (grabbable is not null)
                {
                        if (grabbable.CanGrab (cameraRay))
                        {
                                result = grabbable;
                        }
                }

                return result;
        }

        #region Interactions

        public void Handle (GameActionType gameAction, Player player)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                switch (gameAction.Type)
                {
                        // Try Grab
                        case GameActionType.GActionType.TryGrab:
                                HandleTryGrabAction (gameAction.TryGrab!, player, Delegate);
                                break;

                        // Inform specific player of grab, when it succeeds
                        case GameActionType.GActionType.GrabStart:
                                HandleGrabStartAction (gameAction.GrabStart!, player, Delegate);
                                break;

                        // Sling Move
                        case GameActionType.GActionType.GrabMove:
                                HandleGrabMove (gameAction.GrabMove!, player, Delegate);
                                break;

                        // Try Release
                        case GameActionType.GActionType.TryRelease:
                                HandleTryReleaseAction (gameAction.TryRelease!, player, Delegate);
                                break;

                        // Inform specific player of release
                        case GameActionType.GActionType.ReleaseEnd:
                                HandleReleaseEndAction (gameAction.ReleaseEnd!, player, Delegate);
                                break;

                        // Update Grabbable Status
                        case GameActionType.GActionType.GrabbableStatus:
                                HandleGrabbableStatus (gameAction.GrabbableStatus!);
                                break;
                }
        }

        #endregion

        #region Handle Actions

        void HandleTryGrabAction (GrabInfo data, Player player, IInteractionDelegate @delegate)
        {
                if (GrabDelegate is null)
                {
                        throw new Exception ("GrabDelegate not set");
                }

                // since we can't send a message only to the server, make sure we're the server
                // when processing.
                if (@delegate.IsServer)
                {
                        // Check if player already owned a grabbable
                        // This is to filter tryGrab messages a player might send because it has not received grab message yet
                        foreach (var value in Grabbables.Values)
                        {
                                if (value.Player is not null && value.Player == player)
                                {
                                        return;
                                }
                        }

                        var grabbable = GrabbableById (data.GrabbableId);
                        GrabDelegate.OnServerGrab (grabbable, data.CameraInfo, player);
                        grabbable.Player = player;

                        // Inform player that the grabbable was grabbed
                        var newData = new GrabInfo (grabbable.GrabbableId, data.CameraInfo);
                        @delegate.DispatchToPlayer (new GameActionType { GrabStart = newData, Type = GameActionType.GActionType.GrabStart }, player);

                        // Update grabbable in the server and clients with a new status
                        // Note: status update only sends the information on whether
                        HandleGrabbableStatus (newData);
                        @delegate.ServerDispatchActionToAll (new GameActionType { GrabbableStatus = newData, Type = GameActionType.GActionType.GrabbableStatus });
                }
        }

        void HandleGrabStartAction (GrabInfo data, Player player, IInteractionDelegate @delegate)
        {
                if (GrabDelegate is null)
                {
                        throw new Exception ("GrabDelegate not set");
                }

                var grabbable = GrabbableById (data.GrabbableId);
                GrabbedGrabbable = grabbable;
                GrabDelegate.OnGrabStart (grabbable, data.CameraInfo, player);
        }

        void HandleTryReleaseAction (GrabInfo data, Player player, IInteractionDelegate @delegate)
        {
                if (GrabDelegate is null || Delegate is null)
                {
                        throw new Exception ("GrabDelegate not set");
                }

                if (Delegate.IsServer)
                {
                        // Launch if player already grabbed a grabbable
                        var grabbable = GrabbableById (data.GrabbableId);
                        if (grabbable.IsGrabbed)
                        {
                                GrabDelegate.OnServerRelease (grabbable, data.CameraInfo, player);

                                // Inform player that the grabbable was grabbed
                                var newData = new GrabInfo (grabbable.GrabbableId, data.CameraInfo);
                                @delegate.DispatchToPlayer (new GameActionType { ReleaseEnd = newData, Type = GameActionType.GActionType.ReleaseEnd }, player);
                        }
                }
        }

        void HandleReleaseEndAction (GrabInfo data, Player player, IInteractionDelegate @delegate)
        {
                isTouching = false;
        }

        void HandleGrabMove (GrabInfo data, Player player, IInteractionDelegate @delegate)
        {
                if (Grabbables.TryGetValue (data.GrabbableId, out IGrabbable? grabbable))
                {
                        grabbable.Move (data.CameraInfo);
                }
        }

        private void HandleGrabbableStatus (GrabInfo status)
        {
                if (GrabDelegate is null)
                {
                        throw new Exception ("GrabDelegate not set");
                }

                if (!Grabbables.TryGetValue (status.GrabbableId, out IGrabbable? grabbable))
                {
                        throw new Exception ($"No Grabbable {status.GrabbableId}");
                }

                grabbable.IsGrabbed = true;
                GrabDelegate.OnUpdateGrabStatus (grabbable, status.CameraInfo);
        }

        #endregion

        #region Helper

        IGrabbable GrabbableById (int grabbableId)
        {
                if (!Grabbables.TryGetValue (grabbableId, out IGrabbable? grabbable))
                {
                        throw new Exception ("Grabbable not found");
                }

                return grabbable;
        }

        #endregion

        public void DidCollision (SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }
}
