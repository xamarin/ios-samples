//
// Port of the segment sample
//
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Drawing;

namespace MonoCatalog {
	
	public partial class SegmentViewController : UIViewController {
	
		public SegmentViewController () : base ("SegmentViewController", null) {}
		
		UILabel MakeLabel (string title, RectangleF frame)
		{
			return new UILabel (frame) {
				TextAlignment = UITextAlignment.Left,
				Text = title,
				Font = UIFont.BoldSystemFontOfSize (17f),
				TextColor = new UIColor (76/255f, 86/255f, 108/255f, 1.0f),
				BackgroundColor = UIColor.Clear
			};
		}
	
		const float lHeight = 20f;
		
		void CreateControls ()
		{
			var items = new string [] { "Check", "Search", "Tools" };
			
			float yPlacement = 20f;
			
			View.AddSubview (MakeLabel ("UISegmentedControl", new RectangleF (20f, yPlacement, View.Bounds.Width - 40f, lHeight)));
			yPlacement += 40f;
			
			var segmentedControl = new UISegmentedControl (new object [] {
					UIImage.FromFile ("images/segment_check.png"),
					UIImage.FromFile ("images/segment_search.png"),
					UIImage.FromFile ("images/segment_tools.png")}){
				Frame = new RectangleF (20f, yPlacement, View.Bounds.Width - 40f, 40f),
				ControlStyle = UISegmentedControlStyle.Plain,
				SelectedSegment = 1
			};
			View.AddSubview (segmentedControl);
			yPlacement += 60f;
			
			View.AddSubview (MakeLabel ("UISegmentedControlStyleBordered", new RectangleF (20f, yPlacement, View.Bounds.Width - 40f, lHeight)));
			yPlacement += 40f;
	
			segmentedControl = new UISegmentedControl (items){
				Frame = new RectangleF (20f, yPlacement, View.Bounds.Width - 40f, 40f),
				ControlStyle = UISegmentedControlStyle.Bordered,
				SelectedSegment = 1
			};
			segmentedControl.ValueChanged += delegate {
				Console.WriteLine ("Value changed");
			};
			View.AddSubview (segmentedControl);
			yPlacement += 60f;
			
			View.AddSubview (MakeLabel ("UISegmentedControlStyleBar", new RectangleF (20f, yPlacement, View.Bounds.Width - 40f, lHeight)));
			yPlacement += 40f;
			segmentedControl = new UISegmentedControl (items){
				Frame = new RectangleF (20f, yPlacement, View.Bounds.Width - 40f, 40f),
				ControlStyle = UISegmentedControlStyle.Bar,
				SelectedSegment = 1
			};
			View.AddSubview (segmentedControl);
			yPlacement += 60f;
			
			View.AddSubview (MakeLabel ("UISegmentedControlStyleBar (tinted)", new RectangleF (20f, yPlacement, View.Bounds.Width - 40f, lHeight)));
			yPlacement += 40f;
			segmentedControl = new UISegmentedControl (items){
				Frame = new RectangleF (20f, yPlacement, View.Bounds.Width - 40f, 40f),
				ControlStyle = UISegmentedControlStyle.Bar,
				TintColor = UIColor.FromRGB (0.7f, 0.171f, 0.1f),
				SelectedSegment = 1
			};
			View.AddSubview (segmentedControl);
			yPlacement += 60f;
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			CreateControls ();
		}
	}
}
