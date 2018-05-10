
namespace Fox2.Extensions
{
    using GameplayKit;
    using OpenTK;
    using SceneKit;
    using System;

    public static class GKAgent2DExtensions
    {
        public static SCNMatrix4 GetTransform(this GKAgent2D agent)
        {
            var quat = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), -(agent.Rotation - ((float)Math.PI / 2f)));
            var transform = SCNMatrix4.Rotate(quat);

            transform.M41 = agent.Position.X;
            transform.M42 = BaseComponent.EnemyAltitude;
            transform.M43 = agent.Position.Y;
            transform.M44 = 1f;

            return transform;
        }

        public static void SetTransform(this GKAgent2D agent, SCNMatrix4 newTransform)
        {
            var quatf = new SCNQuaternion(newTransform.Column3.Xyz, newTransform.Column3.W);

            SCNVector3 axis;
           
#if !__OSX__
            float angle;
            quatf.ToAxisAngle(out axis, out angle);
            agent.Rotation = -(angle + ((float)Math.PI / 2f));
            agent.Position = new Vector2(newTransform.M41, newTransform.M43);
#else
            nfloat angle;
            quatf.ToAxisAngle(out axis, out angle);
            agent.Rotation = -((float)angle + ((float)Math.PI / 2f));
            agent.Position = new Vector2((float)newTransform.M41, (float)newTransform.M43);
#endif
        }
    }
}
