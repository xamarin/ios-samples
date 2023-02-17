
namespace XamarinShot.Models.GameplayState {
	using Foundation;
	using GameplayKit;
	using SceneKit;
	using System;

	public class ResetSwitchComponent : GKComponent, IHighlightableComponent {
		private const float LeverHighlightDistance = 2.5f;

		private readonly SCNVector3 leverHoldScale = new SCNVector3 (1.2f, 1.2f, 1.2f);
		private SCNNode highlightObj;
		private SCNNode mirrorObj;
		private SCNNode leverObj;

		public ResetSwitchComponent (GameObject entity, SCNNode lever) : base ()
		{
			this.Base = entity.ObjectRootNode;
			this.leverObj = lever;

			// find outline node to mirror highlighting
			var highlightNode = entity.ObjectRootNode.FindChildNode ("Highlight", true);
			var mirrorOutline = highlightNode.FindChildNode ("resetSwitch_leverOutline", true);
			if (highlightNode != null && mirrorOutline != null) {
				this.highlightObj = highlightNode;
				this.mirrorObj = mirrorOutline;
			}
		}

		public ResetSwitchComponent (NSCoder coder) => throw new NotImplementedException ("init(coder:) has not been implemented");

		public SCNNode Base { get; }

		/// <summary>
		/// set the angle of the lever here
		/// </summary>
		public float Angle {
			get {
				return this.leverObj.EulerAngles.X;
			}

			set {
				this.leverObj.EulerAngles = new SCNVector3 (value,
														   this.leverObj.EulerAngles.Y,
														   this.leverObj.EulerAngles.Z);


				// apply to outline component
				if (this.mirrorObj != null) {
					this.mirrorObj.EulerAngles = new SCNVector3 (value,
																this.leverObj.EulerAngles.Y,
																this.leverObj.EulerAngles.Z);
				}
			}
		}

		public bool IsHighlighted => this.highlightObj != null && !this.highlightObj.Hidden;

		/// <summary>
		/// Convenience function to return which side of center the lever is on, so we can flip the
		/// </summary>
		public SCNVector3 PullOffset (SCNVector3 cameraOffset)
		{
			return this.Base.ConvertVectorFromNode (cameraOffset, null);
		}

		#region IHighlightableComponent

		public bool ShouldHighlight (Ray camera)
		{
			var cameraToButtonDistance = (this.leverObj.WorldPosition - camera.Position).Length;
			return cameraToButtonDistance <= LeverHighlightDistance;
		}

		/// <summary>
		/// Enable/disable the highlight on this object
		/// </summary>
		public void DoHighlight (bool show, SFXCoordinator sfxCoordinator)
		{
			// turn off
			if (!show) {
				this.leverObj.Scale = SCNVector3.One;

				if (this.highlightObj != null) {
					this.highlightObj.Hidden = true;
				}

				if (this.mirrorObj != null) {
					this.mirrorObj.Scale = SCNVector3.One;
				}

				sfxCoordinator?.PlayLeverHighlight (false);
			} else {
				// turn on
				this.leverObj.Scale = this.leverHoldScale;

				if (this.highlightObj != null) {
					this.highlightObj.Hidden = false;
				}

				if (this.mirrorObj != null) {
					this.mirrorObj.Scale = this.leverHoldScale;
				}

				sfxCoordinator?.PlayLeverHighlight (highlighted: true);
			}
		}

		#endregion
	}
}
