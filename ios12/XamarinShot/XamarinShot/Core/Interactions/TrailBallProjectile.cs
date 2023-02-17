
namespace XamarinShot.Models {
	using Foundation;
	using SceneKit;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class TrailBallProjectile : Projectile {
		public const float DefaultTrailWidth = 0.5f; // Default trail with with ball size as the unit (1.0 represents same width as the ball)

		public const int DefaultTrailLength = 108;

		private const float Epsilon = 1.19209290E-07f; // upper limit on float rounding error

		// default values the result of a vast user study.
		private const float BallSize = 0.275f;

		private readonly List<SCNVector3> worldPositions = new List<SCNVector3> ();

		private readonly List<SCNVector3> tempWorldPositions = new List<SCNVector3> ();

		private readonly List<SCNVector4> colors = new List<SCNVector4> ();

		private readonly SCNNode trailNode = new SCNNode ();


		public TrailBallProjectile () { }

		public TrailBallProjectile (SCNNode node, int? index, Dictionary<string, object> gamedefs, bool isAlive, bool isServer) : base (node, index, gamedefs, isAlive, isServer) { }


		public float TrailHalfWidth => (UserDefaults.TrailWidth ?? TrailBallProjectile.DefaultTrailWidth) * BallSize * 0.5f;

		public int MaxTrailPositions => UserDefaults.TrailLength ?? TrailBallProjectile.DefaultTrailLength;

		public bool TrailShouldNarrow => UserDefaults.TrailShouldNarrow;

		public static TrailBallProjectile Create (SCNNode prototypeNode, int? index, Dictionary<string, object> gamedefs) { return Projectile.Create<TrailBallProjectile> (prototypeNode, index, gamedefs); }

		public static TrailBallProjectile Create (SCNNode prototypeNode) { return Projectile.Create<TrailBallProjectile> (prototypeNode); }

		public override void Launch (GameVelocity velocity, double lifeTime, IProjectileDelegate @delegate)
		{
			base.Launch (velocity, lifeTime, @delegate);
			this.AddTrail ();
		}

		public override void OnSpawn ()
		{
			base.OnSpawn ();
			this.AddTrail ();
		}

		public override void Despawn ()
		{
			base.Despawn ();
			this.RemoveTrail ();
		}

		private void AddTrail ()
		{
			if (this.PhysicsNode != null) {
				this.trailNode.CastsShadow = false;
				if (this.PhysicsNode.PhysicsBody != null) {
					this.PhysicsNode.PhysicsBody.AngularDamping = 1f;
				}

				if (this.Delegate != null) {
					this.Delegate.AddNodeToLevel (this.trailNode);
				}
			}
		}

		private void RemoveTrail ()
		{
			this.trailNode.RemoveFromParentNode ();
		}

		public override void OnDidApplyConstraints (ISCNSceneRenderer renderer)
		{
			var frameSkips = 3;
			if ((GameTime.FrameCount + this.Index) % frameSkips == 0) {
				if (this.PhysicsNode != null) {
					if (this.worldPositions.Count > (this.MaxTrailPositions / frameSkips)) {
						this.RemoveVerticesPair ();
					}

					var position = this.PhysicsNode.PresentationNode.WorldPosition;

					SCNVector3 trailDir;
					if (this.worldPositions.Any ()) {
						var previousPosition = this.worldPositions.LastOrDefault ();
						trailDir = position - previousPosition;

						var lengthSquared = trailDir.LengthSquared;
						if (lengthSquared < Epsilon) {
							this.RemoveVerticesPair ();
							this.UpdateColors ();
							var tempPositions = this.tempWorldPositions.Select (tempPosition => this.trailNode.PresentationNode.ConvertPositionFromNode (tempPosition, null)).ToList ();
							this.trailNode.PresentationNode.Geometry = this.CreateTrailMesh (tempPositions, this.colors);
							return;
						}

						trailDir = SCNVector3.Normalize (trailDir);
					} else {
						trailDir = this.ObjectRootNode.WorldFront;
					}

					var right = SCNVector3.Cross (SCNVector3.UnitY, trailDir);
					right = SCNVector3.Normalize (right);
					var scale = 1f; //Float(i - 1) / worldPositions.count
					var halfWidth = this.TrailHalfWidth;
					if (this.TrailShouldNarrow) {
						halfWidth *= scale;
					}

					var u = position + right * halfWidth;
					var v = position - right * halfWidth;

					this.worldPositions.Add (position);
					this.tempWorldPositions.Add (u);
					this.tempWorldPositions.Add (v);

					this.colors.Add (SCNVector4.Zero);
					this.colors.Add (SCNVector4.Zero);

					this.UpdateColors ();
					var localPositions = this.tempWorldPositions.Select (tempPosition => this.trailNode.PresentationNode.ConvertPositionFromNode (tempPosition, null)).ToList ();
					this.trailNode.PresentationNode.Geometry = this.CreateTrailMesh (localPositions, this.colors);
				}
			}
		}

		private void RemoveVerticesPair ()
		{
			this.worldPositions.RemoveAt (0);
			this.tempWorldPositions.RemoveRange (0, 2);
			this.colors.RemoveRange (0, 2);
		}

		private void UpdateColors ()
		{
			var baseColor = SimdExtensions.CreateVector4 (this.Team.GetColor ());
			for (var i = 0; i < this.colors.Count; i++) {
				var scale = (float) i / (float) this.colors.Count;
				this.colors [i] = baseColor * scale;
			}
		}

		private SCNGeometry CreateTrailMesh (List<SCNVector3> positions, List<SCNVector4> colors)
		{
			SCNGeometry result = null;
			if (positions.Count >= 4) {
				var array = new byte [positions.Count];
				for (byte i = 0; i < positions.Count; i++) {
					array [i] = i;
				}

				var positionSource = SCNGeometrySource.FromVertices (positions.ToArray ());
				var colorSource = SCNGeometrySourceExtensions.Create (colors);
				var element = SCNGeometryElement.FromData (NSData.FromArray (array), SCNGeometryPrimitiveType.TriangleStrip, positions.Count - 2, 2);
				result = SCNGeometry.Create (new SCNGeometrySource [] { positionSource, colorSource }, new SCNGeometryElement [] { element });

				var material = result.FirstMaterial;
				if (material == null) {
					throw new Exception ("created geometry without material");
				}

				material.DoubleSided = true;
				material.LightingModelName = SCNLightingModel.Constant;
				material.BlendMode = SCNBlendMode.Add;
				material.WritesToDepthBuffer = false;
			}

			return result;
		}
	}
}
