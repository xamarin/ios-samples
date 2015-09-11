using System;
using System.Linq;

using UIKit;
using SceneKit;

namespace CSSceneKit
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		int Building (float width, float length, float height, float posx, float posy, SCNScene scene, Random rnd) 
		{
			var boxNode = new SCNNode () {
				Geometry = new SCNBox {
					Width = width,
					Height = height,
					Length = length,
					ChamferRadius = 0.02f
				},
				Position = new SCNVector3(posx, height/2.0f, posy)
			};

			scene.RootNode.AddChildNode (boxNode);

			var buildings = new[] { "Content/building1.jpg", "Content/building2.jpg","Content/building3.jpg" };
			var material = new SCNMaterial ();
			material.Diffuse.Contents = UIImage.FromFile (buildings [rnd.Next (buildings.Length)]);
			material.Diffuse.ContentsTransform = SCNMatrix4.Scale (new SCNVector3 (width, height, 1.0f));
			material.Diffuse.WrapS = SCNWrapMode.Repeat;
			material.Diffuse.WrapT = SCNWrapMode.Repeat;
			material.Diffuse.MipFilter = SCNFilterMode.Linear;
			material.Diffuse.MagnificationFilter = SCNFilterMode.Linear;
			material.Specular.Contents = UIColor.Gray;
			material.LocksAmbientWithDiffuse = true;

			boxNode.Geometry.FirstMaterial = material;
			return 0;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			var scene = new SCNScene ();

			var rnd = new Random ();

			Func<int, int, bool, float> random = (min, max, clamp) => {
				float num = (float)((double)rnd.Next(min, max) * rnd.NextDouble ());
				if (!clamp)
					return num;
				else if (num < 1.0f)
					return 1.0f;
				else
					return num;
			};

			Enumerable.Range (0, 200).Select<int, int> ((i) => Building (
				random (2,5, true),
				random (2,5, true),
				random (2,10, true),
				random (-20, 20, false),
				random (-20, 20, false),
				scene,
				rnd
			)).ToArray ();

			//Lights!
			var lightNode = new SCNNode() {
				Light = new SCNLight (),
				Position = new SCNVector3 (30.0F, 20.0F, 60.0F)
			};
			lightNode.Light.LightType = SCNLightType.Omni;
			scene.RootNode.AddChildNode (lightNode);

			var ambientLightNode = new SCNNode () {
				Light = new SCNLight ()
			};

			ambientLightNode.Light.LightType = SCNLightType.Ambient;
			ambientLightNode.Light.Color = UIColor.DarkGray;
			scene.RootNode.AddChildNode (ambientLightNode);

			//Camera!
			var cameraNode = new SCNNode () { Camera = new SCNCamera () };
			scene.RootNode.AddChildNode (cameraNode);
			cameraNode.Position = new SCNVector3 (0.0F, 10.0F, 20.0F);

			var targetNode = new SCNNode () {
				Position = new SCNVector3 (00.0F, 1.5F, 0.0F)
			};
			scene.RootNode.AddChildNode (targetNode);

			var lc = SCNLookAtConstraint.Create (targetNode);
			cameraNode.Constraints = new[] { lc };

			var scnView = new SCNView(UIScreen.MainScreen.Bounds) {
				Scene = scene,
				AllowsCameraControl = true,
				ShowsStatistics = true,
				BackgroundColor = UIColor.FromRGB (52, 152, 219)
			};

			var floorNode = new SCNNode {
				Geometry = new SCNPlane {
					Height = 40.0F,
					Width = 40.0F
				},
				Position = SCNVector3.Zero
			};

			var pi2 = Math.PI / 2.0;
			floorNode.Orientation = SCNQuaternion.FromAxisAngle (SCNVector3.UnitX, (float)(0.0 - pi2));

			scene.RootNode.AddChildNode (floorNode);

			var material = new SCNMaterial ();
			material.Diffuse.Contents = UIImage.FromFile ("Content/road.jpg");
			material.Diffuse.ContentsTransform = SCNMatrix4.Scale (new SCNVector3 (10.0f, 10.0f, 1.0f));
			material.Diffuse.MinificationFilter = SCNFilterMode.Linear;
			material.Diffuse.MagnificationFilter = SCNFilterMode.Linear;
			material.Diffuse.MipFilter = SCNFilterMode.Linear;
			material.Diffuse.WrapS = SCNWrapMode.Repeat;
			material.Diffuse.WrapT = SCNWrapMode.Repeat;
			material.Specular.Contents = UIColor.Gray;

			floorNode.Geometry.FirstMaterial = material;

			this.View = scnView;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

