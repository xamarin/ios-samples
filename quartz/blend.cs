//
// The blend view controller
//
using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

[Register]
public partial class QuartzBlendingViewController : QuartzViewController {

	// You can add more colors, they are sorted at startup
	static UIColor [] Colors = new UIColor [] {
		UIColor.Red,
		UIColor.Green,
		UIColor.Blue,
		UIColor.Yellow,
		UIColor.Magenta,
		UIColor.Cyan,
		UIColor.Orange,
		UIColor.Purple,
		UIColor.Brown,
		UIColor.White,
		UIColor.LightGray,
		UIColor.DarkGray,
		UIColor.Black
	};

	static string [] BlendModes = new string [] {
		"Normal",
		"Multiply",
		"Screen",
		"Overlay",
		"Darken",
		"Lighten",
		"ColorDodge",
		"ColorBurn",
		"SoftLight",
		"HardLight",
		"Difference",
		"Exclusion",
		"Hue",
		"Saturation",
		"Color",
		"Luminosity",
		// Porter-Duff Blend Modes
		"Clear",
		"Copy",
		"SourceIn",
		"SourceOut",
		"SourceAtop",
		"DestinationOver",
		"DestinationIn",
		"DestinationOut",
		"DestinationAtop",
		"XOR",
		"PlusDarker",
		"PlusLighter",
	};
	
	public QuartzBlendingViewController (CreateView creator, string title, string info) : base (creator, "BlendView", title, info)
	{
		Array.Sort (Colors, ColorSortByLuminance);
	}

	//
	// The view loaded: create our QuartzView
	//
	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		picker.Model = new BlendSelector (this);
		
		var qbv = (QuartzBlendingView) quartzView;
		picker.Select (Array.IndexOf (Colors, qbv.DestinationColor), 0, false);
		picker.Select (Array.IndexOf (Colors, qbv.SourceColor), 1, false);
		picker.Select ((int) qbv.BlendMode, 2, false);
	}

	static float LuminanceForColor (UIColor color)
	{
		CGColor cgcolor = color.CGColor;
		var components = cgcolor.Components;
		float luminance = 0;
		
		switch (cgcolor.ColorSpace.Model){
		case CGColorSpaceModel.Monochrome:
			// For grayscale colors, the luminance is the color value
			luminance = components[0];
			break;
			
		case CGColorSpaceModel.RGB:
			// For RGB colors, we calculate luminance assuming sRGB Primaries as per
			// http://en.wikipedia.org/wiki/Luminance_(relative)
			luminance = 0.2126f * components[0] + 0.7152f * components[1] + 0.0722f * components[2];
			break;
			
		default:
			// We don't implement support for non-gray, non-rgb colors at this time.
			// Since our only consumer is colorSortByLuminance, we return a larger than normal
			// value to ensure that these types of colors are sorted to the end of the list.
			luminance = 2.0f;
			break;
		}
		return luminance;
	}

	// Simple comparison function that sorts the two (presumed) UIColors according to their luminance value.
	static int ColorSortByLuminance (UIColor color1,  UIColor color2)
	{
		float luminance1 = LuminanceForColor(color1);
		float luminance2 = LuminanceForColor(color2);

		if (luminance1 == luminance2) 
			return 0;
		else if (luminance1 < luminance2)
			return -1;
		else
			return 1;
	}

	[Register]
	public class BlendSelector : UIPickerViewModel {
		QuartzBlendingViewController parent;
		
		public BlendSelector (QuartzBlendingViewController parent)
		{
			this.parent = parent;
		}
		
		public override int GetComponentCount (UIPickerView picker){
			return 3;
		}

		public override int GetRowsInComponent (UIPickerView picker, int component)
		{
			if (component == 0 || component == 1)
				return Colors.Length;
			return BlendModes.Length;
		}

		public override float GetComponentWidth (UIPickerView picker, int component)
		{
			if (component == 0 || component == 1)
				return 48f;
			return 192f;
		}      

		const int kColorTag = 1;
		const int kLabelTag = 1;
			
		public override UIView GetView (UIPickerView picker, int row, int component, UIView view)
		{
			var size = picker.RowSizeForComponent (component);
			
			if (component == 0 || component == 1){
				if (view == null || view.Tag != kColorTag){
					view = new UIView (new RectangleF (0, 0, size.Width-4, size.Height-4)){
						Tag = kColorTag
					};
				}
				view.BackgroundColor = Colors [row];
			} else {
				if (view == null || view.Tag != kLabelTag){
					view = new UILabel (new RectangleF (0, 0, size.Width-4, size.Height-4)){
						Tag = kLabelTag,
						Opaque = false,
						BackgroundColor = UIColor.Clear
					};
				}
				var label = (UILabel) view;
				label.TextColor = UIColor.Black;
				label.Text = BlendModes [row];
				label.Font = UIFont.BoldSystemFontOfSize (18f);
			}
			return view;
		}

		public override void Selected (UIPickerView picker, int row, int component)
		{
			var qbv = (QuartzBlendingView) parent.quartzView;
			qbv.DestinationColor = Colors [picker.SelectedRowInComponent (0)];
			qbv.SourceColor = Colors [picker.SelectedRowInComponent (1)];
			qbv.BlendMode = (CGBlendMode) picker.SelectedRowInComponent (2);
			qbv.SetNeedsDisplay ();
		}
	}		
}

[Register]
public class QuartzBlendingView : QuartzView {
	public UIColor SourceColor { get; set; }
	public UIColor DestinationColor { get; set; }
	public CGBlendMode BlendMode { get; set; }
	
	public QuartzBlendingView () : base () {
		SourceColor = UIColor.White;
		DestinationColor = UIColor.Black;
		BlendMode = CGBlendMode.Normal;
	}

	public override void DrawInContext (CGContext context)
	{
		// Start with a background whose color we don't use in the demo
		context.SetGrayFillColor (0.2f, 1);
		context.FillRect (Bounds);

		// We want to just lay down the background without any blending so we use the Copy mode rather than Normal
		context.SetBlendMode(CGBlendMode.Copy);
					
		// Draw a rect with the "background" color - this is the "Destination" for the blending formulas
		context.SetFillColorWithColor (DestinationColor.CGColor);
		context.FillRect(new RectangleF (110, 20, 100, 100));
		
		// Set up our blend mode
		context.SetBlendMode (BlendMode);
		
		// And draw a rect with the "foreground" color - this is the "Source" for the blending formulas
		context.SetFillColorWithColor (SourceColor.CGColor);
		context.FillRect (new RectangleF (60, 45, 200, 50));
	}

}
