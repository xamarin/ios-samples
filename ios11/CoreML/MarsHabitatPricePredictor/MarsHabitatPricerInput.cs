using System;
using CoreML;
using Foundation;

namespace MarsHabitatPricePredictor
{

	// test cases
	// 1, 1, 750 = 1430 | 1429.74114545183
	// 2.5, 4, 3000 = 16,767 | 16767.0421324377
	
    /// <summary>
    /// Xcode 9 can generate this class, but for this example it was coded from scratch
    /// </summary>
    /// <remarks>
    /// The FeatureNames are what the model expects
    /// </remarks>
    public class MarsHabitatPricerInput : NSObject, IMLFeatureProvider
	{
		public double SolarPanels { get; set; }
		public double Greenhouses { get; set; }
		public double Size { get; set; }

		public NSSet<NSString> FeatureNames => new NSSet<NSString>(new NSString("solarPanels"), new NSString("greenhouses"), new NSString("size"));

		public MLFeatureValue GetFeatureValue(string featureName)
		{
			switch (featureName)
			{
				case "solarPanels":
					return MLFeatureValue.FromDouble(SolarPanels);
				case "greenhouses":
					return MLFeatureValue.FromDouble(Greenhouses);
				case "size":
					return MLFeatureValue.FromDouble(Size);
				default:
					return MLFeatureValue.FromDouble(0);
			}
		}
	}
}
