using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;
using System.Linq;

namespace PlacingObjects
{
	public static class ListExtensions
	{
		public static void Union<T>(this List<T> self, List<T> list)
		{

			// Add list items to self if not already contained
			foreach (T item in list)
			{
				if (!self.Contains(item)) self.Add(item);
			}
		}

		public static void Subtract<T>(this List<T> self, List<T> list) {

			// Remove the items from the list from self
			foreach(T item in list) {
				self.Remove(item);
			}
		}

		public static void KeepLast<T>(this List<T> self, int count) {

			// Remove first item until the count matches the requested number
			while(self.Count > count) {
				self.RemoveAt(0);
			}
		}

		public static float Average<T>(this List<T> self) {
			if (self.Count() == 0)
			{
				return 0.0f;
			}
			var sum = 0f;

			foreach(T value in self) {
                float v = default(float);
				float.TryParse(value.ToString(), out v);
				sum += v;
			}

			return (sum / (float)self.Count);
		}
	}
}
