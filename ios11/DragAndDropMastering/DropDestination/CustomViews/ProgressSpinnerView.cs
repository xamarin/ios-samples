using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;

namespace DropDestination
{
	/**
	 A ProgressSpinnerView observes changes in the
	 `fractionCompleted` property of its given progress,
	 and updates its spinner UI accordingly.
	 */
	public class ProgressSpinnerView : UIView
	{
		#region Computed Properties
		public CAShapeLayer ArcLayer { get; set; } = new CAShapeLayer();
		public NSProgress Progress { get; set; }
		public UILabel ProgressLabel { get; set; } = new UILabel();
		#endregion

		#region Constructors
		public ProgressSpinnerView(CGRect frame, NSProgress progress) : base(frame)
		{
			// Initialize
			Progress = progress;
			Progress.AddObserver(this, "fractionCompleted", NSKeyValueObservingOptions.New, IntPtr.Zero);

			ArcLayer.Path = UIBezierPath.FromArc(new CGPoint(Bounds.GetMidX(), Bounds.GetMidY()),
			                                     (float)(0.2f * Math.Min(Bounds.Width, Bounds.Height)),
			                                     (float)(-(Math.PI / 2f)),
			                                     (float)(2f * Math.PI - (Math.PI / 2f)),
			                                    true).CGPath;

			ArcLayer.StrokeColor = TintColor.CGColor;
			ArcLayer.FillColor = UIColor.Clear.CGColor;
			ArcLayer.LineWidth = 5;
			ArcLayer.StrokeStart = 0;
			ArcLayer.StrokeEnd = 0;
			ArcLayer.Position = CGPoint.Empty;
			Layer.AddSublayer(ArcLayer);

			BackgroundColor = UIColor.Clear;
			Layer.BorderColor = UIColor.LightGray.CGColor;
			Layer.BorderWidth = 2;
			Layer.CornerRadius = 10;

			ProgressLabel.Text = "0%";
			ProgressLabel.TextAlignment = UITextAlignment.Center;
			ProgressLabel.Frame = Bounds;
			AddSubview(ProgressLabel);
		}
		#endregion

		public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			BeginInvokeOnMainThread (() => {
				ArcLayer.StrokeEnd = (float)Progress.FractionCompleted;
				ArcLayer.Opacity = (float)(1 - Progress.FractionCompleted);
				ProgressLabel.Text = $"{100 * Progress.FractionCompleted:##0}%";
				if (Progress.Finished || Progress.Cancelled)
					StopObservingValue(); 
			});
		}

		void StopObservingValue()
		{
			if (Progress != null) {
				Progress.RemoveObserver(this, "fractionCompleted");
				Progress = null;
			}
		}

		#region Destructors
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				StopObservingValue(); 
			base.Dispose(disposing);
		}
		#endregion
	}
}
