//
// ElementBadge.cs: defines the Badge Element.
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010, Novell, Inc.
//
// Code licensed under the MIT X11 license
//
using System;
using System.Collections;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.Foundation;

namespace PaintCode {
	/// <summary>
	/// Lifted this code from MT.D source, so it could be customized
	/// </summary>
	public class CustomBadgeElement {
		public CustomBadgeElement ()
		{
		}
		public static UIImage MakeCalendarBadge (string smallText, string bigText)
		{
			UIGraphics.BeginImageContext (new SizeF (42, 42));

			
			// ------------- START PAINTCODE ----------------


//// Abstracted Graphic Attributes
			var textContent = bigText;
			var text2Content = smallText;

//// General Declarations
var colorSpace = CGColorSpace.CreateDeviceRGB();
var context = UIGraphics.GetCurrentContext();

//// Color Declarations
UIColor dateRed = UIColor.FromRGBA(0.83f, 0.11f, 0.06f, 1.00f);

//// Gradient Declarations
var greyGradientColors = new CGColor [] {UIColor.White.CGColor, UIColor.FromRGBA(0.57f, 0.57f, 0.57f, 1.00f).CGColor, UIColor.Black.CGColor};
var greyGradientLocations = new float [] {0.65f, 0.75f, 0.75f};
var greyGradient = new CGGradient(colorSpace, greyGradientColors, greyGradientLocations);

//// Shadow Declarations
var dropShadow = UIColor.DarkGray.CGColor;
var dropShadowOffset = new SizeF(2, 2);
var dropShadowBlurRadius = 1;


//// Rounded Rectangle Drawing
var roundedRectanglePath = UIBezierPath.FromRoundedRect(new RectangleF(1.5f, 1.5f, 38, 38), 4);
context.SaveState();
context.SetShadowWithColor(dropShadowOffset, dropShadowBlurRadius, dropShadow);
context.BeginTransparencyLayer(null);
roundedRectanglePath.AddClip();
context.DrawLinearGradient(greyGradient, new PointF(20.5f, 1.5f), new PointF(20.5f, 39.5f), 0);
context.EndTransparencyLayer();
context.RestoreState();

UIColor.Black.SetStroke();
roundedRectanglePath.LineWidth = 1;
roundedRectanglePath.Stroke();


//// Rounded Rectangle 2 Drawing
var roundedRectangle2Path = UIBezierPath.FromRoundedRect(new RectangleF(2, 28, 37, 11), UIRectCorner.BottomLeft | UIRectCorner.BottomRight, new SizeF(4, 4));
dateRed.SetFill();
roundedRectangle2Path.Fill();



//// Text Drawing
var textRect = new RectangleF(2, 0, 37, 28);
UIColor.Black.SetFill();
new NSString(textContent).DrawString(textRect, UIFont.FromName("Helvetica-Bold", 24), UILineBreakMode.WordWrap, UITextAlignment.Center);


//// Text 2 Drawing
var text2Rect = new RectangleF(2, 27, 37, 15);
UIColor.White.SetFill();
new NSString(text2Content).DrawString(text2Rect, UIFont.FromName("HelveticaNeue-Bold", 9), UILineBreakMode.WordWrap, UITextAlignment.Center);








			// ------------- END PAINTCODE ----------------

			var converted = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return converted;

		}
	}
}