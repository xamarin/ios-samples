//
// Port of the toolbar sample to C#
//
using Foundation;
using UIKit;
using System;
using CoreGraphics;

namespace MonoCatalog {
	
	public partial class ToolbarViewController : UIViewController {
	
		UIBarButtonSystemItem currentSystemItem;
		UIToolbar toolbar;
		static string [] pickerViewArray = {
			"Done",
			"Cancel",
			"Edit",
			"Save",
			"Add",
			"FlexibleSpace",
			"FixedSpace",
			"Compose",
			"Reply",
			"Action",
			"Organize",
			"Bookmarks",
			"Search",
			"Refresh",
			"Stop",
			"Camera",
			"Trash",
			"Play",
			"Pause",
			"Rewind",
			"FastForward",
			"Undo",
			"Redo"
		};
		
		public ToolbarViewController () : base ("ToolbarViewController", null) {}
	
		//
		// This is the general callback that we give to buttons to show how to hook events up
		// this could have been an anonymous delegate if desired.
		//
		void Action (object sender, EventArgs args)
		{
			Console.WriteLine ("I am the action");
		}
		
		void CreateToolbarItems ()
		{
			// The order is mapped one to one to the UIBarButtonItemStyle
						var style = UIBarButtonItemStyle.Plain;
				
			var systemItem = new UIBarButtonItem (currentSystemItem, Action){
				Style = style
			};
	
			var flexItem = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
	
			var infoItem = new UIBarButtonItem (UIImage.FromFile ("images/segment_tools.png"), style, Action);
	
			var customItem = new UIBarButtonItem ("Item", style, Action);
	
			toolbar.Items = new UIBarButtonItem [] { systemItem, flexItem, customItem, infoItem };
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
	
			Title = "Toolbar";
	
			NavigationController.NavigationBar.Translucent = false;
			View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			toolbar = new UIToolbar() {
				BarStyle = UIBarStyle.Default
			};
			toolbar.SizeToFit ();
			nfloat toolbarHeight = toolbar.Frame.Height;
			var mainViewBounds = View.Bounds;
			toolbar.Frame = new CGRect (mainViewBounds.X, (float)(mainViewBounds.Y + mainViewBounds.Height - toolbarHeight * 2 + 2),
							mainViewBounds.Width, toolbarHeight);
			View.AddSubview (toolbar);
			currentSystemItem = UIBarButtonSystemItem.Done;
			CreateToolbarItems ();
	
			systemButtonPicker.Model = new ItemPickerModel (this);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			barStyleSegControl.AccessibilityLabel = "BarStyle";
			buttonItemStyleSegControl.AccessibilityLabel = "BarButtonStyle";
		}
		
		partial void toggleStyle (UISegmentedControl sender)
		{
			var style = UIBarButtonItemStyle.Plain;
	
			switch (sender.SelectedSegment){
			case 0: 
				style = UIBarButtonItemStyle.Plain;
				break;
			case 1: 
				style = UIBarButtonItemStyle.Bordered;
				break;
			case 2: 
				style = UIBarButtonItemStyle.Done;
				break;
			}
			foreach (var item in toolbar.Items)
				item.Style = style;
		}
	
		partial void toggleBarStyle (UISegmentedControl sender)
		{
			switch (sender.SelectedSegment){
			case 0:
				toolbar.BarStyle = UIBarStyle.Default;
				break;
				
			case 1:
				toolbar.BarStyle = UIBarStyle.Black;
				break;
				
			case 2:
				toolbar.BarStyle = UIBarStyle.BlackTranslucent;
				break;
			}
			
		}
	
		partial void toggleTintColor (UISwitch sender)
		{
			if (sender.On){
				toolbar.TintColor = UIColor.Red;
				barStyleSegControl.Enabled = false;
				barStyleSegControl.Alpha = 0.5f;
			} else {
				toolbar.TintColor = null;
				barStyleSegControl.Enabled = true;
				barStyleSegControl.Alpha  = 1.0f;
			}
		}
	
		public class ItemPickerModel : UIPickerViewModel {
			ToolbarViewController tvc;
			
			public ItemPickerModel (ToolbarViewController tvc)
			{
				this.tvc = tvc;
			}
			
			public override void Selected (UIPickerView picker, int row, int component)
			{
							
								//tvc.currentSystemItem = picker.SelectedRowInComponent (0);
				tvc.CreateToolbarItems ();
			}
	
			public override string GetTitle (UIPickerView picker, int row, int component)
			{
				return pickerViewArray [row];
			}
	
			public override float GetComponentWidth (UIPickerView picker, int component)
			{
				return 240f;
			}
	
			public override float GetRowHeight (UIPickerView picker, int component)
			{
				return 40f;
			}
	
			public override int GetRowsInComponent (UIPickerView pickerView, int component)
			{
				return pickerViewArray.Length;
			}
	
			public override int GetComponentCount (UIPickerView v)
			{
				return 1;
			}
		}
	}
}
