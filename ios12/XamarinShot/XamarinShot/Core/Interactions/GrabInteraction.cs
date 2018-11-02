
namespace XamarinShot.Models
{
    using SceneKit;
    using XamarinShot.Models.Enums;
    using XamarinShot.Models.Interactions;
    using System;
    using System.Collections.Generic;

    public interface IGrabInteractionDelegate
    {
        bool ShouldForceRelease(IGrabbable grabbable);
        void OnServerGrab(IGrabbable grabbable, CameraInfo cameraInfo, Player player);
        void OnGrabStart(IGrabbable grabbable, CameraInfo cameraInfo, Player player);
        void OnServerRelease(IGrabbable grabbable, CameraInfo cameraInfo, Player player);
        void OnUpdateGrabStatus(IGrabbable grabbable, CameraInfo cameraInfo);
    }

    public class GrabInteraction : IInteraction
    {
        private bool isTouching;

        // Index used to assign to new object
        private int currentIndex = 0;

        public GrabInteraction(IInteractionDelegate @delegate)
        {
            this.Delegate = @delegate;
        }

        public IInteractionDelegate Delegate { get; private set; }

        public IGrabInteractionDelegate GrabDelegate { get; set; }

        public IGrabbable GrabbedGrabbable { get; set; }

        public Dictionary<int, IGrabbable> Grabbables { get; private set; } = new Dictionary<int, IGrabbable>();

        /// <summary>
        /// Should be added only from the classes using GrabInteraction to prevent duplicates
        /// </summary>
        public void AddGrabbable(IGrabbable grabbable)
        {
            grabbable.GrabbableId = this.currentIndex;
            this.Grabbables[this.currentIndex] = grabbable;
            this.currentIndex += 1;
        }

        public void HandleTouch(TouchType type, Ray camera)
        {
            if (type == TouchType.Began)
            {
                if (this.GrabbableToGrab(camera) != null)
                {
                    this.isTouching = true;
                }
            }
            else if (type == TouchType.Ended)
            {
                this.isTouching = false;
            }
        }

        public void Update(CameraInfo cameraInfo)
        {
            if(this.Delegate == null || this.GrabDelegate == null)
            {
                throw new Exception("No Delegate");
            }

            // Dispatch grab action to server so that server can test if grab succeed.
            // If grab succeed, server would update the grabbable's player for all clients
            // Note:
            // This check is done in update to counter the case of network lag
            // if touch down and up is very quick, the shouldTryRelease might not trigger right at touch up,
            // because information on grabbed grabbableID might not arrived yet
            if (this.isTouching && this.GrabbedGrabbable == null)
            {
                if (GameTime.FrameCount % 3 == 0) // Only send messages at 20 fps to save bandwidth
                {
                    // Send grab message to server if player can grab something
                    var grabbable = this.GrabbableToGrab(cameraInfo.Ray);
                    if (grabbable != null)
                    {
                        var grab = new GrabInfo(grabbable.GrabbableId, cameraInfo);
                        this.Delegate.DispatchActionToServer(new GameActionType { TryGrab = grab, Type = GameActionType.GActionType.TryGrab });
                        return;
                    }
                }
            }

            if (this.GrabbedGrabbable != null)
            {
                if (!this.Delegate.IsServer)
                {
                    // Client move the sling locally, ignore server's physics data to prevent lag
                    this.GrabbedGrabbable.Move(cameraInfo);
                }

                // If touch is up or the sling is pulled too far, release the ball
                if (!this.isTouching || this.GrabDelegate.ShouldForceRelease(this.GrabbedGrabbable))
                {
                    if (GameTime.FrameCount % 3 == 0)// Only send messages at 20 fps to save bandwidth
                    {
                        var grab = new GrabInfo(this.GrabbedGrabbable.GrabbableId, cameraInfo);
                        this.Delegate.DispatchActionToServer(new GameActionType { TryRelease = grab, Type = GameActionType.GActionType.TryRelease });
                        return;
                    }
                }

                // Dispatch slingMove to server.
                var data = new GrabInfo(this.GrabbedGrabbable.GrabbableId, cameraInfo);
                this.Delegate.DispatchActionToServer(new GameActionType { GrabMove = data, Type = GameActionType.GActionType.GrabMove });
            }
        }

        private IGrabbable NearestVisibleGrabbable(Ray cameraRay) 
        {
            // Find closest visible grabbable
            IGrabbable closestGrabbable = null;
            var closestDist = 0f;
            foreach(var grabbable in this.Grabbables.Values)
            {
                if (grabbable.IsVisible)
                {
                    var distance = grabbable.DistanceFrom(cameraRay.Position);
                    if (closestGrabbable == null || distance < closestDist)
                    {
                        closestGrabbable = grabbable;
                        closestDist = distance;
                    }
                }
            }

            return closestGrabbable;
        }

        public IGrabbable GrabbableToGrab(Ray cameraRay) 
        {
            IGrabbable result = null;

            var grabbable = this.NearestVisibleGrabbable(cameraRay);
            if(grabbable != null)
            {
                if(grabbable.CanGrab(cameraRay))
                {
                    result = grabbable;
                }
            }

            return result;
        }

        #region Interactions

        public void Handle(GameActionType gameAction, Player player)
        {
            if (this.Delegate == null)
            {
                throw new Exception("No delegate");
            }

            switch (gameAction.Type)
            {
                // Try Grab
                case GameActionType.GActionType.TryGrab:
                    this.HandleTryGrabAction(gameAction.TryGrab, player, this.Delegate);
                    break;

                // Inform specific player of grab, when it succeeds
                case GameActionType.GActionType.GrabStart:
                    this.HandleGrabStartAction(gameAction.GrabStart, player, this.Delegate);
                    break;

                // Sling Move
                case GameActionType.GActionType.GrabMove:
                    this.HandleGrabMove(gameAction.GrabMove, player, this.Delegate);
                    break;

                // Try Release
                case GameActionType.GActionType.TryRelease:
                    this.HandleTryReleaseAction(gameAction.TryRelease, player, this.Delegate);
                    break;

                // Inform specific player of release
                case GameActionType.GActionType.ReleaseEnd:
                    this.HandleReleaseEndAction(gameAction.ReleaseEnd, player, this.Delegate);
                    break;

                // Update Grabbable Status
                case GameActionType.GActionType.GrabbableStatus:
                    this.HandleGrabbableStatus(gameAction.GrabbableStatus);
                    break;
            }
        }

        #endregion

        #region Handle Actions

        private void HandleTryGrabAction(GrabInfo data, Player player, IInteractionDelegate @delegate)
        {
			if(this.GrabDelegate == null)
			{
                throw new Exception("GrabDelegate not set");
            }

            // since we can't send a message only to the server, make sure we're the server
            // when processing.
            if (@delegate.IsServer)
            {
                // Check if player already owned a grabbable
                // This is to filter tryGrab messages a player might send because it has not received grab message yet
                foreach(var value in this.Grabbables.Values)
                {
                    if (value.Player != null && value.Player == player)
                    {
                        return;
                    }
                }

                var grabbable = this.GrabbableById(data.GrabbableId);
                this.GrabDelegate.OnServerGrab(grabbable, data.CameraInfo, player);
                grabbable.Player = player;

                // Inform player that the grabbable was grabbed
                var newData = new GrabInfo(grabbable.GrabbableId, data.CameraInfo);
                @delegate.DispatchToPlayer(new GameActionType { GrabStart = newData, Type = GameActionType.GActionType.GrabStart }, player);

                // Update grabbable in the server and clients with a new status
                // Note: status update only sends the information on whether
                this.HandleGrabbableStatus(newData);
                @delegate.ServerDispatchActionToAll(new GameActionType { GrabbableStatus = newData, Type = GameActionType.GActionType.GrabbableStatus });
            }
        }

        private void HandleGrabStartAction(GrabInfo data, Player player, IInteractionDelegate @delegate)
        {
            if(this.GrabDelegate == null)
            {
                throw new Exception("GrabDelegate not set");
            }

            var grabbable = this.GrabbableById(data.GrabbableId);
            this.GrabbedGrabbable = grabbable;
            this.GrabDelegate.OnGrabStart(grabbable, data.CameraInfo, player);
        }

        private void HandleTryReleaseAction(GrabInfo data, Player player, IInteractionDelegate @delegate)
        {
            if(this.GrabDelegate == null)
            {
                throw new Exception("GrabDelegate not set");
            }

            if (this.Delegate.IsServer)
            {
                // Launch if player already grabbed a grabbable
                var grabbable = this.GrabbableById(data.GrabbableId);
                if (grabbable.IsGrabbed)
                {
                    this.GrabDelegate.OnServerRelease(grabbable, data.CameraInfo, player);

                    // Inform player that the grabbable was grabbed
                    var newData = new GrabInfo(grabbable.GrabbableId, data.CameraInfo);
                    @delegate.DispatchToPlayer(new GameActionType { ReleaseEnd = newData, Type = GameActionType.GActionType.ReleaseEnd }, player);
                }
            }
        }

        private void HandleReleaseEndAction(GrabInfo data, Player player, IInteractionDelegate @delegate) 
        {
            this.isTouching = false;
        }

        private void HandleGrabMove(GrabInfo data, Player player, IInteractionDelegate @delegate) 
        {
            if (this.Grabbables.TryGetValue(data.GrabbableId, out IGrabbable grabbable))
            {
                grabbable.Move(data.CameraInfo);
            }
        }

        private void HandleGrabbableStatus(GrabInfo status)
        {
            if(this.GrabDelegate == null)
            {
                throw new Exception("GrabDelegate not set");
            }

            if(!this.Grabbables.TryGetValue(status.GrabbableId, out IGrabbable grabbable))
            {
                throw new Exception($"No Grabbable {status.GrabbableId}");
            }

            grabbable.IsGrabbed = true;
            this.GrabDelegate.OnUpdateGrabStatus(grabbable, status.CameraInfo);
        }

        #endregion

        #region Helper

        private IGrabbable GrabbableById(int grabbableId) 
        {
            if(!this.Grabbables.TryGetValue(grabbableId, out IGrabbable grabbable))
            {
                throw new Exception("Grabbable not found");
            }

            return grabbable;
        }

        #endregion

        public void DidCollision(SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }
    }
}