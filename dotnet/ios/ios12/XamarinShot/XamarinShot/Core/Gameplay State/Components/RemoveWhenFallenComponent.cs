namespace XamarinShot.Models.GameplayState;

/// <summary>
/// Removes nodes from the scene when they fall out of bounds.
/// </summary>
public class RemoveWhenFallenComponent : GKComponent
{
        readonly SCNVector3 MinBounds = new SCNVector3 (-80f, -10f, -80f); // -10.0 represents 1.0 meter high table
        readonly SCNVector3 MaxBounds = new SCNVector3 (80f, 1000f, 80f);

        public override void Update (double deltaTimeInSeconds)
        {
                if (GameTime.FrameCount % 6 != 0)
                {
                        if (Entity is GameObject gameObject && gameObject?.PhysicsNode is not null)
                        {
                                // check past min/max bounds
                                // the border was chosen experimentally to see what feels good
                                var position = gameObject.PhysicsNode.PresentationNode.WorldPosition;

                                // this is only checking position, but bounds could be offset or bigger
                                var shouldRemove = SCNVector3.ComponentMin (position, MinBounds) != MinBounds ||
                                                   SCNVector3.ComponentMax (position, MaxBounds) != MaxBounds;
                                if (shouldRemove)
                                {
                                        gameObject.Disable ();
                                }
                        }
                }
        }
}
