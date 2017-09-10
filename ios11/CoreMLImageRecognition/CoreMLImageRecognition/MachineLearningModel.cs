using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using CoreML;
using CoreVideo;
using Foundation;
using UIKit;

namespace CoreMLImageRecognition
{
	struct ImageDescriptionPrediction
	{
		public string ModelName;
		public List<Tuple<double, string>> predictions;
	}

	class MachineLearningModel
	{
		public event EventHandler<EventArgsT<ImageDescriptionPrediction>> PredictionsUpdated = delegate { };
		public event EventHandler<EventArgsT<String>> ErrorOccurred = delegate { };
		public event EventHandler<EventArgsT<String>> MessageUpdated = delegate { };

		Dictionary<string, MLModel> models = new Dictionary<string, MLModel>();
		Dictionary<MLModel, CGSize> sizeFor = new Dictionary<MLModel, CGSize>();

		string currentModel;
		string currentAuthors;

		internal MachineLearningModel()
		{
			
			var modelData = new[] { 
				new Tuple<string,CGSize>("VGG16", new CGSize(224,224)), 
				new Tuple<string,CGSize>("SqueezeNet", new CGSize(227,227)) 
			};

			foreach(var m in modelData)
			{
				var modelName = m.Item1;
				var modelInputImageSize = m.Item2;
				models[modelName] = LoadModel(modelName);
                if (models[modelName] != null)
    				sizeFor[models[modelName]] = modelInputImageSize;
			}

			SwitchToModel("SqueezeNet");
		}

		public IEnumerable<string> ModelNames => models.Keys;

		public MLModel Model(string name) => models[name];

		public MLModel SwitchToModel(string modelName)
		{
			if (models[modelName] == null)
			{
                var errorSuffix = modelName == "VGG16"?"(it probably hasn't been download, built, and added to the project's Resources. See the README for instructions). Touch again to switch back to the SqueezeNet model.not sur":"";
                ErrorOccurred(this, new EventArgsT<string>($"ML model '{modelName}' is null " + errorSuffix));
			}
			else
			{
				MessageUpdated(this, new EventArgsT<string>($"Switched to {modelName}"));
				currentModel = modelName;
				currentAuthors = models[modelName].ModelDescription.Metadata.Author;
			}
			return models[modelName];
		}

		MLModel LoadModel(string modelName)
		{
			NSBundle bundle = NSBundle.MainBundle;
			var assetPath = bundle.GetUrlForResource(modelName, "mlmodelc");
            MLModel mdl = null;
            try
            {
				 mdl = MLModel.Create(assetPath, out var err);
                if (err != null)
                {
                    ErrorOccurred(this, new EventArgsT<string>(err.ToString()));
                }
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("*** VGG16 model probably hasn't been downloaded, built, and added to the project's Resources. Refer to the README for instructions. Error: " + ane.Message);
            }
            return mdl;
		}

		internal void Classify(UIImage source)
		{
			var model = models[currentModel];

			var pixelBuffer = source.Scale(sizeFor[model]).ToCVPixelBuffer();
			var imageValue = MLFeatureValue.Create(pixelBuffer);

			var inputs = new NSDictionary<NSString, NSObject>(new NSString("image"), imageValue);

			var inputFp = new MLDictionaryFeatureProvider(inputs, out var error);
			if(error != null)
			{
				ErrorOccurred(this, new EventArgsT<string>(error.ToString()));
				return;
			}
			var outFeatures = model.GetPrediction(inputFp, out var err2);
			if(err2 != null)
			{
				ErrorOccurred(this, new EventArgsT<string>(err2.ToString()));
				return;
			}

			var predictionsDictionary = outFeatures.GetFeatureValue("classLabelProbs").DictionaryValue;
			var byProbability = new List<Tuple<double, string>>();
			foreach(var key in predictionsDictionary.Keys)
			{
				var description = (string) (NSString) key;
				var prob = (double)predictionsDictionary[key];
				byProbability.Add(new Tuple<double, string>(prob, description));
			}
			//Sort descending
			byProbability.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1) * -1);

			var prediction = new ImageDescriptionPrediction();
			prediction.ModelName = currentModel;
			prediction.predictions = byProbability;

			PredictionsUpdated(this, new EventArgsT<ImageDescriptionPrediction>(prediction));
		}
	}

}
