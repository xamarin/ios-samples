using CoreFoundation;
using SceneKit;
using UIKit;

namespace PlacingObjects
{
	public static class SCNScene_Extensions
	{
		public static void EnableEnvironmentMapWithIntensity(this SCNScene self, float intensity, DispatchQueue queue)
		{
			if (self.LightingEnvironment.Contents == null)
			{
				var environmentMap = UIImage.FromFile("Models.scnassets/sharedImages/environment_blur.exr");
				self.LightingEnvironment.ContentImage = environmentMap;
			}
			self.LightingEnvironment.Intensity = intensity;
		}
	}
}
