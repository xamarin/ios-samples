
namespace XamarinShot.Utils {
	using Foundation;
	using SceneKit;
	using UIKit;

	public static class SCNShadableExtensions {
		// https://developer.apple.com/documentation/scenekit/scnshadable#1654834
		// Some of these can be animated inside of an SCNTransaction.
		// Sets shader modifier data onto a material or all materials in a geometry.
		public static void SetTexture (this SCNGeometry geometry, string uniform, SCNMaterialProperty texture)
		{
			// this must be the texture name, and not the sampler name
			geometry.SetValueForKey (texture, new NSString (uniform));
		}

		public static void SetFloat4 (this SCNGeometry geometry, string uniform, SCNVector4 value)
		{
			geometry.SetValueForKey (NSValue.FromVector (value), new NSString (uniform));
		}

		public static void SetColor (this SCNGeometry geometry, string uniform, UIColor value)
		{
			geometry.SetValueForKey (value, new NSString (uniform));
		}

		#region getters

		public static bool HasUniform (this SCNGeometry geometry, string uniform)
		{
			return geometry.ValueForKey (new NSString (uniform)) != null;
		}

		#endregion
	}
}
