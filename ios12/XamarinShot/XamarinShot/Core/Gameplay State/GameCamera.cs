
namespace XamarinShot.Models {
	using SceneKit;
	using XamarinShot.Utils;
	using System;

	public class GameCamera {
		private readonly GameCameraProps properties = new GameCameraProps ();

		private readonly SCNNode node;

		public GameCamera (SCNNode node)
		{
			this.node = node;
		}

		public void UpdateProperties ()
		{
			var gameObject = this.node.GetGameObject ();
			if (gameObject != null) {
				// use the props data, or else use the defaults in the struct above
				var hdr = gameObject.PropBool ("hdr");
				if (hdr) {
					properties.Hdr = hdr;
				}

				var motionBlur = gameObject.PropDouble ("motionBlur");
				if (motionBlur.HasValue) {
					properties.MotionBlur = motionBlur.Value;
				}

				var ambientOcclusion = gameObject.PropDouble ("ambientOcclusion");
				if (ambientOcclusion.HasValue) {
					properties.AmbientOcclusion = ambientOcclusion.Value;
				}
			}
		}

		public void TransferProperties ()
		{
			if (this.node.Camera != null) {
				// Wide-gamut rendering is enabled by default on supported devices;
				// to opt out, set the SCNDisableWideGamut key in your app's Info.plist file.
				this.node.Camera.WantsHdr = this.properties.Hdr;

				// Ambient occlusion doesn't work with defaults
				this.node.Camera.ScreenSpaceAmbientOcclusionIntensity = (nfloat) this.properties.AmbientOcclusion;

				// Motion blur is not supported when wide-gamut color rendering is enabled.
				this.node.Camera.MotionBlurIntensity = (nfloat) this.properties.MotionBlur;
			}
		}

		class GameCameraProps {
			public bool Hdr { get; set; }

			public double AmbientOcclusion { get; set; } = 0d;

			public double MotionBlur { get; set; } = 0d;
		}
	}
}
