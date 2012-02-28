
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_StandardControls.Screens.iPhone.SegmentedControl
{
	public partial class SegmentedControls2_iPhone : UIViewController
	{
		UISegmentedControl segControl1;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public SegmentedControls2_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public SegmentedControls2_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public SegmentedControls2_iPhone () : base("SegmentedControls2_iPhone", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.Title = "Programmatic Segmented Controls";
			
			segControl1 = new UISegmentedControl ();
			segControl1.ControlStyle = UISegmentedControlStyle.Bordered;
			segControl1.InsertSegment ("One", 0, false);
			segControl1.InsertSegment ("Two", 1, false);
			segControl1.SetWidth (100f, 1);
			segControl1.SelectedSegment = 1;
			segControl1.Frame = new System.Drawing.RectangleF (20, 20, 280, 44);
			this.View.AddSubview (segControl1);
			
			segControl1.ValueChanged += delegate(object sender, EventArgs e) {
				Console.WriteLine ("Item " + (sender as UISegmentedControl).SelectedSegment.ToString () + " selected");
			};
			
		}
		
	}
}

