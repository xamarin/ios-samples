using System;
using UIKit;
using Foundation;

namespace QuartzSample
{

	public partial class MainViewController : UITableViewController {
	
		QuartzViewController [] controllers;

		public MainViewController (IntPtr b) : base (b) {
			controllers = new QuartzViewController [] {
				new QuartzViewController (() => new LineDrawingView (), "Lines", "LineDrawingView (linedrawing.cs)"),
				new QuartzViewController (() => new LineWidthDrawingView (), "Stroke Width", "LineWidthDrawingView (linedrawing.cs)"),
				new QuartzViewController (() => new LineCapJoinDrawingView (), "Caps & Joins ", "LineCapJoinDrawingView (linedrawing.cs)"),
				new QuartzViewController (() => new LineDashDrawingView (), "Dash Patterns", "LineDashDrawingView (linedrawing.cs)"),
				new QuartzViewController (() => new EllipseArcDrawingView (), "Ellipses and Arcs", "EllipseArcDrawingView (curvedrawing.cs)"),
				new QuartzViewController (() => new BezierDrawingView (), "Beziers & Quadratics", "BezierDrawingView (curvedrawing.cs)"),
				new QuartzViewController (() => new GradientDrawingView (), "Gradients", "GradientDrawingView (rendereddrawing.cs)"),
				new QuartzViewController (() => new PatternDrawingView (), "Patterns", "PatternDrawingView (rendereddrawing.cs)"),
				new QuartzViewController (() => new RectDrawingView (), "Rectangles", "RectDrawingView (polydrawing.cs)"),
				new QuartzViewController (() => new PolyDrawingView (), "Polygons", "PolyDrawingView (polydrawing.cs)"),
				new QuartzViewController (() => new ImageDrawingView (), "Images", "ImageDrawingView (imagedrawing.cs)"),
				new QuartzViewController (() => new PDFDrawingView (), "PDF", "PDFDrawingView (imagedrawing.cs)"),
				new QuartzBlendingViewController (() => new QuartzBlendingView (), "Blending", "QuartzBlendingView (blend.cs)")
			};
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.DataSource = new DataSource (this);
			TableView.Delegate = new TableDelegate (this);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			// Deselect the current row
			TableView.DeselectRow (TableView.IndexPathForSelectedRow, false);
	
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
		}
		
		//
		// The data source for our TableView
		//
		class DataSource : UITableViewDataSource {
			static NSString kCellIdentifier = new NSString ("MyIdentifier");
			MainViewController mvc;
			
			public DataSource (MainViewController mvc)
			{
				this.mvc = mvc;
			}
			
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return mvc.controllers.Length;
			}
	
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}
				
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (kCellIdentifier);
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, kCellIdentifier);
	
				var vc = mvc.controllers [indexPath.Row];
				cell.TextLabel.Text = vc.DemoTitle;
				cell.DetailTextLabel.Text = vc.DemoInfo;
				cell.DetailTextLabel.AdjustsFontSizeToFitWidth = true;
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
	
				return cell;
			}
		}

		//
		// This class receives notifications that happen on the UITableView
		//
		class TableDelegate : UITableViewDelegate {
			MainViewController mvc;
			
			public TableDelegate (MainViewController mvc)
			{
				this.mvc = mvc;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				var cont = mvc.controllers [indexPath.Row];
				mvc.NavigationController.PushViewController (cont, true);
			}
		}
	}
}

