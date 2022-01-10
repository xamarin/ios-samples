// ObjectDetector.cs
//
// This file was automatically generated and should not be edited.
//

using CoreML;
using CoreVideo;

namespace BreakfastFinder;

/// <summary>
/// Model Prediction Input Type
/// </summary>
public class ObjectDetectorInput : NSObject, IMLFeatureProvider
{
	static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
		new NSString ("image"), new NSString ("iouThreshold"), new NSString ("confidenceThreshold")
	);

	/// <summary>
	/// Input image as color (kCVPixelFormatType_32RGBA) image buffer, 416 pizels wide by 416 pixels high
	/// </summary>
	/// <value>Input image</value>
	public CVPixelBuffer Image { get; set; }

	/// <summary>
	/// (optional) IOU Threshold override (default: 0.45) as double
	/// </summary>
	/// <value>(optional) IOU Threshold override (default: 0.45)</value>
	public double IouThreshold { get; set; }

	/// <summary>
	/// (optional) Confidence Threshold override (default: 0.25) as double
	/// </summary>
	/// <value>(optional) Confidence Threshold override (default: 0.25)</value>
	public double ConfidenceThreshold { get; set; }

	public NSSet<NSString> FeatureNames => featureNames;

	public MLFeatureValue? GetFeatureValue (string featureName)
	{
		switch (featureName)
		{
			case "image":
				return MLFeatureValue.Create (Image);
			case "iouThreshold":
				return MLFeatureValue.Create (IouThreshold);
			case "confidenceThreshold":
				return MLFeatureValue.Create (ConfidenceThreshold);
			default:
				return null;
		}
	}

	public ObjectDetectorInput (CVPixelBuffer image, double iouThreshold, double confidenceThreshold)
	{
		Image = image ?? throw new ArgumentNullException (nameof (image));
		IouThreshold = iouThreshold;
		ConfidenceThreshold = confidenceThreshold;
	}
}

/// <summary>
/// Model Prediction Output Type
/// </summary>
public class ObjectDetectorOutput : NSObject, IMLFeatureProvider
{
	static readonly NSSet<NSString> featureNames = new(
		new NSString ("confidence"), new NSString ("coordinates")
	);

	/// <summary>
	/// Boxes × Class confidence (see user-defined metadata "classes") as  0-dimensional array of doubles
	/// </summary>
	/// <value>Boxes × Class confidence (see user-defined metadata "classes")</value>
	public MLMultiArray Confidence { get; set; }

	/// <summary>
	/// Boxes × [x, y, width, height] (relative to image size) as  0-dimensional array of doubles
	/// </summary>
	/// <value>Boxes × [x, y, width, height] (relative to image size)</value>
	public MLMultiArray Coordinates { get; set; }

	public NSSet<NSString> FeatureNames => featureNames;

	public MLFeatureValue? GetFeatureValue (string featureName)
	{
		switch (featureName)
		{
			case "confidence":
				return MLFeatureValue.Create (Confidence);
			case "coordinates":
				return MLFeatureValue.Create (Coordinates);
			default:
				return null;
		}
	}

	public ObjectDetectorOutput (MLMultiArray? confidence, MLMultiArray? coordinates)
	{
		if (confidence is null)
			throw new ArgumentNullException (nameof (confidence));

		if (coordinates is null)
			throw new ArgumentNullException (nameof (coordinates));

		Confidence = confidence;
		Coordinates = coordinates;
	}
}

/// <summary>
/// Class for model loading and prediction
/// </summary>
public class ObjectDetector : NSObject
{
	public readonly MLModel? model;

	public ObjectDetector ()
	{
		var url = NSBundle.MainBundle.GetUrlForResource ("ObjectDetector", "mlmodelc");
		NSError err;

		model = MLModel.Create (url, out err);
	}

	ObjectDetector (MLModel model)
	{
		this.model = model;
	}

	public static ObjectDetector? Create (NSUrl url, out NSError error)
	{
		if (url is null)
			throw new ArgumentNullException (nameof (url));

		var model = MLModel.Create (url, out error);

		if (model is null)
			return null;

		return new ObjectDetector (model);
	}

	/// <summary>
	/// Make a prediction using the standard interface
	/// </summary>
	/// <param name="input">an instance of ObjectDetectorInput to predict from</param>
	/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
	public ObjectDetectorOutput? GetPrediction (ObjectDetectorInput input, out NSError error)
	{
		if (model is null)
		{
			error = new NSError ();
			return null;
		}
		var prediction = model.GetPrediction (input, out error);

		if (prediction is null)
			return null;

		var confidenceValue = prediction.GetFeatureValue ("confidence").MultiArrayValue;
		var coordinatesValue = prediction.GetFeatureValue ("coordinates").MultiArrayValue;

		return new ObjectDetectorOutput (confidenceValue, coordinatesValue);
	}

	/// <summary>
	/// Make a prediction using the convenience interface
	/// </summary>
	/// <param name="image">Input image as color (kCVPixelFormatType_32RGBA) image buffer, 416 pizels wide by 416 pixels high</param>
	/// <param name="iouThreshold">(optional) IOU Threshold override (default: 0.45) as double</param>
	/// <param name="confidenceThreshold">(optional) Confidence Threshold override (default: 0.25) as double</param>
	/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
	public ObjectDetectorOutput? GetPrediction (CVPixelBuffer image, double iouThreshold, double confidenceThreshold, out NSError error)
	{
		var input = new ObjectDetectorInput (image, iouThreshold, confidenceThreshold);

		return GetPrediction (input, out error);
	}
}
