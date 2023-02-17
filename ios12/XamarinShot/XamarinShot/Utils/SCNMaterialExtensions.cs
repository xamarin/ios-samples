
namespace XamarinShot.Utils {
	using Foundation;
	using SceneKit;

	public static class SCNMaterialExtensions {
		public static SCNMaterial Create (NSObject diffuse)
		{
			var material = SCNMaterial.Create ();
			material.Diffuse.Contents = diffuse;
			material.DoubleSided = true;
			material.LightingModelName = SCNLightingModel.PhysicallyBased;

			return material;
		}
	}
}
