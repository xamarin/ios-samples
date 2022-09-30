namespace XamarinShot.Models;

public class GameCamera
{
        readonly GameCameraProps properties = new GameCameraProps ();

        readonly SCNNode node;

        public GameCamera (SCNNode node)
        {
                this.node = node;
        }

        public void UpdateProperties ()
        {
                var gameObject = node.GetGameObject ();
                if (gameObject is not null)
                {
                        // use the props data, or else use the defaults in the struct above
                        var hdr = gameObject.PropBool ("hdr");
                        if (hdr)
                        {
                                properties.Hdr = hdr;
                        }

                        var motionBlur = gameObject.PropDouble ("motionBlur");
                        if (motionBlur.HasValue)
                        {
                                properties.MotionBlur = motionBlur.Value;
                        }

                        var ambientOcclusion = gameObject.PropDouble ("ambientOcclusion");
                        if (ambientOcclusion.HasValue)
                        {
                                properties.AmbientOcclusion = ambientOcclusion.Value;
                        }
                }
        }

        public void TransferProperties ()
        {
                if (node.Camera is not null)
                {
                        // Wide-gamut rendering is enabled by default on supported devices;
                        // to opt out, set the SCNDisableWideGamut key in your app's Info.plist file.
                        node.Camera.WantsHdr = properties.Hdr;

                        // Ambient occlusion doesn't work with defaults
                        node.Camera.ScreenSpaceAmbientOcclusionIntensity = (nfloat)properties.AmbientOcclusion;

                        // Motion blur is not supported when wide-gamut color rendering is enabled.
                        node.Camera.MotionBlurIntensity = (nfloat)properties.MotionBlur;
                }
        }

        class GameCameraProps
        {
                public bool Hdr { get; set; }

                public double AmbientOcclusion { get; set; } = 0d;

                public double MotionBlur { get; set; } = 0d;
        }
}
