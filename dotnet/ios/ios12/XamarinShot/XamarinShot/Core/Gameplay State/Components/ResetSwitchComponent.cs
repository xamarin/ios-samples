
namespace XamarinShot.Models.GameplayState;

public class ResetSwitchComponent : GKComponent, IHighlightableComponent
{
        const float LeverHighlightDistance = 2.5f;

        readonly SCNVector3 leverHoldScale = new SCNVector3 (1.2f, 1.2f, 1.2f);
        SCNNode? highlightObj;
        SCNNode? mirrorObj;
        SCNNode leverObj;

        public ResetSwitchComponent (GameObject entity, SCNNode lever) : base ()
        {
                Base = entity.ObjectRootNode;
                leverObj = lever;

                // find outline node to mirror highlighting
                var highlightNode = entity.ObjectRootNode.FindChildNode ("Highlight", true);
                var mirrorOutline = highlightNode?.FindChildNode ("resetSwitch_leverOutline", true);
                if (highlightNode is not null && mirrorOutline is not null)
                {
                        highlightObj = highlightNode;
                        mirrorObj = mirrorOutline;
                }
        }

        public ResetSwitchComponent (NSCoder coder) => throw new NotImplementedException ("init(coder:) has not been implemented");

        public SCNNode? Base { get; }

        /// <summary>
        /// set the angle of the lever here
        /// </summary>
        public float Angle
        {
                get
                {
                        return leverObj.EulerAngles.X;
                }

                set
                {
                        leverObj.EulerAngles = new SCNVector3 (value,
                                leverObj.EulerAngles.Y, leverObj.EulerAngles.Z);


                        // apply to outline component
                        if (mirrorObj is not null)
                        {
                                mirrorObj.EulerAngles = new SCNVector3 (value,
                                        leverObj.EulerAngles.Y, leverObj.EulerAngles.Z);
                        }
                }
        }

        public bool IsHighlighted => highlightObj is not null && !highlightObj.Hidden;

        /// <summary>
        /// Convenience function to return which side of center the lever is on, so we can flip the
        /// </summary>
        public SCNVector3 PullOffset (SCNVector3 cameraOffset)
        {
                return Base.ConvertVectorFromNode (cameraOffset, null);
        }

        #region IHighlightableComponent

        public bool ShouldHighlight (Ray camera)
        {
                var cameraToButtonDistance = (leverObj.WorldPosition - camera.Position).Length;
                return cameraToButtonDistance <= LeverHighlightDistance;
        }

        /// <summary>
        /// Enable/disable the highlight on this object
        /// </summary>
        public void DoHighlight (bool show, SFXCoordinator? sfxCoordinator)
        {
                // turn off
                if (!show)
                {
                        leverObj.Scale = SCNVector3.One;

                        if (highlightObj is not null)
                        {
                                highlightObj.Hidden = true;
                        }

                        if (mirrorObj is not null)
                        {
                                mirrorObj.Scale = SCNVector3.One;
                        }

                        sfxCoordinator?.PlayLeverHighlight (false);
                } else {
                        // turn on
                        leverObj.Scale = leverHoldScale;

                        if (highlightObj is not null)
                        {
                                highlightObj.Hidden = false;
                        }

                        if (mirrorObj is not null)
                        {
                                mirrorObj.Scale = leverHoldScale;
                        }

                        sfxCoordinator?.PlayLeverHighlight (highlighted: true);
                }
        }

        #endregion
}
