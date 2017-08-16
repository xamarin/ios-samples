using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using CoreImage;

namespace CoreMLVision
{
	public static class DictionaryExtensions
	{
		public static NSDictionary<NSString, CIVector> ToNSDictionary(this Dictionary<string, CGPoint> self){

			var keys = new List<NSString>();
			var values = new List<CIVector>();

			// Process all keys in the dictionary
			foreach(string key in self.Keys) {
				keys.Add(new NSString(key));
				values.Add(new CIVector(self[key]));
			}

			// Return results
			return new NSDictionary<NSString, CIVector>(keys.ToArray(), values.ToArray());
		}

		public static NSDictionary<NSString, NSNumber> ToNSDictionary(this Dictionary<NSString, NSNumber> self)
		{

			var keys = new List<NSString>();
			var values = new List<NSNumber>();

			// Process all keys in the dictionary
			foreach (NSString key in self.Keys)
			{
				keys.Add(key);
				values.Add(self[key]);
			}

			// Return results
			return new NSDictionary<NSString, NSNumber>(keys.ToArray(), values.ToArray());
		}
	}
}
