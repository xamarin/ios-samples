//
// Controls sample in C#
//

using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace MonoCatalog {
	
	public partial class ControlsViewController : UITableViewController {
	
		//
		// This datasource describes how the UITableView should render the
		// contents.   We have a number of sections determined by the
		// samples in our container class, and 2 rows per section:
		//
		//   Row 0: the actual styled button
		//   Row 1: the text information about the button
		//
		class DataSource : UITableViewDataSource {
			ControlsViewController cvc;
			static NSString kDisplayCell_ID = new NSString ("DisplayCellID");
			static NSString kSourceCell_ID = new NSString ("SourceCellID");
			
			public DataSource (ControlsViewController cvc)
			{
				this.cvc = cvc;
			}
	
			public override nint NumberOfSections (UITableView tableView)
			{
				return cvc.samples.Length;
			}
	
			public override string TitleForHeader (UITableView tableView, nint section)
			{
				return cvc.samples [section].Title;
			}
	
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return 2;
			}
	
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;
	
				if (indexPath.Row == 0){
					cell = tableView.DequeueReusableCell (kDisplayCell_ID);
					if (cell == null){
						cell = new UITableViewCell (UITableViewCellStyle.Default, kDisplayCell_ID);
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					} else {
						// The cell is being recycled, remove the old content
						
						UIView viewToRemove = cell.ContentView.ViewWithTag (kViewTag);
						if (viewToRemove != null)
							viewToRemove.RemoveFromSuperview ();
					}
					cell.TextLabel.Text = cvc.samples [indexPath.Section].Label;
					cell.ContentView.AddSubview (cvc.samples [indexPath.Section].Control);
				} else {
					cell = tableView.DequeueReusableCell (kSourceCell_ID);
					if (cell == null){
						// Construct the cell with reusability (the second argument is not null)
						cell = new UITableViewCell (UITableViewCellStyle.Default, kSourceCell_ID);
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
						var label = cell.TextLabel;
						
						label.Opaque = false;
						label.TextAlignment = UITextAlignment.Center;
						label.TextColor = UIColor.Gray;
						label.Lines = 2;
						label.HighlightedTextColor = UIColor.Black;
						label.Font = UIFont.SystemFontOfSize (12f);
					}
					cell.TextLabel.Text = cvc.samples [indexPath.Section].Source;
				}
	
				return cell;
			}
		}
	
		class TableDelegate : UITableViewDelegate {
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// First row is always 50 pixes, second row 38
				return indexPath.Row == 0 ? 50f : 38f;
			}
		}
	
		// Load our definition from the NIB file
		public ControlsViewController () : base ("ControlsViewController", null)
		{
		}
	
		// For tagging our embedded controls at cell recylce time.
		const int kViewTag = 1;
	
		UIControl SwitchControl ()
		{
			var sw = new UISwitch (new CGRect (198f, 12f, 94f, 27f)){
				BackgroundColor = UIColor.Clear,
				Tag = kViewTag
			};
			sw.ValueChanged += delegate {
				// The enum variant causes a full-aot crash
				Console.WriteLine ("New state: {0}", (int) sw.State);
			};
			return sw;
		}
		
		UIControl SliderControl ()
		{
			var slider = new UISlider (new CGRect (174f, 12f, 120f, 7f)){
				BackgroundColor = UIColor.Clear,
				MinValue = 0f,
				MaxValue = 100f,
				Continuous = true,
				Value = 50f,
				Tag = kViewTag,
				AccessibilityLabel = "SimpleSlider"
			};
			slider.ValueChanged += delegate {
				Console.WriteLine ("New value {0}", slider.Value);
			};
			return slider;
		}
	
		UIControl CustomSliderControl ()
		{
			var cslider = new UISlider (new CGRect (174f, 12f, 120f, 7f)){
				BackgroundColor = UIColor.Clear,
				MinValue = 0f,
				MaxValue = 100f,
				Continuous = true,
				Value = 50f,
				Tag = kViewTag,
				AccessibilityLabel = "CustomSlider"
			};
			
			var left = UIImage.FromFile ("images/orangeslide.png");
			left = left.StretchableImage (10, 0);
			var right = UIImage.FromFile ("images/yellowslide.png");
			right = right.StretchableImage (10, 0);
	
			cslider.SetThumbImage (UIImage.FromFile ("images/slider_ball.png"), UIControlState.Normal);
			cslider.SetMinTrackImage (left, UIControlState.Normal);
			cslider.SetMaxTrackImage (right, UIControlState.Normal);
			
			cslider.ValueChanged += delegate {
				Console.WriteLine ("New value {0}", cslider.Value);
			};
			return cslider;
		}
	
		UIControl PageControl ()
		{
			var page = new UIPageControl (new CGRect (120f, 14f, 178f, 20f)){
				BackgroundColor = UIColor.Gray,
				Pages = 10,
				Tag = kViewTag
			};
	
			page.TouchUpInside += delegate {
				Console.WriteLine ("Current page: {0}", page.CurrentPage);
			};
	
			return page;
		}
	
		UIView ProgressIndicator ()
		{
			var pind = new UIActivityIndicatorView (new CGRect (265f, 12f, 40f, 40f)){
				ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray,
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin |
					UIViewAutoresizing.FlexibleRightMargin |
					UIViewAutoresizing.FlexibleTopMargin |
					UIViewAutoresizing.FlexibleBottomMargin,
				Tag = kViewTag
			};
	
			pind.StartAnimating ();
			pind.SizeToFit ();
			
			return pind;
		}
	
		UIView ProgressBar ()
		{
			return new UIProgressView (new CGRect (126f, 20f, 160f, 24f)){
				Style = UIProgressViewStyle.Default,
				Progress = 0.5f,
				Tag = kViewTag
			};
		}
			
		struct ControlSample {
			public string Title, Label, Source;
			public UIView Control;
	
			public ControlSample (string t, string l, string s, UIView c)
			{
				Title = t;
				Label = l;
				Source = s;
				Control = c;
			}
		}
	
		ControlSample [] samples;
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Controls";
	
			samples = new ControlSample [] {
				new ControlSample ("UISwitch", "Standard Switch", "controls.cs: SwitchControl ()", SwitchControl ()),
				new ControlSample ("UISlider", "Standard Slider", "controls.cs: SliderControl ()", SliderControl ()),
				new ControlSample ("UISlider", "Customized Slider", "controls.cs: CustomSliderControl ()", CustomSliderControl ()),
				new ControlSample ("UIPageControl", "Ten Pages", "controls.cs: PageControl ()", PageControl ()),
				new ControlSample ("UIActivityIndicatorView", "Style Gray", "controls.cs: ProgressIndicator ()", ProgressIndicator ()),
				new ControlSample ("UIProgressView", "Style Default", "controls.cs: ProgressBar ()", ProgressBar ()),
			};
	
			TableView.DataSource = new DataSource (this);
			TableView.Delegate = new TableDelegate ();
		}
	
		
	}
}