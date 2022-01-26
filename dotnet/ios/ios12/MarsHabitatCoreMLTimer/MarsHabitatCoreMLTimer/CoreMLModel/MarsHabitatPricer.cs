// MarsHabitatPricer.cs
//
// This file was automatically generated and should not be edited.
//
using CoreML;

namespace MarsHabitatCoreMLTimer;
/// <summary>
/// Model Prediction Input Type
/// </summary>
public class MarsHabitatPricerInput : NSObject, IMLFeatureProvider
{
	static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
		new NSString ("solarPanels"), new NSString ("greenhouses"), new NSString ("size")
	);

	double solarPanels;
	double greenhouses;
	double size;

	/// <summary>
	/// Number of solar panels as double
	/// </summary>
	/// <value>Number of solar panels</value>
	public double SolarPanels
	{
		get { return solarPanels; }
		set
		{
			solarPanels = value;
		}
	}

	/// <summary>
	/// Number of greenhouses as double
	/// </summary>
	/// <value>Number of greenhouses</value>
	public double Greenhouses
	{
		get { return greenhouses; }
		set
		{
			greenhouses = value;
		}
	}

	/// <summary>
	/// Size in acres as double
	/// </summary>
	/// <value>Size in acres</value>
	public double Size
	{
		get { return size; }
		set
		{
			size = value;
		}
	}

	public NSSet<NSString> FeatureNames
	{
		get { return featureNames; }
	}

	public MLFeatureValue? GetFeatureValue (string featureName)
	{
		switch (featureName)
		{
			case "solarPanels":
				return MLFeatureValue.Create (SolarPanels);
			case "greenhouses":
				return MLFeatureValue.Create (Greenhouses);
			case "size":
				return MLFeatureValue.Create (Size);
			default:
				return null;
		}
	}

	public MarsHabitatPricerInput (double solarPanels, double greenhouses, double size)
	{
		SolarPanels = solarPanels;
		Greenhouses = greenhouses;
		Size = size;
	}
}

/// <summary>
/// Model Prediction Output Type
/// </summary>
public class MarsHabitatPricerOutput : NSObject, IMLFeatureProvider
{
	static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
		new NSString ("price")
	);

	double price;

	/// <summary>
	/// Price of the habitat (in millions) as double
	/// </summary>
	/// <value>Price of the habitat (in millions)</value>
	public double Price
	{
		get { return price; }
		set
		{
			price = value;
		}
	}

	public NSSet<NSString> FeatureNames
	{
		get { return featureNames; }
	}

	public MLFeatureValue? GetFeatureValue (string featureName)
	{
		switch (featureName)
		{
			case "price":
				return MLFeatureValue.Create (Price);
			default:
				return null;
		}
	}

	public MarsHabitatPricerOutput (double price)
	{
		Price = price;
	}
}

/// <summary>
/// Class for model loading and prediction
/// </summary>
public class MarsHabitatPricer : NSObject
{
	readonly MLModel? model;

	public MarsHabitatPricer ()
	{
		var url = NSBundle.MainBundle.GetUrlForResource ("MarsHabitatPricer", "mlmodelc");
		NSError err;

		model = MLModel.Create (url, out err);
	}

	MarsHabitatPricer (MLModel model)
	{
		this.model = model;
	}

	public static MarsHabitatPricer? Create (NSUrl url, out NSError error)
	{
		if (url is null)
			throw new ArgumentNullException (nameof (url));

		var model = MLModel.Create (url, out error);

		if (model is null)
			return null;

		return new MarsHabitatPricer (model);
	}

	/// <summary>
	/// Make a prediction using the standard interface
	/// </summary>
	/// <param name="input">an instance of MarsHabitatPricerInput to predict from</param>
	/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
	public MarsHabitatPricerOutput? GetPrediction (MarsHabitatPricerInput input, out NSError? error)
	{
		if (model is null)
		{
			error = null;
			return null;
		}
		var prediction = model.GetPrediction (input, out error);

		if (prediction is null)
			return null;

		var priceValue = prediction.GetFeatureValue ("price")?.DoubleValue ?? 0.0;

		return new MarsHabitatPricerOutput (priceValue);
	}

	/// <summary>
	/// Make a prediction using the convenience interface
	/// </summary>
	/// <param name="solarPanels">Number of solar panels as double</param>
	/// <param name="greenhouses">Number of greenhouses as double</param>
	/// <param name="size">Size in acres as double</param>
	/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
	public MarsHabitatPricerOutput? GetPrediction (double solarPanels, double greenhouses, double size, out NSError? error)
	{
		var input = new MarsHabitatPricerInput (solarPanels, greenhouses, size);

		return GetPrediction (input, out error);
	}
}

