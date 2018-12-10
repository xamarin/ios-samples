using System;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace AVTouch
{
    /// <summary>
    /// The LevelMeterColorThreshold struct is used to define the colors for the LevelMeter, and at what values each of those colors begins.
    /// </summary>
    public struct LevelMeterColorThreshold
    {
        public LevelMeterColorThreshold(float value, UIColor color)
        {
            MaxValue = value;
            Color = color;
        }

        /// <summary>
        /// A value from 0 - 1. The maximum value shown in this color
        /// </summary>
        public float MaxValue { get; private set; }

        /// <summary>
        /// A UIColor to be used for this value range
        /// </summary>
        public UIColor Color { get; private set; }
    }

    public class LevelMeter : UIView
    {
        public LevelMeter(CGRect frame) : base(frame)
        {
            PerformInit();
        }

        public LevelMeter(NSCoder coder) : base(coder)
        {
            PerformInit();
        }

        /// <summary>
        /// Whether the view is oriented V or H. This is initially automatically set based on the aspect ratio of the view.
        /// </summary>
        public bool Vertical { get; set; }

        /// <summary>
        /// The background color of the lights
        /// </summary>
        public UIColor BgColor { get; set; }

        /// <summary>
        /// The border color of the lights
        /// </summary>
        public UIColor BorderColor { get; set; }

        /// <summary>
        /// Whether to use variable intensity lights. Has no effect if numLights == 0.
        /// </summary>
        public bool VariableLightIntensity { get; set; }

        /// <summary>
        /// The current level, from 0 - 1
        /// </summary>
        public float Level { get; set; }

        /// <summary>
        /// Optional peak level, will be drawn if > 0
        /// </summary>
        public float PeakLevel { get; set; }

        /// <summary>
        /// The number of lights to show, or 0 to show a continuous bar
        /// </summary>
        public int NumLights { get; set; }

        public LevelMeterColorThreshold[] ColorThresholds { get; set; }

        private void PerformInit()
        {
            Level = 0;
            NumLights = 0;
            Vertical = Frame.Width < Frame.Height;
            BgColor = UIColor.FromRGBA(0, 0, 0, 0.6f);
            BorderColor = UIColor.FromRGBA(0f, 0f, 0, 1f);
            ColorThresholds = new LevelMeterColorThreshold[]
            {
                new LevelMeterColorThreshold (0.25f, UIColor.FromRGBA (0, 1f, 0, 1f)),
                new LevelMeterColorThreshold (0.8f, UIColor.FromRGBA (1f, 1f, 0, 1f)),
                new LevelMeterColorThreshold (1, UIColor.FromRGBA (1f, 0, 0, 1f)),
            };
        }

        public override void Draw(CGRect rect)
        {
            CGColorSpace cs = null;
            CGContext ctx = null;
            CGRect bds;

            using(ctx = UIGraphics.GetCurrentContext())
            {
                using(cs = CGColorSpace.CreateDeviceRGB())
                {
                    if (Vertical)
                    {
                        ctx.TranslateCTM(0, Bounds.Height);
                        ctx.ScaleCTM(1, -1);
                        bds = Bounds;
                    }
                    else
                    {
                        ctx.TranslateCTM(0, Bounds.Height);
                        ctx.RotateCTM(-(float)Math.PI / 2);
                        bds = new CGRect(0, 0, Bounds.Height, Bounds.Width);
                    }

                    ctx.SetFillColorSpace(cs);
                    ctx.SetStrokeColorSpace(cs);

                    if (NumLights == 0)
                    {
                        float currentTop = 0;

                        if (BgColor != null)
                        {
                            BgColor.SetColor();
                            ctx.FillRect(bds);
                        }

                        foreach (var thisTresh in ColorThresholds)
                        {
                            var value = Math.Min(thisTresh.MaxValue, Level);

                            var fillRect = new CGRect(0, bds.Height * currentTop, bds.Width, bds.Height * (value - currentTop));
                            thisTresh.Color.SetColor();
                            ctx.FillRect(fillRect);

                            if (Level < thisTresh.MaxValue)
                                break;
                            currentTop = value;
                        }

                        if (BorderColor != null)
                        {
                            BorderColor.SetColor();
                            bds.Inflate(-0.5f, -0.5f);
                            ctx.StrokeRect(bds);
                        }
                    }
                    else
                    {
                        float lightMinVal = 0;
                        float insetAmount, lightVSpace;
                        int peakLight = -1;

                        lightVSpace = (float)bds.Height / NumLights;
                        if (lightVSpace < 4)
                        {
                            insetAmount = 0;
                        }
                        else if (lightVSpace < 8)
                        {
                            insetAmount = 0.5f;
                        }
                        else
                        {
                            insetAmount = 1;
                        }

                        if (PeakLevel > 0)
                        {
                            peakLight = (int)(PeakLevel * NumLights);
                            if (peakLight >= NumLights)
                            {
                                peakLight = NumLights - 1;
                            }
                        }

                        for (int light_i = 0; light_i < NumLights; light_i++)
                        {
                            float lightMaxVal = (light_i + 1) / (float)NumLights;
                            float lightIntensity;
                            CGRect lightRect;
                            UIColor lightColor;

                            if (light_i == peakLight)
                            {
                                lightIntensity = 1;
                            }
                            else
                            {
                                lightIntensity = (Level - lightMinVal) / (lightMaxVal - lightMinVal);
                                lightIntensity = Clamp(0, lightIntensity, 1);
                                if (!VariableLightIntensity && lightIntensity > 0)
                                {
                                    lightIntensity = 1;
                                }
                            }

                            lightColor = ColorThresholds[0].Color;
                            int color_i = 0;
                            for (; color_i < ColorThresholds.Length - 1; color_i++)
                            {
                                var thisTresh = ColorThresholds[color_i];
                                var nextTresh = ColorThresholds[color_i + 1];
                                if (thisTresh.MaxValue <= lightMaxVal)
                                {
                                    lightColor = nextTresh.Color;
                                }
                            }

                            lightRect = new CGRect(0, bds.Height * light_i / (float)NumLights, bds.Width, bds.Height * (1f / NumLights));
                            lightRect.Inset(insetAmount, insetAmount);

                            if (BgColor != null)
                            {
                                BgColor.SetColor();
                                ctx.FillRect(lightRect);
                            }

                            if (lightIntensity == 1f)
                            {
                                lightColor.SetColor();
                                //Console.WriteLine ("Setting color to {0}", lightColor);
                                ctx.FillRect(lightRect);
                            }
                            else if (lightIntensity > 0)
                            {
                                using (var clr = new CGColor(lightColor.CGColor, lightIntensity))
                                {
                                    ctx.SetFillColor(clr);
                                    ctx.FillRect(lightRect);
                                }
                            }

                            if (BorderColor != null)
                            {
                                BorderColor.SetColor();
                                lightRect.Inset(0.5f, 0.5f);
                                ctx.StrokeRect(lightRect);
                            }

                            lightMinVal = lightMaxVal;
                        }
                    }
                }
            }
        }

        private static float Clamp(float min, float val, float max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }
    }
}