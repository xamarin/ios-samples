using System;
using Foundation;

namespace PhotoHandoff
{
	public class UserInfo : DictionaryContainer
	{
		const string BlurKey = "activityImageBlurKey";
		const string SepiaKey = "activityImageSepiaKey";
		const string ImageKey = "imageIdentifier";

		public float BlurRadius {
			get {
				return GetFloatValue ((NSString)BlurKey).Value;
			}
			set {
				SetNumberValue ((NSString)BlurKey, value);
			}
		}

		public float Intensity {
			get {
				return GetFloatValue ((NSString)SepiaKey).Value;
			}
			set {
				SetNumberValue ((NSString)SepiaKey, value);
			}
		}

		public string ImageId {
			get {
				return base.GetStringValue (ImageKey);
			}
			set {
				SetStringValue ((NSString)ImageKey, value);
			}
		}

		public UserInfo()
		{

		}

		public UserInfo(NSDictionary dictionary)
		{
			var mDict = (NSMutableDictionary)Dictionary;
			foreach (var item in dictionary)
				mDict.Add (item.Key, item.Value);
		}
	}
}

