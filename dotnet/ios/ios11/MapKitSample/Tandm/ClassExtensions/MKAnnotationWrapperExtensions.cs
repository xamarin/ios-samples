using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using MapKit;
using CoreGraphics;
using CoreLocation;

namespace Tandm
{
	public static class MKAnnotationWrapperExtensions
	{
		public static MKClusterAnnotation UnwrapClusterAnnotation(IMKAnnotation annotation) {
			if (annotation == null) return null;
			return ObjCRuntime.Runtime.GetNSObject(annotation.Handle) as MKClusterAnnotation;
		}
	}
}
