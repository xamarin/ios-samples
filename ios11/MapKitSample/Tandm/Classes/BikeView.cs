using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace Tandm
{
	[Register("BikeView")]
	public class BikeView : MKMarkerAnnotationView
	{
		#region Static Variables
		public static UIColor UnicycleColor = UIColor.FromRGB(254, 122, 36);
		public static UIColor TricycleColor = UIColor.FromRGB(153, 180, 44);
		#endregion

		#region Override Methods
		public override IMKAnnotation Annotation
		{
			get
			{
				return base.Annotation;
			}
			set
			{
				base.Annotation = value;

				var bike = value as Bike;
				if (bike != null){
					ClusteringIdentifier = "bike";
					switch(bike.Type){
						case BikeType.Unicycle:
							MarkerTintColor = UnicycleColor;
							GlyphImage = UIImage.FromBundle("Unicycle");
							DisplayPriority = MKFeatureDisplayPriority.DefaultLow;
							break;
						case BikeType.Tricycle:
							MarkerTintColor = TricycleColor;
							GlyphImage = UIImage.FromBundle("Tricycle");
							DisplayPriority = MKFeatureDisplayPriority.DefaultHigh;
							break;
					}
				}
			}
		}
		#endregion

		#region Constructors
		public BikeView()
		{
		}

		public BikeView(NSCoder coder) : base(coder)
		{
		}

		public BikeView(IntPtr handle) : base(handle)
		{
		}
		#endregion


	}
}
