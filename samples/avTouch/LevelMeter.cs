
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using System.Collections.Generic;

namespace avTouch
{
	public struct LevelMeterColorThreshold {
		public float MaxValue;
		public UIColor Color;
		
		public LevelMeterColorThreshold (float mv, UIColor color)
		{
			MaxValue = mv;
			Color = color;
		}
	}
	
	public class LevelMeter : UIView {
		public bool Vertical { get; set; }
		public UIColor BgColor { get; set; }
		public UIColor BorderColor { get; set; }
		public bool VariableLightIntensity { get; set; }
		public float Level { get; set; }
		public float PeakLevel { get; set; }
		public int NumLights { get; set; }
		public LevelMeterColorThreshold [] ColorThresholds { get; set; }
		
		void MeterInit ()
		{
			Level = 0;
			NumLights = 0;
			BgColor = UIColor.FromRGBA (0, 0, 0, 0.6f);
			BorderColor = UIColor.FromRGBA (0, 0, 0, 1);
			ColorThresholds = new LevelMeterColorThreshold [] {
				new LevelMeterColorThreshold (0.25f, UIColor.FromRGBA (0, 1, 0, 1)),
				new LevelMeterColorThreshold (0.8f, UIColor.FromRGBA (1, 1, 0, 1)),
				new LevelMeterColorThreshold (1, UIColor.FromRGBA (1, 0, 0, 1)),
			};
			Vertical = Frame.Width < Frame.Height;
		}
		
		public LevelMeter (RectangleF frame) : base (frame)
		{
			MeterInit ();
		}
		
		public LevelMeter (NSCoder coder) : base (coder)
		{
			MeterInit ();
		}
		
		float Clamp (float min, float val, float max)
		{
			if (val < min)
				return min;
			if (val > max)
				return max;
			return val;
		}
		
		public override void Draw (RectangleF rectB)
		{
			CGColorSpace cs = null;
			CGContext ctx = null;
			RectangleF bds;
			
			ctx = UIGraphics.GetCurrentContext ();
			cs = CGColorSpace.CreateDeviceRGB ();
			
			if (Vertical){
				ctx.TranslateCTM (0, Bounds.Height);
				ctx.ScaleCTM (1, -1);
				bds = Bounds;
			} else {
				ctx.TranslateCTM (0, Bounds.Height);
				ctx.RotateCTM ((float)Math.PI/2);
				bds = new RectangleF (0, 0, Bounds.Height, Bounds.Width);
			}
			
			ctx.SetFillColorSpace (cs);
			ctx.SetStrokeColorSpace (cs);
			
			if (NumLights == 0){
				float currentTop = 0;
				
				if (BgColor != null){
					BgColor.SetColor ();
					ctx.FillRect (bds);
				}
				
				foreach (var thisTresh in ColorThresholds){
					var val = Math.Min (thisTresh.MaxValue, Level);
					
					var rect = new RectangleF (0, bds.Height * currentTop, bds.Width, bds.Height * (val - currentTop));
					thisTresh.Color.SetColor ();
					ctx.FillRect (rect);
					
					if (Level < thisTresh.MaxValue)
						break;
					currentTop = val;
				}
				
				if (BorderColor != null){
					BorderColor.SetColor ();
					bds.Inflate (-0.5f, -0.5f);
					ctx.StrokeRect (bds);
				}
			} else {
				float lightMinVal = 0;
				float insetAmount, lightVSpace;
				int peakLight = -1;
				
				lightVSpace = bds.Height / (float) NumLights;
				if (lightVSpace < 4)
					insetAmount = 0;
				else if (lightVSpace < 8)
					insetAmount = 0.5f;
				else
					insetAmount = 1;
				
				if (PeakLevel > 0){
					peakLight = (int) PeakLevel * NumLights;
					if (peakLight >= NumLights)
						peakLight = NumLights-1;
				}
				
				for (int light_i = 0; light_i< NumLights; light_i++){
					float lightMaxVal = (light_i + 1) / (float)NumLights;
					float lightIntensity;
					RectangleF lightRect;
					UIColor lightColor;
					
					if (light_i == peakLight){
						lightIntensity = 1;
					} else {
						lightIntensity = (Level - lightMinVal) / (lightMaxVal - lightMinVal);
						lightIntensity = Clamp (0, lightIntensity, 1);
						if (!VariableLightIntensity && lightIntensity > 0)
							lightIntensity = 1;
					}
					
					lightColor = ColorThresholds [0].Color;
					int color_i = 0;
					for (; color_i < ColorThresholds.Length-1; color_i++){
						var thisTresh = ColorThresholds [color_i];
						var nextTresh = ColorThresholds [color_i+1];
						if (thisTresh.MaxValue < lightMaxVal)
							lightColor = nextTresh.Color;
					}
					
					lightRect = new RectangleF (0, bds.Height * color_i / (float) NumLights,
					                            bds.Width, bds.Height * (1f / NumLights));
					lightRect.Inflate (-insetAmount, -insetAmount);
					if (BgColor != null){
						BgColor.SetColor ();
						ctx.FillRect (lightRect);
					}
					if (lightIntensity == 1){
						lightColor.SetColor ();
						ctx.FillRect (lightRect);
					} else if (lightIntensity > 0){
						using (var clr = new CGColor (lightColor.CGColor, lightIntensity)){
							ctx.SetFillColorWithColor (clr);
							ctx.FillRect (lightRect);
						}						
					}
					
					if (BorderColor != null){
						BorderColor.SetColor ();
						lightRect.Inflate (-0.5f, -0.5f);
						ctx.StrokeRect (lightRect);
					}
					
					lightMinVal = lightMaxVal;
				}
			}
			cs.Dispose ();
		}
	}
}
