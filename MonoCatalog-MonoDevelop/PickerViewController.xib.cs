//
// The PickerViewController
//
using Foundation;
using UIKit;
using CoreGraphics;
using System;

namespace MonoCatalog {
	
	public partial class PickerViewController : UIViewController  {
		UIPickerView myPickerView, customPickerView;
		UIDatePicker datePickerView;
		UILabel label;
		UIColor backgroundColor;
		UIColor labelTextColor;
		bool greaterThanSeven;

		public PickerViewController () : base ("PickerViewController", null)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Picker";
			greaterThanSeven = UIDevice.CurrentDevice.CheckSystemVersion (7, 0);
			backgroundColor = greaterThanSeven ? UIColor.White : UIColor.Clear;
			labelTextColor = greaterThanSeven ? UIColor.Black : UIColor.White;

			NavigationController.NavigationBar.Translucent = false;
			NavigationController.NavigationBar.BackgroundColor = backgroundColor;
			View.BackgroundColor = greaterThanSeven ? UIColor.White : UIColor.Black;
			CreatePicker ();
			CreateDatePicker ();
			CreateCustomPicker ();
	
			// Colors
			buttonBarSegmentedControl.TintColor = UIColor.DarkGray;
			pickerStyleSegmentedControl.TintColor = UIColor.DarkGray;
	
			label = new UILabel (new CGRect (20f, myPickerView.Frame.Y - 30f, View.Bounds.Width - 40f, 30f)){
				Font = UIFont.SystemFontOfSize (14),
				TextAlignment = UITextAlignment.Center,
				TextColor = labelTextColor,
				BackgroundColor = UIColor.Clear
			};
			View.AddSubview (label);
			buttonBarSegmentedControl.SelectedSegment = 0;
			datePickerView.Mode = UIDatePickerMode.Date;
		}
	
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.NavigationBar.BarStyle = greaterThanSeven ? UIBarStyle.Default : UIBarStyle.Black;
			UIApplication.SharedApplication.StatusBarStyle = greaterThanSeven ? UIStatusBarStyle.Default : UIStatusBarStyle.BlackOpaque;
			TogglePickers (buttonBarSegmentedControl);
		}
	
		public override void ViewWillDisappear (bool animated)
		{
			if (currentPicker != null)
				currentPicker.Hidden = true;
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
		}
		
		CGRect PickerFrameWithSize (CGSize size)
		{
			var screenRect = UIScreen.MainScreen.ApplicationFrame;
			return new CGRect (0f, screenRect.Height - 84f - size.Height, size.Width, size.Height);
		}
	
		UIView currentPicker;
		void ShowPicker (UIView picker)
		{
			if (currentPicker != null){
				currentPicker.Hidden = true;
				label.Text = "";
			}
	
			picker.Hidden = false;
			currentPicker = picker;
		}
		
		#region Hooks from Interface Builder
		[Export ("togglePickers:")]
		public void TogglePickers (UISegmentedControl sender)
		{
			switch (sender.SelectedSegment){
			case 0: 
				pickerStyleSegmentedControl.Hidden = true;
				segmentLabel.Hidden = true;
				ShowPicker (myPickerView);
				break;
	
			case 1:
				pickerStyleSegmentedControl.SelectedSegment = 1;
				datePickerView.Mode = UIDatePickerMode.Date;
				pickerStyleSegmentedControl.Hidden = false;
				segmentLabel.Hidden = false;
				ShowPicker (datePickerView);
				break;
	
			case 2:
				pickerStyleSegmentedControl.Hidden = true;
				segmentLabel.Hidden = true;
				ShowPicker (customPickerView);
				break;
			}
		}
	
		[Export ("togglePickerStyle:")]
		public void TogglePickerStyle (UISegmentedControl sender)
		{
			switch (sender.SelectedSegment){
			case 0: // time
				datePickerView.Mode = UIDatePickerMode.Time;
				break;
				
			case 1: // date
				datePickerView.Mode = UIDatePickerMode.Date;
				break;
				
			case 2: // date & time
				datePickerView.Mode = UIDatePickerMode.DateAndTime;
				break;
				
			case 3: // counter
				datePickerView.Mode = UIDatePickerMode.CountDownTimer;
				break;
			}
	
			datePickerView.Date = NSDate.Now; //DateTime.Now;
	
			Console.WriteLine ("Date is: {0} {1} {2}", NSDate.Now.ToString (), ((NSDate) DateTime.Now).ToString (), DateTime.Now);
		}
		#endregion
		
		#region Custom picker
		public void CreateCustomPicker ()
		{
			customPickerView = new UIPickerView (CGRect.Empty) {
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				Model = new CustomPickerModel (),
				ShowSelectionIndicator = true,
				BackgroundColor = backgroundColor,
				Hidden = true
			};
			customPickerView.Frame = PickerFrameWithSize (customPickerView.SizeThatFits (CGSize.Empty));
			View.AddSubview (customPickerView);
		}
	
		#endregion
		
		#region Date picker
		public void CreateDatePicker ()
		{
			datePickerView = new UIDatePicker (CGRect.Empty) {
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				Mode = UIDatePickerMode.Date,
				BackgroundColor = backgroundColor,
				Hidden = true
			};
			datePickerView.Frame = PickerFrameWithSize (datePickerView.SizeThatFits (CGSize.Empty));
			View.AddSubview (datePickerView);
		}
		#endregion
	
		#region People picker
		
		void CreatePicker ()
		{
			//
			// Empty is used, since UIPickerViews have auto-sizing,
			// all that is required is the origin
			//
			myPickerView = new UIPickerView (CGRect.Empty){
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				ShowSelectionIndicator = true,
				Model = new PeopleModel (this),
				BackgroundColor = backgroundColor,
				Hidden = true
			};
			// Now update it:
			myPickerView.Frame = PickerFrameWithSize (myPickerView.SizeThatFits (CGSize.Empty));
			View.AddSubview (myPickerView);
		}
	
		public class PeopleModel : UIPickerViewModel {
			static string [] names = new string [] {
				"Brian Kernighan",
				"Dennis Ritchie",
				"Ken Thompson",
				"Kirk McKusick",
				"Rob Pike",
				"Dave Presotto",
				"Steve Johnson"
			};
	
			PickerViewController pvc;
			public PeopleModel (PickerViewController pvc) {
				this.pvc = pvc;
			}
			
			public override int GetComponentCount (UIPickerView v)
			{
				return 2;
			}
	
			public override int GetRowsInComponent (UIPickerView pickerView, int component)
			{
				return names.Length;
			}
	
			public override string GetTitle (UIPickerView picker, int row, int component)
			{
				if (component == 0)
					return names [row];
				else
					return row.ToString ();
			}
	
			public override void Selected (UIPickerView picker, int row, int component)
			{
				pvc.label.Text = String.Format ("{0} - {1}",
							    names [picker.SelectedRowInComponent (0)],
							    picker.SelectedRowInComponent (1));
			}
			
			public override float GetComponentWidth (UIPickerView picker, int component)
			{
				if (component == 0)
					return 240f;
				else
					return 40f;
			}
	
			public override float GetRowHeight (UIPickerView picker, int component)
			{
				return 40f;
			}
		}
		#endregion
	}
}
