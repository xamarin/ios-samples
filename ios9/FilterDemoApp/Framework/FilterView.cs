using System;
using System.Collections.Generic;
using System.Linq;

using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FilterDemoFramework {
	public partial class FilterView : UIView {
		public static readonly float DefaultMinHertz = 12f;
		public static readonly float DefaultMaxHertz = 22050f;

		const int logBase = 2;
		const float leftMargin = 54f;
		const float rightMargin = 10f;
		const float bottomMargin = 40f;
		const int numDBLines = 4;
		const int defaultGain = 20;
		const int gridLineCount = 11;
		const float labelWidth = 40f;
		const int maxNumberOfResponseFrequencies = 1024;

		CGPoint editPoint = CGPoint.Empty;
		bool touchDown;
		CAShapeLayer curveLayer;
		List<double> frequencies;

		readonly List <CATextLayer> dbLabels = new List <CATextLayer> ();
		readonly List <CATextLayer> frequencyLabels = new List<CATextLayer> ();
		readonly List <CALayer> dbLines = new List <CALayer> ();
		readonly List <CALayer> freqLines = new List<CALayer> ();
		readonly List <CALayer> controls = new List<CALayer> ();
		readonly CALayer containerLayer = new CALayer ();
		readonly CALayer graphLayer = new CALayer ();

		public IFilterViewDelegate Delegate { get; set;}

		float resonance;
		public float Resonance {
			get {
				return resonance;
			}
			set {
				resonance = value;

				// Clamp the resonance to min/max values.
				if (resonance > defaultGain)
					resonance = defaultGain;
				else if (resonance < -defaultGain)
					resonance = -defaultGain;

				editPoint.Y = NMath.Floor (GetLocationForDbValue (Resonance));
			}
		}

		float frequency = DefaultMinHertz;
		public float Frequency {
			get {
				return frequency;
			}
			set {
				frequency = value;

				if (value > DefaultMaxHertz)
					frequency = DefaultMaxHertz;
				else if (value < DefaultMinHertz)
					frequency = DefaultMinHertz;

				editPoint.X = NMath.Floor (GetLocationForFrequencyValue (Frequency));
			}
		}

		public FilterView (IntPtr handle) : base (handle)
		{
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			var firstTouch = touches.Cast<UITouch> ().FirstOrDefault ();
			if (firstTouch == null)
				return;

			var pointOfTouch = firstTouch.LocationInView (this);
			pointOfTouch.Y += bottomMargin;

			if (graphLayer.Contains (pointOfTouch))
				editPoint = ProcessTouch (pointOfTouch);

			touchDown = false;
			UpdateFrequenciesAndResonance ();
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			var firstTouch = touches.Cast<UITouch> ().FirstOrDefault ();
			if (firstTouch == null)
				return;

			var pointOfTouch = firstTouch.LocationInView (this);
			pointOfTouch.Y += bottomMargin;

			if (graphLayer.Contains (pointOfTouch)) {
				touchDown = true;
				editPoint = pointOfTouch;

				UpdateFrequenciesAndResonance ();
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			var scale = UIScreen.MainScreen.Scale;

			containerLayer.Name = "container";
			containerLayer.AnchorPoint = CGPoint.Empty;
			containerLayer.Frame = new CGRect (CGPoint.Empty, Layer.Bounds.Size);
			containerLayer.Bounds = containerLayer.Frame;
			containerLayer.ContentsScale = scale;
			Layer.AddSublayer (containerLayer);

			graphLayer.Name = "graph background";
			graphLayer.BorderColor = UIColor.DarkGray.CGColor;
			graphLayer.BorderWidth = 1f;
			graphLayer.BackgroundColor = UIColor.FromWhiteAlpha (0.88f, 1f).CGColor;
			graphLayer.Bounds = new CGRect (0, 0, Layer.Frame.Width - leftMargin, Layer.Frame.Height - bottomMargin);
			graphLayer.Position = new CGPoint (leftMargin, 0);
			graphLayer.AnchorPoint = CGPoint.Empty;
			graphLayer.ContentsScale = scale;

			containerLayer.AddSublayer (graphLayer);

			Layer.ContentsScale = scale;

			CreateDbLabelsAndLines ();
			CreateFrequencyLabelsAndLines ();
			CreateControlPoint ();
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
			touchDown = false;
		}

		public double [] GetFrequencyData ()
		{
			if (frequencies != null)
				return frequencies.ToArray ();

			var width = graphLayer.Bounds.Width;
			var rightEdge = width + leftMargin;

			var pixelRatio = (int)Math.Ceiling (width / maxNumberOfResponseFrequencies);
			nfloat location = leftMargin;
			var locationsCount = maxNumberOfResponseFrequencies;

			if (pixelRatio <= 1) {
				pixelRatio = 1;
				locationsCount = (int)width;
			}

			frequencies = new List <double> ();

			for (int i = 0; i < locationsCount; i++) {
				if (location > rightEdge)
					frequencies.Add (DefaultMaxHertz);
				else {
					var frequencyTmp = GetFrequencyValue (location);

					if (frequencyTmp > DefaultMaxHertz)
						frequencyTmp = DefaultMaxHertz;

					location += (nfloat)pixelRatio;

					frequencies.Add (frequencyTmp);
				}
			}

			return frequencies.ToArray ();
		}

		public void SetMagnitudes (double [] magnitudeData)
		{
			if (curveLayer == null) {
				curveLayer = new CAShapeLayer ();
				curveLayer.FillColor = UIColor.FromRGBA (0.31f, 0.37f, 0.73f, 0.8f).CGColor;
				graphLayer.AddSublayer (curveLayer);
			}

			var bezierPath = new CGPath ();
			var width = graphLayer.Bounds.Width;
			bezierPath.MoveToPoint (leftMargin, graphLayer.Frame.Height + bottomMargin);

			float lastDbPos = 0f;
			float location = leftMargin;
			var frequencyCount = frequencies?.Count ?? 0;
			var pixelRatio = (int)(Math.Ceiling (width / frequencyCount));

			for (int i = 0; i < frequencyCount; i++) {
				var dbValue = 20 * Math.Log10 (magnitudeData [i]);
				float dbPos;

				if (dbValue < -defaultGain)
					dbPos = GetLocationForDbValue (-defaultGain);
				else if (dbValue > defaultGain)
					dbPos = GetLocationForDbValue (defaultGain);
				else
					dbPos = GetLocationForDbValue ((float)dbValue);

				if (Math.Abs (lastDbPos - dbPos) >= 0.1)
					bezierPath.AddLineToPoint (location, dbPos);

				lastDbPos = dbPos;
				location += pixelRatio;

				if (location > width + graphLayer.Frame.X) {
					location = (float)width + (float)graphLayer.Frame.X;
					break;
				}
			}

			bezierPath.AddLineToPoint (location, graphLayer.Frame.Y + graphLayer.Frame.Height + bottomMargin);
			bezierPath.CloseSubpath ();

			CATransaction.Begin ();
			CATransaction.DisableActions = true;
			curveLayer.Path = bezierPath;
			CATransaction.Commit ();

			UpdateControls (true);
		}

		public float GetLocationForDbValue (float value)
		{
			var step = graphLayer.Frame.Height / (defaultGain * 2);
			var location = (value + defaultGain) * step;
			return (float)(graphLayer.Frame.Height - location + bottomMargin);
		}

		float GetLocationForFrequencyValue (float value)
		{
			var pixelIncrement = graphLayer.Frame.Width / gridLineCount;
			var number = value / DefaultMinHertz;
			var location = GetLogValue (number, logBase) * pixelIncrement;
			return (float)Math.Floor (location + graphLayer.Frame.X) + 0.5f;
		}

		float GetDbValue (nfloat location)
		{
			var step = graphLayer.Frame.Height / (defaultGain * 2);
			return (float)(-(((location - bottomMargin) / step) - defaultGain));
		}

		void CreateDbLabelsAndLines ()
		{
			int value;
			var scale = Layer.ContentsScale;

			for (int index = -numDBLines; index <= numDBLines; index++) {
				value = index * (defaultGain / numDBLines);

				if (index >= -numDBLines && index <= numDBLines) {
					var labelLayer = new CATextLayer {
						String = string.Format ("{0} db", value),
						Name = index.ToString (),
						FontSize = 14,
						ContentsScale = scale,
						ForegroundColor = UIColor.FromWhiteAlpha (0.1f, 1f).CGColor,
						AlignmentMode = CATextLayer.AlignmentRight
					};

					labelLayer.SetFont (UIFont.SystemFontOfSize (14).Name);

					dbLabels.Add (labelLayer);
					containerLayer.AddSublayer (labelLayer);

					var lineLayer = new CALayer {
						BackgroundColor = index == 0 ?
							UIColor.FromWhiteAlpha (0.65f, 1f).CGColor : UIColor.FromWhiteAlpha (0.8f, 1f).CGColor
					};

					dbLines.Add (lineLayer);
					graphLayer.AddSublayer (lineLayer);
				}
			}
		}

		void CreateFrequencyLabelsAndLines ()
		{
			float value;
			bool firstK = true;
			var scale = Layer.ContentsScale;

			for (int index = 0; index <= gridLineCount; index++) {
				value = GetValue ((float)index);

				var labelLayer = new CATextLayer {
					ForegroundColor = UIColor.FromWhiteAlpha (0.1f, 1f).CGColor,
					FontSize = 14,
					AlignmentMode = CATextLayer.AlignmentCenter,
					ContentsScale = scale,
					AnchorPoint = CGPoint.Empty
				};
				labelLayer.SetFont (UIFont.SystemFontOfSize (14).Name);

				frequencyLabels.Add (labelLayer);

				if (index > 0 && index < gridLineCount) {
					var lineLayer = new CALayer {
						BackgroundColor = UIColor.FromWhiteAlpha (0.8f, 1f).CGColor
					};
					freqLines.Add (lineLayer);
					graphLayer.AddSublayer (lineLayer);
					var s = GetString (value);

					if (value >= 1000 && firstK) {
						s += "K";
						firstK = false;
					}

					labelLayer.String = s;
				} else if (index == 0)
					labelLayer.String = string.Format ("{0} Hz", GetString (value));
				else
					labelLayer.String = string.Format ("{0} K", GetString (DefaultMaxHertz));

				containerLayer.AddSublayer (labelLayer);
			}
		}

		void CreateControlPoint ()
		{
			var controlColor = touchDown ? TintColor.CGColor : UIColor.DarkGray.CGColor;
			var lineLayer = new CALayer {
				BackgroundColor = controlColor,
				Name = "x"
			};
			controls.Add (lineLayer);
			graphLayer.AddSublayer (lineLayer);

			lineLayer = new CALayer {
				BackgroundColor = controlColor,
				Name = "y"
			};
			controls.Add (lineLayer);
			graphLayer.AddSublayer (lineLayer);

			var circleLayer = new CALayer {
				BorderColor = controlColor,
				BorderWidth = 2f,
				CornerRadius = 3f,
				Name = "point"
			};
			controls.Add (circleLayer);
			graphLayer.AddSublayer (circleLayer);
		}

		void UpdateControls (bool refreshColor)
		{
			var color = touchDown ? TintColor.CGColor : UIColor.DarkGray.CGColor;

			CATransaction.Begin ();
			CATransaction.DisableActions = true;

			foreach (var layer in controls) {
				switch (layer.Name) {
				case "point":
					layer.Frame = new CGRect (editPoint.X - 3, editPoint.Y - 3, 7f, 7f).Integral ();
					layer.Position = editPoint;

					if (refreshColor)
						layer.BorderColor = color;
					break;
				case "x":
					layer.Frame = new CGRect (graphLayer.Frame.X, NMath.Floor (editPoint.Y + 0.5f), graphLayer.Frame.Width, 1f);

					if (refreshColor)
						layer.BackgroundColor = color;
					break;
				case "y":
					layer.Frame = new CGRect (NMath.Floor (editPoint.X) + 0.5f, bottomMargin, 1f, graphLayer.Frame.Height);

					if (refreshColor)
						layer.BackgroundColor = color;
					break;
				default:
					layer.Frame = CGRect.Empty;
					break;
				}
			}

			CATransaction.Commit ();
		}

		void UpdateDbLayers ()
		{
			for (int index = -numDBLines; index <= numDBLines; index++) {
				var location = Math.Floor (GetLocationForDbValue (index * defaultGain / numDBLines));

				if (index >= -numDBLines && index <= numDBLines) {
					dbLines [index + 4].Frame = new CGRect (graphLayer.Frame.X, location, graphLayer.Frame.Width, 1f);
					dbLabels [index + 4].Frame = new CGRect (0f, location - bottomMargin - 8, leftMargin - 7f, 16f);
				}
			}
		}

		void UpdateFrequencyLayers ()
		{
			for (int index = 0; index <= gridLineCount; index++) {
				var value = GetValue (index);
				var location = Math.Floor (GetLocationForFrequencyValue (value));

				if (index > 0 && index < gridLineCount) {
					freqLines [index - 1].Frame = new CGRect (location, bottomMargin, 1f, graphLayer.Frame.Height);

					frequencyLabels [index].Frame = new CGRect (location - labelWidth / 2f, graphLayer.Frame.Height, labelWidth, 16f);
				}

				frequencyLabels [index].Frame = new CGRect (location - labelWidth / 2f, graphLayer.Frame.Height + 6, labelWidth + rightMargin, 16f);
			}
		}

		[Export ("layoutSublayersOfLayer:")]
		public void LayoutSublayersOfLayer (CALayer layer)
		{
			if (layer == Layer) {
				CATransaction.Begin ();
				CATransaction.DisableActions = true;

				containerLayer.Bounds = layer.Bounds;

				graphLayer.Bounds = new CGRect (
					leftMargin, 
					bottomMargin, 
					layer.Bounds.Width - leftMargin - rightMargin, 
					layer.Bounds.Height - bottomMargin - 10);

				UpdateDbLayers ();

				UpdateFrequencyLayers ();

				editPoint = new CGPoint (GetLocationForFrequencyValue (Frequency), GetLocationForDbValue (Resonance));

				if (curveLayer != null) {
					curveLayer.Bounds = graphLayer.Bounds;

					curveLayer.Frame = new CGRect (
						graphLayer.Frame.X,
						graphLayer.Frame.Y + bottomMargin,
						graphLayer.Frame.Width,
						graphLayer.Frame.Height);
				}

				CATransaction.Commit ();
			}

			UpdateControls (false);

			frequencies = null;

			Delegate?.DataChanged (this);
		}

		void UpdateFrequenciesAndResonance ()
		{
			var del = Delegate;

			var lastFrequency = GetFrequencyValue (editPoint.X);
			if (Math.Abs (lastFrequency - Frequency) > float.Epsilon) {
				Frequency = lastFrequency;
				if (del != null)
					del.FrequencyChanged (this, Frequency);
			}

			var lastResonance = GetDbValue (editPoint.Y);
			if (Math.Abs (lastResonance - Resonance) > float.Epsilon) {
				Resonance = lastResonance;
				if(del != null)
					del.ResonanceChanged (this, Resonance);
			}
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			var pointOfTouch = ((UITouch)touches.FirstOrDefault ()).LocationInView (this);
			pointOfTouch.Y += bottomMargin;

			if (graphLayer.Contains (pointOfTouch)) {
				touchDown = true;
				editPoint = pointOfTouch;

				UpdateFrequenciesAndResonance ();
			}
		}

		CGPoint ProcessTouch (CGPoint touchPoint)
		{
			nfloat x = NMath.Max (0, touchPoint.X);
			x = NMath.Min (x, graphLayer.Frame.Width + leftMargin);

			nfloat y = NMath.Max (0, touchPoint.Y);
			y = NMath.Min (y, graphLayer.Frame.Height + bottomMargin);

			return new CGPoint (x, y);
		}

		float GetFrequencyValue (nfloat location)
		{
			var pixelIncrement = graphLayer.Frame.Width / gridLineCount;
			var index = (location - graphLayer.Frame.X) / pixelIncrement;
			return GetValue (index);
		}

		static float GetValue (nfloat gridIndex) 
		{
			return (float)(DefaultMinHertz * Math.Pow (logBase, gridIndex));
		}

		static float GetLogValue (float number, float baseNum)
		{
			return (float)(Math.Log (number) / Math.Log (baseNum));
		}

		static string GetString (float value)
		{
			var temp = value;

			if (value >= 1000)
				temp /= 1000;

			temp = (float)Math.Floor (temp * 100 / 100);

			bool tooSmall = Math.Abs ((float)Math.Floor (temp) - temp) < float.Epsilon;
			return tooSmall ? temp.ToString ("F1") : temp.ToString ("0.#");
		}
	}
}
