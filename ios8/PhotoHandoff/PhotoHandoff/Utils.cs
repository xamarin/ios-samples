using System;
using Foundation;
using System.Collections.Generic;

namespace PhotoHandoff
{
	public static class Utils
	{
		public static NSDictionary Convert<T>(this Dictionary<string, T> dict) where T : NSObject
		{
			if (dict == null)
				throw new ArgumentNullException ("dict");

			var nativeDict = new NSMutableDictionary ();

			foreach (var item in dict)
				nativeDict.Add ((NSString)item.Key, item.Value);

			return nativeDict;
		}

		public static Dictionary<string, T> Convert<T>(this NSDictionary nativeDict) where T : NSObject
		{
			var dict = new Dictionary<string, T> ();

			foreach (var item in nativeDict)
				dict.Add ((NSString)item.Key, (T)item.Value);

			return dict;
		}

		// https://trello.com/c/TydBAJP0
		public static bool TryDecodeObject(this NSCoder coder, string key, out NSObject obj)
		{
			obj = null;

			if (coder.ContainsKey (key)) {
				obj = coder.DecodeObject (key);
				return true;
			}

			return false;
		}
	}
}

