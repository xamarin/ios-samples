
namespace XamarinShot.Models
{
    using SceneKit;
    using XamarinShot.Models.Enums;
    using System;
    using System.Linq;

    public class CatapultDisableInteraction : IInteraction
    {
        private const double CatapultUnstableTimeUntilDisable = 3d;

        public CatapultDisableInteraction(IInteractionDelegate @delegate)
        {
            this.Delegate = @delegate;

            // Client should try to request the initial catapult disable state from server
            if (!this.Delegate.IsServer)
            {
                this.Delegate.DispatchActionToServer(new GameActionType { Type = GameActionType.GActionType.KnockoutSync });
            }
        }

        public IInteractionDelegate Delegate { get; private set; }

        public void Update(CameraInfo cameraInfo)
        {
            if(this.Delegate == null)
            {
                throw new Exception("No Delegate");
            }

            foreach(var catapult in this.Delegate.Catapults)
            {
                // Check and disable knocked catapults
                if (!catapult.Disabled && catapult.CatapultKnockedTime > CatapultUnstableTimeUntilDisable)
                {
                    var knockoutInfo = new HitCatapult(catapult.CatapultId, true, false);
                    this.Delegate.DispatchActionToAll(new GameActionType { CatapultKnockOut = knockoutInfo, Type = GameActionType.GActionType.HitCatapult });
                }
            }
        }

        #region Game Action Handling

        public void Handle(GameActionType gameAction, Player player)
        {
            if (this.Delegate == null)
            {
                throw new Exception("No Delegate");
            }

            if(gameAction.Type == GameActionType.GActionType.HitCatapult)
            {
                var catapult = this.Delegate.Catapults.FirstOrDefault(item => item.CatapultId == gameAction.CatapultKnockOut.CatapultId);
                if(catapult != null)
                {
                    if (!catapult.Disabled)
                    {
                        catapult.ProcessKnockOut(gameAction.CatapultKnockOut);
                        catapult.IsGrabbed = false;
                    }
                }
            }
            else if (gameAction.Type == GameActionType.GActionType.KnockoutSync && this.Delegate.IsServer)
            {
                // Server will dispatch catapult knockout messages to all clients to make sure knockout states are in sync
                foreach (var catapult in this.Delegate.Catapults.Where(catapult => catapult.Disabled))
                {
                    var knockoutInfo = new HitCatapult(catapult.CatapultId, false, false);
                    this.Delegate.DispatchActionToAll(new GameActionType { CatapultKnockOut = knockoutInfo, Type = GameActionType.GActionType.HitCatapult });
                }
            }
        }

        public void HandleTouch(TouchType type, Ray camera) { }

        #endregion

        public void DidCollision(SCNNode node, SCNNode otherNode, SCNVector3 pos, float impulse) { }
    }
}