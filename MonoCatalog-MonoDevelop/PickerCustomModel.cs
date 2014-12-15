//
// The PickerViewController
//
using Foundation;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using System;

public class CustomView : UIView {
	const float MAIN_FONT_SIZE = 18.0f;
	const float MIN_MAIN_FONT_SIZE = 16.0f;

	public CustomView (CGRect frame) : base (new CGRect (CGPoint.Empty, new CGSize (Width, Height))) {
		AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
		BackgroundColor = UIColor.Clear;
	}

	public override void Draw (CGRect rect) {
		nfloat yCoord = (Bounds.Size.Height - Image.Size.Height) / 2f;
		CGPoint point = new CGPoint (10.0f, yCoord);

		Image.Draw (point);

		yCoord = (Bounds.Size.Height - MAIN_FONT_SIZE) / 2f;
		point = new CGPoint (10.0f + Image.Size.Width + 10.0f, yCoord);
		Title.DrawString (point, UIFont.SystemFontOfSize (MAIN_FONT_SIZE));
	}

	public string Title { get; set; }
	public UIImage Image { get; set; }

	public const float Width = 200f;
	public const float Height = 44f;
}

public class CustomPickerModel : UIPickerViewModel {
	List <CustomView> views;

	public CustomPickerModel () : base () {
		views = new List <CustomView> ();
		var empty = CGRect.Empty;

		views.Add (new CustomView (empty) { Title = "Early Morning", Image = UIImage.FromFile ("images/12-6AM.png") });
		views.Add (new CustomView (empty) { Title = "Late Morning", Image = UIImage.FromFile ("images/6-12AM.png") });
		views.Add (new CustomView (empty) { Title = "Afternoon", Image = UIImage.FromFile ("images/12-6PM.png") });
		views.Add (new CustomView (empty) { Title = "Evening", Image = UIImage.FromFile ("images/6-12PM.png") });
	}

	public override nfloat GetComponentWidth (UIPickerView pickerView, nint component) {
		return CustomView.Width;
	}

	public override nfloat GetRowHeight (UIPickerView pickerView, nint component) {
		return CustomView.Height;
	}

	public override nint GetRowsInComponent (UIPickerView pickerView, nint component) {
		return views.Count;
	}

	public override nint GetComponentCount (UIPickerView pickerView) {
		return 1;
	}

	public override UIView GetView (UIPickerView pickerView, nint row, nint component, UIView view) {
				return views[(int)row];
	}
}
