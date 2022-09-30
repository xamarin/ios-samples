namespace XamarinShot.Models;

public class GameLight
{
        readonly GameLightProperties properties = new GameLightProperties ();

        readonly SCNNode node;

        public GameLight (SCNNode node)
        {
                this.node = node;
        }

        public void UpdateProperties ()
        {
                var gameObject = node.GetGameObject ();
                if (gameObject is not null)
                {
                        // use the props data, or else use the defaults in the struct above
                        var shadowMapSize = gameObject.PropFloat2 ("shadowMapSize");
                        if (shadowMapSize.HasValue)
                        {
                                properties.ShadowMapSize = shadowMapSize.Value;
                        }

                        var angles = gameObject.PropFloat3 ("angles");
                        if (angles.HasValue)
                        {
                                var toRadians = (float)Math.PI / 180f;
                                properties.Angles = angles.Value * toRadians;
                        }

                        var shadowMode = gameObject.PropInt ("shadowMode");
                        if (shadowMode.HasValue)
                        {
                                properties.ShadowMode = shadowMode.Value;
                        }
                }
        }

        public void TransferProperties ()
        {
                // are euler's set at refeference (ShadowLight) or internal node (LightNode)
                var lightNode = node.FindChildNode ("LightNode", true);
                var light = lightNode?.Light;
                if (light is null)
                        return;

                // As shadow map size is reduced get a softer shadow but more acne
                // and bias results in shadow offset.  Mostly thin geometry like the boxes
                // and the shadow plane have acne.  Turn off z-writes on the shadow plane.

                switch (properties.ShadowMode)
                {
                        case 0:
                                // activate special filtering mode with 16 sample fixed pattern
                                // this slows down the rendering by 2x
                                light.ShadowRadius = 0f;
                                light.ShadowSampleCount = 16;
                                break;

                        case 1:
                                light.ShadowRadius = 3f; // 2.5
                                light.ShadowSampleCount = 8;
                                break;

                        case 2:
                                // as resolution decreases more acne, use bias and cutoff in shadowPlane shaderModifier
                                light.ShadowRadius = 1f;
                                light.ShadowSampleCount = 1;
                                break;
                        default:
                                break;
                }

                // when true, this reduces acne, but is causing shadow to separate
                // not seeing much acne, so turn it off for now
                light.ForcesBackFaceCasters = false;

                light.ShadowMapSize = new CGSize (properties.ShadowMapSize.X,
                        properties.ShadowMapSize.Y);

                // Can turn on cascades with auto-adjust disabled here, but not in editor.
                // Based on shadowDistance where next cascade starts.  These are the defaults.
                // light.shadowCascadeCount = 2
                // light.shadowCascadeSplittingFactor = 0.15

                // this is a square volume that is mapped to the shadowMapSize
                // may need to adjust this based on the angle of the light and table size
                // setting angles won't work until we isolate angles in the level file to a single node
                // lightNode.parent.angles = prop.angles
                light.OrthographicScale = 15f;
                light.ZNear = 1f;
                light.ZFar = 30f;
        }

        class GameLightProperties
        {
                public OpenTK.Vector2 ShadowMapSize { get; set; } = new OpenTK.Vector2 (2048f, 4096f);

                public SCNVector3 Angles { get; set; } = new SCNVector3 (-90f, 0f, 0f);

                public int ShadowMode { get; set; } = 0;
        }
}
