// MarsHabitatPricer.cs
//
// This file was automatically generated and should not be edited.
//

using CoreML;

namespace MarsHabitatPricePredictor;

/// <summary>
/// Model Prediction Input Type
/// </summary>
public class MarsHabitatPricerInput : NSObject, IMLFeatureProvider
{
	public NSSet<NSString> FeatureNames { get; } = new (
		new NSString ("solarPanels"), new NSString ("greenhouses"), new NSString ("size")
	);

	/// <summary>
	/// Number of solar panels as double
	/// </summary>
	/// <value>Number of solar panels</value>
	public double SolarPanels { get; set; }

	/// <summary>
	/// Number of greenhouses as double
	/// </summary>
	/// <value>Number of greenhouses</value>
	public double Greenhouses { get; set; }

	/// <summary>
	/// Size in acres as double
	/// </summary>
	/// <value>Size in acres</value>
	public double Size { get; set; }

	public MLFeatureValue? GetFeatureValue (string featureName) => featureName switch
	{
		"solarPanels" => MLFeatureValue.Create (SolarPanels),
		"greenhouses" => MLFeatureValue.Create (Greenhouses),
		"size" => MLFeatureValue.Create (Size),
		_ => null,
	};

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
	public NSSet<NSString> FeatureNames { get; } = new (
		new NSString ("price")
	);

	/// <summary>
	/// Price of the habitat (in millions) as double
	/// </summary>
	/// <value>Price of the habitat (in millions)</value>
	public double Price { get; set; }

	public MLFeatureValue? GetFeatureValue (string featureName) => featureName switch
	{
		"price" => MLFeatureValue.Create (Price),
		_ => null,
	};

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

	static NSUrl GetModelUrl ()
	{
		return NSBundle.MainBundle.GetUrlForResource ("MarsHabitatPricer", "mlmodelc");
	}

	public MarsHabitatPricer ()
	{
		model = MLModel.Create (GetModelUrl (), out _);
	}

	MarsHabitatPricer (MLModel model)
	{
		this.model = model;
	}

	public static MarsHabitatPricer? Create (NSUrl url, out NSError error)
	{
		var model = MLModel.Create (url, out error);
		return model is not null ? new MarsHabitatPricer (model) : null;
	}

	public static MarsHabitatPricer? Create (MLModelConfiguration configuration, out NSError error)
	{
		var model = MLModel.Create (GetModelUrl (), configuration, out error);
		return model is not null ? new MarsHabitatPricer (model) : null;
	}

	public static MarsHabitatPricer? Create (NSUrl url, MLModelConfiguration configuration, out NSError error)
	{
		var model = MLModel.Create (url, configuration, out error);
		return model is not null ?  new MarsHabitatPricer (model) : null;
	}

	/// <summary>
	/// Make a prediction using the standard interface
	/// </summary>
	/// <param name="input">an instance of MarsHabitatPricerInput to predict from</param>
	/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
	public MarsHabitatPricerOutput? GetPrediction (MarsHabitatPricerInput input, out NSError? error)
	{
		error = null;

		var prediction = model?.GetPrediction (input, out error);

		if (prediction is null)
			return null;

		var priceValue = prediction.GetFeatureValue ("price")?.DoubleValue;

		return priceValue is not null ? new MarsHabitatPricerOutput (priceValue.Value) : null;
	}

	/// <summary>
	/// Make a prediction using the standard interface
	/// </summary>
	/// <param name="input">an instance of MarsHabitatPricerInput to predict from</param>
	/// <param name="options">prediction options</param>
	/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
	public MarsHabitatPricerOutput? GetPrediction (MarsHabitatPricerInput input, MLPredictionOptions options, out NSError? error)
	{
		error = null;

		var prediction = model?.GetPrediction (input, options, out error);

		if (prediction is null)
			return null;

		var priceValue = prediction.GetFeatureValue ("price")?.DoubleValue;

		return priceValue is not null ? new MarsHabitatPricerOutput (priceValue.Value) : null;
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

	/// <summary>
	/// Make a prediction using the convenience interface
	/// </summary>
	/// <param name="solarPanels">Number of solar panels as double</param>
	/// <param name="greenhouses">Number of greenhouses as double</param>
	/// <param name="size">Size in acres as double</param>
	/// <param name="options">prediction options</param>
	/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
	public MarsHabitatPricerOutput? GetPrediction (double solarPanels, double greenhouses, double size, MLPredictionOptions options, out NSError? error)
	{
		var input = new MarsHabitatPricerInput (solarPanels, greenhouses, size);

		return GetPrediction (input, options, out error);
	}
}
