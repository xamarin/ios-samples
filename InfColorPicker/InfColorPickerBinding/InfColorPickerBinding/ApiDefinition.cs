using System;
using System.Drawing;

using ObjCRuntime;
using Foundation;
using UIKit;
using CoreGraphics;

namespace InfColorPicker {

	// @interface InfColorBarView : UIView
	[BaseType (typeof (UIView))]
	interface InfColorBarView {

	}

	// @interface InfColorBarPicker : UIControl
	[BaseType (typeof (UIControl))]
	interface InfColorBarPicker {

		// @property (nonatomic) float value;
		[Export ("value")]
		float Value { get; set; }
	}

	// @interface InfColorIndicatorView : UIView
	[BaseType (typeof (UIView))]
	interface InfColorIndicatorView {

		// @property (nonatomic) UIColor * color;
		[Export ("color")]
		UIColor Color { get; set; }
	}

	// @interface InfColorPickerController : UIViewController
	[BaseType (typeof (UIViewController))]
	interface InfColorPickerController {

		// @property (nonatomic) UIColor * sourceColor;
		[Export ("sourceColor")]
		UIColor SourceColor { get; set; }

		// @property (nonatomic) UIColor * resultColor;
		[Export ("resultColor")]
		UIColor ResultColor { get; set; }

		// @property (nonatomic, weak) id<InfColorPickerControllerDelegate> delegate;
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		// @property (nonatomic, weak) id<InfColorPickerControllerDelegate> delegate;
		[Wrap ("WeakDelegate")]
		InfColorPickerControllerDelegate Delegate { get; set; }

		// +(InfColorPickerController *)colorPickerViewController;
		[Static, Export ("colorPickerViewController")]
		InfColorPickerController ColorPickerViewController ();

		// +(CGSize)idealSizeForViewInPopover;
		[Static, Export ("idealSizeForViewInPopover")]
		CGSize IdealSizeForViewInPopover ();

		// -(void)presentModallyOverViewController:(UIViewController *)controller;
		[Export ("presentModallyOverViewController:")]
		void PresentModallyOverViewController (UIViewController controller);
	}

	// @protocol InfColorPickerControllerDelegate
	[BaseType(typeof(NSObject))]
	[Model]
	interface InfColorPickerControllerDelegate {

		// @optional -(void)colorPickerControllerDidFinish:(InfColorPickerController *)controller;
		[Export ("colorPickerControllerDidFinish:")]
		void ColorPickerControllerDidFinish (InfColorPickerController controller);

		// @optional -(void)colorPickerControllerDidChangeColor:(InfColorPickerController *)controller;
		[Export ("colorPickerControllerDidChangeColor:")]
		void ColorPickerControllerDidChangeColor (InfColorPickerController controller);
	}

	// @interface InfColorPickerNavigationController : UINavigationController
	[BaseType (typeof (UINavigationController))]
	interface InfColorPickerNavigationController {

	}

	// @interface InfColorSquareView : UIImageView
	[BaseType (typeof (UIImageView))]
	interface InfColorSquareView {

		// @property (nonatomic) float hue;
		[Export ("hue")]
		float Hue { get; set; }
	}

	// @interface InfColorSquarePicker : UIControl
	[BaseType (typeof (UIControl))]
	interface InfColorSquarePicker {

		// @property (nonatomic) float hue;
		[Export ("hue")]
		float Hue { get; set; }

		// @property (nonatomic) CGPoint value;
		[Export ("value")]
		CGPoint Value { get; set; }
	}

	// @interface InfSourceColorView : UIControl
	[BaseType (typeof (UIControl))]
	interface InfSourceColorView {

		// @property (nonatomic) BOOL trackingInside;
		[Export ("trackingInside")]
		bool TrackingInside { get; set; }
	}
}


