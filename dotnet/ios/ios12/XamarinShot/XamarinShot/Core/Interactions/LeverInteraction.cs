using XamarinShot.Models.GameplayState;

namespace XamarinShot.Models;

public interface ILeverInteractionDelegate
{
        bool IsActivated { get; }
        void Activate ();
}

public class LeverInteraction : IInteraction
{
        const float LeverHighlightDistance = 2.5f;
        const float LeverPullZtoLeverEulerRotation = 1f;
        const float LeverSpringBackSpeed = 2f;
        const float LeverMaxEulerX = (float)Math.PI / 6f;

        readonly List<ResetSwitchComponent> resetSwitches = new List<ResetSwitchComponent> ();

        SCNVector3 startLeverHoldCameraPosition = SCNVector3.Zero;
        ResetSwitchComponent? activeSwitch;
        ResetSwitchComponent? highlightedSwitch;
        float startLeverEulerX;

        public LeverInteraction (IInteractionDelegate @delegate)
        {
                Delegate = @delegate;
        }

        public IInteractionDelegate? Delegate { get; private set; }

        public ILeverInteractionDelegate? InteractionToActivate { get; private set; }

        public SFXCoordinator? SfxCoordinator { get; set; }

        public void Setup (List<GameObject> resetSwitches, ILeverInteractionDelegate interactionToActivate)
        {
                foreach (var @object in resetSwitches)
                {
                        if (@object.GetComponent (typeof (ResetSwitchComponent)) is ResetSwitchComponent resetComponent)
                        {
                                this.resetSwitches.Add (resetComponent);
                        }
                }

                InteractionToActivate = interactionToActivate;
        }

        public void HandleTouch (TouchType type, Ray camera)
        {
                if (type == TouchType.Began)
                {
                        if (highlightedSwitch is not null)
                        {
                                startLeverHoldCameraPosition = camera.Position;
                                startLeverEulerX = highlightedSwitch.Angle;
                                activeSwitch = highlightedSwitch;
                        }
                }
                else if (type == TouchType.Ended)
                {
                        activeSwitch = null;
                }
        }

        public void Handle (GameActionType gameAction, Player player)
        {
                // Move the lever to received position unless this player is already holding a lever
                if (gameAction.Type == GameActionType.GActionType.LeverMove && gameAction.LeverMove is not null)
                {
                        if (resetSwitches.Count > gameAction.LeverMove.LeverId)
                        {
                                if (activeSwitch != resetSwitches [gameAction.LeverMove.LeverId])
                                {
                                        resetSwitches [gameAction.LeverMove.LeverId].Angle = gameAction.LeverMove.EulerAngleX;
                                }
                        } else {
                                throw new Exception ("resetSwitches does not match across network");
                        }
                }
        }

        public void Update (CameraInfo cameraInfo)
        {
                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                // Do not move the lever after it has been activated
                if (!(InteractionToActivate?.IsActivated ?? false))
                {
                        if (activeSwitch is not null)
                        {
                                // Lever Pulling
                                var cameraOffset = activeSwitch.PullOffset (cameraInfo.Ray.Position - startLeverHoldCameraPosition);
                                var cameraMovedZ = cameraOffset.Z;

                                var targetEulerX = startLeverEulerX + LeverPullZtoLeverEulerRotation * cameraMovedZ;
                                targetEulerX = DigitExtensions.Clamp (-LeverMaxEulerX, targetEulerX, LeverMaxEulerX);
                                activeSwitch.Angle = targetEulerX;

                                if (targetEulerX <= -LeverMaxEulerX)
                                {
                                        // Interaction activation once the switch lever is turned all the way
                                        InteractionToActivate?.Activate ();

                                        // Fade out the switches
                                        var waitAction = SCNAction.Wait (3f);
                                        var fadeAction = SCNAction.FadeOut (3d);

                                        foreach (var resetSwitch in resetSwitches)
                                        {
                                                if (resetSwitch.Base is null)
                                                        continue;
                                                resetSwitch.Base.RunAction (SCNAction.Sequence (new SCNAction [] { waitAction, fadeAction }));
                                        }
                                        return;
                                } else {
                                        // Inform peers of the movement
                                        var leverId = resetSwitches.IndexOf (activeSwitch);
                                        if (leverId == -1)
                                        {
                                                throw new Exception ("No lever in array");
                                        }

                                        Delegate.DispatchActionToServer (new GameActionType { LeverMove = new LeverMove (leverId, targetEulerX), Type = GameActionType.GActionType.LeverMove });
                                }
                        } else {
                                // Lever spring back
                                foreach (var lever in resetSwitches.Where (lever => lever.Angle < LeverMaxEulerX))
                                {
                                        lever.Angle = Math.Min (LeverMaxEulerX, lever.Angle + LeverSpringBackSpeed * (float)GameTime.DeltaTime);
                                }
                        }

                        // Highlight lever when nearby, otherwise check if we should hide the highlight
                        if (highlightedSwitch is not null)
                        {
                                if (!highlightedSwitch.ShouldHighlight (cameraInfo.Ray))
                                {
                                        highlightedSwitch.DoHighlight (false, SfxCoordinator);
                                        highlightedSwitch = null;
                                }
                        }
                        else
                        {
                                foreach (var resetSwitch in resetSwitches)
                                {
                                        if (resetSwitch.ShouldHighlight (cameraInfo.Ray))
                                        {
                                                resetSwitch.DoHighlight (true, SfxCoordinator);
                                                highlightedSwitch = resetSwitch;
                                        }
                                }
                        }
                }
        }

        public void DidCollision (SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }
}
