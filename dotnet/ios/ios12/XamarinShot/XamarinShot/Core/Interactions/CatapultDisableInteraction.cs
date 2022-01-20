namespace XamarinShot.Models;

public class CatapultDisableInteraction : IInteraction
{
        const double CatapultUnstableTimeUntilDisable = 3d;

        public CatapultDisableInteraction (IInteractionDelegate @delegate)
        {
                Delegate = @delegate;

                // Client should try to request the initial catapult disable state from server
                if (!Delegate.IsServer)
                {
                        Delegate.DispatchActionToServer (new GameActionType { Type = GameActionType.GActionType.KnockoutSync });
                }
        }

        public IInteractionDelegate Delegate { get; private set; }

        public void Update (CameraInfo cameraInfo)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                foreach (var catapult in Delegate.Catapults)
                {
                        // Check and disable knocked catapults
                        if (!catapult.Disabled && catapult.CatapultKnockedTime > CatapultUnstableTimeUntilDisable)
                        {
                                var knockoutInfo = new HitCatapult (catapult.CatapultId, true, false);
                                Delegate.DispatchActionToAll (new GameActionType { CatapultKnockOut = knockoutInfo, Type = GameActionType.GActionType.HitCatapult });
                        }
                }
        }

        #region Game Action Handling

        public void Handle (GameActionType gameAction, Player player)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                if (gameAction.Type == GameActionType.GActionType.HitCatapult)
                {
                        var gameActionKnockout = gameAction.CatapultKnockOut;
                        if (gameActionKnockout is not null)
                        {
                                var catapult = Delegate.Catapults.FirstOrDefault (item => item.CatapultId == gameActionKnockout.CatapultId);
                                if (catapult is not null)
                                {
                                        if (!catapult.Disabled)
                                        {
                                                catapult.ProcessKnockOut (gameActionKnockout);
                                                catapult.IsGrabbed = false;
                                        }
                                }
                        }
                } else if (gameAction.Type == GameActionType.GActionType.KnockoutSync && Delegate.IsServer)
                {
                        // Server will dispatch catapult knockout messages to all clients to make sure knockout states are in sync
                        foreach (var catapult in Delegate.Catapults.Where (catapult => catapult.Disabled))
                        {
                                var knockoutInfo = new HitCatapult (catapult.CatapultId, false, false);
                                Delegate.DispatchActionToAll (new GameActionType { CatapultKnockOut = knockoutInfo, Type = GameActionType.GActionType.HitCatapult });
                        }
                }
        }

        public void HandleTouch (TouchType type, Ray camera) { }

        #endregion

        public void DidCollision (SCNNode node, SCNNode otherNode, SCNVector3 pos, float impulse) { }
}
