
namespace XamarinShot.Models
{
    using SceneKit;
    using XamarinShot.Models.Enums;
    using XamarinShot.Utils;
    using System;
    using System.Linq;

    public class HighlightInteraction : IInteraction
    {
        public HighlightInteraction(IInteractionDelegate @delegate)
        {
            this.Delegate = @delegate;
        }

        public IInteractionDelegate Delegate { get; private set; }

        public GrabInteraction GrabInteraction { get; set; }

        public SFXCoordinator SfxCoordinator { get; set; }

        public void Update(CameraInfo cameraInfo)
        {
            if(!UserDefaults.Spectator)
            {
                if(this.GrabInteraction == null)
                {
                    throw new Exception("GrabInteraction not set");
                }

                // Get the current nearest grabbable
                var nearestGrabbable = this.GrabInteraction.GrabbableToGrab(cameraInfo.Ray);
                var grabbables = this.GrabInteraction.Grabbables.Values;

                // If player already grab something, we should turn off all highlight silently
                if (this.GrabInteraction.GrabbedGrabbable != null)
                {
                    foreach(var grabbable in grabbables.Where(item => item.IsHighlighted))
                    {
                        grabbable.DoHighlight(false, null);
                    }

                    return;
                }

                // Turn on/off highlight with sound
                foreach(var grabbable in grabbables)
                {
                    var isNearestGrabbable = false;
                    if (nearestGrabbable != null && 
                        grabbable.GrabbableId == nearestGrabbable.GrabbableId)
                    {
                        isNearestGrabbable = true;
                    }

                    if (isNearestGrabbable && !grabbable.IsHighlighted)
                    {
                        grabbable.DoHighlight(true, this.SfxCoordinator);
                    }
                    else if (!isNearestGrabbable && grabbable.IsHighlighted)
                    {
                        grabbable.DoHighlight(false, this.SfxCoordinator);
                    }
                }
            }
        }

        public void DidCollision(SCNNode node, SCNNode otherNode, SCNVector3 position, float impulse) { }

        public void Handle(GameActionType gameAction, Player player) { }

        public void HandleTouch(TouchType type, Ray camera) { }
    }
}