
namespace XamarinShot.Models
{
    using SceneKit;
    using XamarinShot.Models.Enums;
    using XamarinShot.Models.GameplayState;
    using XamarinShot.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ILeverInteractionDelegate
    {
        bool IsActivated { get; }
        void Activate();
    }

    public class LeverInteraction : IInteraction
    {
        private const float LeverHighlightDistance = 2.5f;
        private const float LeverPullZtoLeverEulerRotation = 1f;
        private const float LeverSpringBackSpeed = 2f;
        private const float LeverMaxEulerX = (float)Math.PI / 6f;

        private readonly List<ResetSwitchComponent> resetSwitches = new List<ResetSwitchComponent>();

        private SCNVector3 startLeverHoldCameraPosition = SCNVector3.Zero;
        private ResetSwitchComponent activeSwitch;
        private ResetSwitchComponent highlightedSwitch;
        private float startLeverEulerX;
        
        public LeverInteraction(IInteractionDelegate @delegate)
        {
            this.Delegate = @delegate;
        }

        public IInteractionDelegate Delegate { get; private set; }
       
        public ILeverInteractionDelegate InteractionToActivate { get; private set; }

        public SFXCoordinator SfxCoordinator { get; set; }

        public void Setup(List<GameObject> resetSwitches, ILeverInteractionDelegate interactionToActivate)
        {
            foreach(var @object in resetSwitches)
            {
                if (@object.GetComponent(typeof(ResetSwitchComponent)) is ResetSwitchComponent resetComponent)
                {
                    this.resetSwitches.Add(resetComponent);
                }
            }

            this.InteractionToActivate = interactionToActivate;
        }

        public void HandleTouch(TouchType type, Ray camera)
        {
            if (type == TouchType.Began)
            {
                if (this.highlightedSwitch != null)
                {
                    this.startLeverHoldCameraPosition = camera.Position;
                    this.startLeverEulerX = this.highlightedSwitch.Angle;
                    this.activeSwitch = this.highlightedSwitch;
                }
            }
            else if (type == TouchType.Ended)
            {
                this.activeSwitch = null;
            }
        }

        public void Handle(GameActionType gameAction, Player player)
        {
            // Move the lever to received position unless this player is already holding a lever
            if (gameAction.Type == GameActionType.GActionType.LeverMove)
            {
                if(this.resetSwitches.Count > gameAction.LeverMove.LeverId)
                {
                    if (this.activeSwitch != this.resetSwitches[gameAction.LeverMove.LeverId])
                    {
                        this.resetSwitches[gameAction.LeverMove.LeverId].Angle = gameAction.LeverMove.EulerAngleX;
                    }
                }
                else 
                {
                    throw new Exception("resetSwitches does not match across network");
                }
            }
        }

        public void Update(CameraInfo cameraInfo)
        {
            if (this.Delegate == null)
            {
                throw new Exception("No delegate");
            }

            // Do not move the lever after it has been activated
            if(!(this.InteractionToActivate?.IsActivated ?? false))
            {
                if (this.activeSwitch != null)
                {
                    // Lever Pulling
                    var cameraOffset = this.activeSwitch.PullOffset(cameraInfo.Ray.Position - this.startLeverHoldCameraPosition);
                    var cameraMovedZ = cameraOffset.Z;

                    var targetEulerX = this.startLeverEulerX + LeverPullZtoLeverEulerRotation * cameraMovedZ;
                    targetEulerX = DigitExtensions.Clamp(-LeverMaxEulerX, targetEulerX, LeverMaxEulerX);
                    this.activeSwitch.Angle = targetEulerX;

                    if (targetEulerX <= -LeverMaxEulerX)
                    {
                        // Interaction activation once the switch lever is turned all the way
                        this.InteractionToActivate?.Activate();

                        // Fade out the switches
                        var waitAction = SCNAction.Wait(3f);
                        var fadeAction = SCNAction.FadeOut(3d);

                        foreach(var resetSwitch in this.resetSwitches)
                        {
                            resetSwitch.Base.RunAction(SCNAction.Sequence(new SCNAction[] { waitAction, fadeAction }));
                        }
                        return;
                    } 
                    else
                    {
                        // Inform peers of the movement
                        var leverId = this.resetSwitches.IndexOf(this.activeSwitch); 
                        if(leverId == -1)
                        {
                            throw new Exception("No lever in array");
                        }

                        this.Delegate.DispatchActionToServer(new GameActionType { LeverMove = new LeverMove(leverId, targetEulerX), Type = GameActionType.GActionType.LeverMove });
                    }
                } 
                else 
                {
                    // Lever spring back
                    foreach(var lever in this.resetSwitches.Where(lever => lever.Angle < LeverMaxEulerX))
                    {
                        lever.Angle = Math.Min(LeverMaxEulerX, lever.Angle + LeverSpringBackSpeed * (float)GameTime.DeltaTime);
                    }
                }

                // Highlight lever when nearby, otherwise check if we should hide the highlight
                if (this.highlightedSwitch != null)
                {
                    if (!this.highlightedSwitch.ShouldHighlight(cameraInfo.Ray))
                    {
                        this.highlightedSwitch.DoHighlight(false, this.SfxCoordinator);
                        this.highlightedSwitch = null;
                    }
                } 
                else 
                {
                    foreach(var resetSwitch in this.resetSwitches )
                    {
                        if (resetSwitch.ShouldHighlight(cameraInfo.Ray))
                        {
                            resetSwitch.DoHighlight(true, this.SfxCoordinator);
                            this.highlightedSwitch = resetSwitch;
                        }
                    }
                }        
            }
        }

        public void DidCollision(SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }
    }
}