
namespace XamarinShot.Models.GameplayState
{
    using SceneKit;
    using System.Collections.Generic;

    /// <summary>
    /// Components that imply the object can be touched
    /// </summary>
    public interface ITouchableComponent { }

    /// <summary>
    /// Components that indicate the object should exhibit highlight behavior based on camera distance + orientation
    /// </summary>
    public interface IHighlightableComponent
    {
        bool IsHighlighted { get; }

        bool ShouldHighlight(Ray camera);

        void DoHighlight(bool show, SFXCoordinator sfxCoordinator);
    }

    /// <summary>
    /// Components that allow require special handling during collisions
    /// </summary>
    public interface ICollisionHandlerComponent
    {
        // node is the node associated with this CollisionHandlerComponent.
        void DidCollision(GameManager manager, SCNNode node, SCNNode otherNode, SCNVector3 pos, float impulse);
    }

    /// <summary>
    /// Components that require setup for scenekit's SCNPhysicsBehavior to work
    /// </summary>
    public interface IPhysicsBehaviorComponent
    {
        IList<SCNPhysicsBehavior> Behaviors { get; }

        void InitBehavior(SCNNode levelRoot, SCNPhysicsWorld world);
    }
}