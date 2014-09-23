using System;
using Foundation;

namespace SceneKitVehicle
{
	public static class ResourceManager
	{
		public static string ResourceFolder {
			get {
				return "Models.scnassets";
			}
		}

		public static NSString GetResourcePath (string fileName)
		{
			if (string.IsNullOrEmpty (fileName))
				throw new ArgumentException ("File name can't be null or empty string");

			return (NSString)String.Format ("{0}/{1}", ResourceFolder, fileName);
		}
	}
}

