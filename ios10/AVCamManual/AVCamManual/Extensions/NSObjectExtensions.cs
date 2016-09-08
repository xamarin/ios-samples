using CoreMedia;
using Foundation;

namespace AVCamManual
{
	public static class NSObjectExtensions
	{
		public static int AsInt(this NSObject nsObj)
		{
			var num = (NSNumber)nsObj;
			return num.Int32Value;
		}

		public static float AsFloat(this NSObject nsObj)
		{
			var num = (NSNumber)nsObj;
			return num.FloatValue;
		}

		public static bool AsBool(this NSObject nsObj)
		{
			return ((NSNumber)nsObj).BoolValue;
		}

		public static CMTime AsCMTime (this NSObject nsObj)
		{
			return ((NSValue)nsObj).CMTimeValue;
		}
	}
}
