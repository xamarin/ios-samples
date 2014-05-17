// 
// WeatherAnnotationView.cs
//  
// Author: Jeffrey Stedfast <jeff@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using CoreGraphics;
using System.Reflection;
using UIKit;
using MapKit;
using Foundation;


namespace WeatherMap
{
	public class WeatherAnnotationView : MKAnnotationView
	{
		public WeatherAnnotationView (NSObject annotation, string reuseIdentifier) : base (annotation, reuseIdentifier)
		{
			CGRect frame = Frame;
			frame.Size = new CGSize (60.0f, 85.0f);
			Frame = frame;
			
			BackgroundColor = UIColor.Clear;
			CenterOffset = new CGPoint (30.0f, 42.0f);
		}

		public override NSObject Annotation {
			get {
				return base.Annotation;
			}
			
			set {
				base.Annotation = value;
				
				// this annotation view has custom drawing code.  So when we reuse an annotation view
				// (through MapView's delegate "dequeueReusableAnnoationViewWithIdentifier" which returns non-nil)
				// we need to have it redraw the new annotation data.
				//
				// for any other custom annotation view which has just contains a simple image, this won't be needed
				//
				SetNeedsDisplay ();
			}
		}

		public override void Draw (CGRect rect)
		{
			WeatherForecastAnnotation annotation;
			CGPath path;
			
			base.Draw (rect);
			
			annotation = Annotation as WeatherForecastAnnotation;
			if (annotation == null)
				return;
			
			// Get the current graphics context
			using (var context = UIGraphics.GetCurrentContext ()) {
			
				context.SetLineWidth (1.0f);
			
				// Draw the gray pointed shape:
				path = new CGPath ();
				path.MoveToPoint (14.0f, 0.0f);
				path.AddLineToPoint (0.0f, 0.0f);
				path.AddLineToPoint (55.0f, 50.0f);
				context.AddPath (path);
			
				context.SetFillColor (UIColor.LightGray.CGColor);
				context.SetStrokeColor (UIColor.Gray.CGColor);
				context.DrawPath (CGPathDrawingMode.FillStroke);
			
				// Draw the cyan rounded box
				path = new CGPath ();
				path.MoveToPoint (15.0f, 0.5f);
				path.AddArcToPoint (59.5f, 00.5f, 59.5f, 05.0f, 5.0f);
				path.AddArcToPoint (59.5f, 69.5f, 55.5f, 69.5f, 5.0f);
				path.AddArcToPoint (10.5f, 69.5f, 10.5f, 64.0f, 5.0f);
				path.AddArcToPoint (10.5f, 00.5f, 15.5f, 00.5f, 5.0f);
				context.AddPath (path);
			
				context.SetFillColor (UIColor.Cyan.CGColor);
				context.SetStrokeColor (UIColor.Blue.CGColor);
				context.DrawPath (CGPathDrawingMode.FillStroke);
			
				// Create the location & temperature string
				WeatherForecast forecast = annotation.Forecast;
				NSString temperature = new NSString (string.Format ("{0}\n{1} / {2}", forecast.Place, forecast.High, forecast.Low));
			
				// Draw the text in black
				UIColor.Black.SetColor ();
				temperature.DrawString (new CGRect (15.0f, 5.0f, 50.0f, 40.0f), UIFont.SystemFontOfSize (11.0f));
				temperature.Dispose ();
			
				// Draw the icon for the weather condition
				string imageName = string.Format ("WeatherMap.WeatherIcons.{0}.png", forecast.Condition);
				UIImage image = UIImage.FromResource (typeof(WeatherAnnotationView).Assembly, imageName);
				image.Draw (new CGRect (12.5f, 28.0f, 45.0f, 45.0f));
				image.Dispose ();
			}
		}
	}
}

