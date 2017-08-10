using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using MapKit;
using CoreGraphics;
using CoreLocation;

namespace Tandm
{
	[Register("ClusterView")]
	public class ClusterView : MKAnnotationView
	{
		#region Static Variables
		public static UIColor ClusterColor = UIColor.FromRGB(202, 150, 38);
		#endregion

		#region Computed Properties
		public override IMKAnnotation Annotation
		{
			get
			{
				return base.Annotation;
			}
			set
			{
				base.Annotation = value;

				// TODO: Workaround, the developer should be able to use
				// `value as MKClusterAnnotation` instead of the following 
				// extension method call:
				var cluster = MKAnnotationWrapperExtensions.UnwrapClusterAnnotation(value);
				if (cluster != null)
				{
					var renderer = new UIGraphicsImageRenderer(new CGSize(40, 40));
					var count = cluster.MemberAnnotations.Length;
					var unicycleCount = CountBikeType(cluster.MemberAnnotations, BikeType.Unicycle);

					Image = renderer.CreateImage((context) => {
						// Fill full circle with tricycle color
						BikeView.TricycleColor.SetFill();
						UIBezierPath.FromOval(new CGRect(0, 0, 40, 40)).Fill();

						// Fill pie with unicycle color
						BikeView.UnicycleColor.SetFill();
						var piePath = new UIBezierPath();
						piePath.AddArc(new CGPoint(20,20), 20, 0, (nfloat)(Math.PI * 2.0 * unicycleCount / count), true);
						piePath.AddLineTo(new CGPoint(20, 20));
						piePath.ClosePath();
						piePath.Fill();

						// Fill inner circle with white color
						UIColor.White.SetFill();
						UIBezierPath.FromOval(new CGRect(8, 8, 24, 24)).Fill();

						// Finally draw count text vertically and horizontally centered
						var attributes = new UIStringAttributes() {
							ForegroundColor = UIColor.Black,
							Font = UIFont.BoldSystemFontOfSize(20)
						};
						var text = new NSString($"{count}");
						var size = text.GetSizeUsingAttributes(attributes);
						var rect = new CGRect(20 - size.Width / 2, 20 - size.Height / 2, size.Width, size.Height);
						text.DrawString(rect, attributes);
					});
				}
			}
		}
		#endregion

		#region Constructors
		public ClusterView()
		{
		}

		public ClusterView(NSCoder coder) : base(coder)
		{
		}

		public ClusterView(IntPtr handle) : base(handle)
		{
		}

		public ClusterView(MKAnnotation annotation, string reuseIdentifier) : base(annotation, reuseIdentifier)
		{
			// Initialize
			DisplayPriority = MKFeatureDisplayPriority.DefaultHigh;
			CollisionMode = MKAnnotationViewCollisionMode.Circle;

			// Offset center point to animate better with marker annotations
			CenterOffset = new CoreGraphics.CGPoint(0, -10);
		}
		#endregion

		#region Private Methods
		private nuint CountBikeType(IMKAnnotation[] members, BikeType type) {
			nuint count = 0;

			foreach(Bike member in members){
				if (member.Type == type) ++count;
			}

			return count;
		}
		#endregion
	}
}
