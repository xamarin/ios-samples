using System;
using UIKit;
using SceneKit;
using Foundation;
using OpenTK;
using System.Linq;

namespace ScanningAndDetecting3DObjects {
	internal static class Utilities {
		// Colors defined in Asset Catalog
		internal readonly static UIColor AppYellow = UIColor.FromName ("appYellow");
		internal readonly static UIColor AppBrown = UIColor.FromName ("appBrown");
		internal readonly static UIColor AppGreen = UIColor.FromName ("appGreen");
		internal readonly static UIColor AppBlue = UIColor.FromName ("appBlue");
		internal readonly static UIColor AppLightBlue = UIColor.FromName ("appLightBlue");
		internal readonly static UIColor AppGray = UIColor.FromName ("appGray");

		internal static SCNMaterial Material (NSObject diffuse, bool respondsToLighting = false, bool isDoubleSided = true)
		{
			var material = new SCNMaterial ();
			material.Diffuse.Contents = diffuse;
			material.DoubleSided = isDoubleSided;
			if (respondsToLighting) {
				material.LocksAmbientWithDiffuse = true;
			} else {
				material.LocksAmbientWithDiffuse = false;
				material.Ambient.ContentColor = UIColor.Black;
				material.LightingModelName = SCNLightingModel.Constant;
			}
			return material;
		}

		internal static SCNNode Load3DModel (NSUrl url)
		{
			var scene = SCNScene.FromUrl (url, new NSDictionary (), out var err);
			if (scene == null) {
				Console.WriteLine ($"Error: failed to load 3D model from file {url}.");
				if (err != null) {
					Console.WriteLine (err.ToString ());
				}
				return null;
			}

			var node = new SCNNode ();
			foreach (var child in scene.RootNode.ChildNodes) {
				node.AddChildNode (child);
			}

			// If there are no light sources in the model, add some
			var lightNodes = node.ChildNodes.Where (child => child.Light != null);
			if (lightNodes.Count () == 0) {
				var ambientLight = new SCNLight {
					LightType = SCNLightType.Ambient,
					Intensity = 100
				};
				var ambientLightNode = new SCNNode ();
				ambientLightNode.Light = ambientLight;
				node.AddChildNode (ambientLightNode);

				var directionLight = new SCNLight {
					LightType = SCNLightType.Directional,
					Intensity = 500
				};
				var directionalLightNode = new SCNNode ();
				directionalLightNode.Light = directionLight;
				node.AddChildNode (directionalLightNode);
			}
			return node;
		}

		internal static Vector4 SCNVector4Create (SCNVector3 float3, int last) => new Vector4 (float3.X, float3.Y, float3.Z, last);

		internal static NMatrix4 NMatrix4Create (Vector4 [] rows) => new NMatrix4 (rows [0], rows [1], rows [2], rows [3]);
	}
}
