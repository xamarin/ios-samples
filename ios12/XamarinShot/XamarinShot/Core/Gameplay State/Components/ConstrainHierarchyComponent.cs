
namespace XamarinShot.Models.GameplayState
{
    using Foundation;
    using GameplayKit;
    using SceneKit;
    using XamarinShot.Utils;
    using System;
    using System.Collections.Generic;

    // goes through hierarchy of node, starting at parent, and checks for constraint_ball nodes
    // which it tries to attach to constraint_socket_nodes with the same suffix.
    public class ConstrainHierarchyComponent : GKComponent, IPhysicsBehaviorComponent
    {
        public const string HingeName = "constraint_hinge";
        public const string JointName = "constraint_ball";
        public const string SocketName = "constraint_attach";

        private const float SearchDist = 0.5f;

        private readonly List<SCNPhysicsBehavior> joints = new List<SCNPhysicsBehavior>();

        public ConstrainHierarchyComponent() : base() { }

        public ConstrainHierarchyComponent(NSCoder coder) => throw new NotImplementedException("init(coder:) has not been implemented");

        public IList<SCNPhysicsBehavior> Behaviors => this.joints;

        /// <summary>
        /// go through hierarchy, find all socket nodes + their corresponding join locations
        /// </summary>
        public void InitBehavior(SCNNode levelRoot, SCNPhysicsWorld world)
        {
            if (this.Entity is GameObject entity)
            {
                var root = entity.ObjectRootNode;
                var systemRoot = root.ParentWithPrefix("_system");
                if (systemRoot != null)
                {
                    // search for ball constraint with name constraint_ball_
                    var ballArray = root.FindAllJoints(ConstrainHierarchyComponent.JointName);
                    foreach (var ballSocket in ballArray)
                    {
                        if (entity.PhysicsNode?.PhysicsBody != null)
                        {
                            entity.PhysicsNode.PhysicsBody.ResetTransform();
                            var socketOffset = ballSocket.ConvertPositionToNode(SCNVector3.Zero, systemRoot);

                            // find in root first
                            var (closestNode, _) = this.FindAttachNodeNearPoint(systemRoot, systemRoot, socketOffset, SearchDist);

                            var attachPhysics = closestNode?.NearestParentGameObject()?.PhysicsNode;
                            var attachBody = attachPhysics?.PhysicsBody;
                            if(attachBody != null)
                            {
                                attachBody.ResetTransform();
                                this.CreateBallJoint(entity.PhysicsNode.PhysicsBody,
                                                     ballSocket.ConvertPositionToNode(SCNVector3.Zero, entity.PhysicsNode),
                                                     attachBody,
                                                     closestNode.ConvertPositionToNode(SCNVector3.Zero, attachPhysics));
                            }
                        }
                    }

                    var hingeArray = root.FindAllJoints(ConstrainHierarchyComponent.HingeName);
                    foreach (var hingeJoint in hingeArray)
                    {
                        if (entity.PhysicsNode?.PhysicsBody != null)
                        {
                            entity.PhysicsNode.PhysicsBody.ResetTransform();
                            var hingeOffset = hingeJoint.ConvertPositionToNode(SCNVector3.Zero, systemRoot);

                            // find in root first
                            var (closestNode, _) = this.FindAttachNodeNearPoint(systemRoot, systemRoot, hingeOffset, SearchDist);

                            var attachPhysics = closestNode?.NearestParentGameObject()?.PhysicsNode;
                            var attachBody = attachPhysics?.PhysicsBody;
                            if (attachBody != null)
                            {
                                attachBody.ResetTransform();
                                this.CreateHingeJoint(entity.PhysicsNode.PhysicsBody,
                                                      hingeJoint.ConvertVectorToNode(SCNVector3.UnitY, entity.PhysicsNode),
                                                      hingeJoint.ConvertPositionToNode(SCNVector3.Zero, entity.PhysicsNode),
                                                      attachBody,
                                                      hingeJoint.ConvertVectorToNode(SCNVector3.UnitY, attachPhysics),
                                                      closestNode.ConvertPositionToNode(SCNVector3.Zero, attachPhysics));
                            }
                        }
                    }

                    foreach(var joint in this.joints)
                    {
                        world.AddBehavior(joint);
                    }
                }
            }
        }

        private void CreateBallJoint(SCNPhysicsBody source, SCNVector3 sourceOffset, SCNPhysicsBody dest, SCNVector3 destOffset)
        {
            var joint = SCNPhysicsBallSocketJoint.Create(source, sourceOffset, dest, destOffset);
            this.joints.Add(joint);
        }

        private void CreateHingeJoint(SCNPhysicsBody source,
                                      SCNVector3 sourceAxis,
                                      SCNVector3 sourceAnchor,
                                      SCNPhysicsBody dest,
                                      SCNVector3 destAxis,
                                      SCNVector3 destAnchor)
        {
            var joint = SCNPhysicsHingeJoint.Create(source, sourceAxis, sourceAnchor, dest, destAxis, destAnchor);
            this.joints.Add(joint);
        }

        private (SCNNode, float) FindAttachNodeNearPoint(SCNNode system, SCNNode node, SCNVector3 point, float tolerance)
        {
            var currentTolerance = tolerance;
            SCNNode currentClosestNode = null;

            // if this object has a socket node near ball node, then use it
            if (!string.IsNullOrEmpty(node.Name) &&
                node.Name.StartsWith((ConstrainHierarchyComponent.SocketName), StringComparison.Ordinal))
            {
                var attachOffset = node.ConvertPositionToNode(SCNVector3.Zero, system);
                var distance = (point - attachOffset).Length;
                if (distance < currentTolerance)
                {
                    currentTolerance = distance;
                    currentClosestNode = node;
                }
            }

            foreach (var child in node.ChildNodes)
            {
                var (socketNode, distance) = FindAttachNodeNearPoint(system, child, point, currentTolerance);
                if (socketNode != null)
                {
                    currentTolerance = distance;
                    currentClosestNode = socketNode;
                }
            }

            return (currentClosestNode, currentTolerance);
        }
    }
}