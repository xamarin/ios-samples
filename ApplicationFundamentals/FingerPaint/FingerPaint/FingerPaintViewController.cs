using CoreGraphics;
using UIKit;

namespace FingerPaint {
	public partial class FingerPaintViewController : UIViewController {
		// UITextField derivative used for invoking picker views
		class NoCaretField : UITextField {
			public NoCaretField () : base (new CGRect ())
			{
				BorderStyle = UITextBorderStyle.Line;
			}

			public override CGRect GetCaretRectForPosition (UITextPosition position)
			{
				return new CGRect ();
			}
		}

		public FingerPaintViewController ()
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			// White view covering entire screen 
			UIView contentView = new UIView () {
				BackgroundColor = UIColor.White
			};
			View = contentView;

			// Vertical UIStackView offset from status bar
			CGRect rect = UIScreen.MainScreen.Bounds;
			rect.Y += 20;
			rect.Height -= 20;

			UIStackView vertStackView = new UIStackView (rect) {
				Axis = UILayoutConstraintAxis.Vertical,
			};
			contentView.Add (vertStackView);

			// Horizontal UIStackView for tools
			UIStackView horzStackView = new UIStackView {
				Axis = UILayoutConstraintAxis.Horizontal,
				Alignment = UIStackViewAlignment.Center,
				Distribution = UIStackViewDistribution.EqualSpacing
			};
			vertStackView.AddArrangedSubview (horzStackView);

			// FingerPaintCanvasView for drawing
			FingerPaintCanvasView canvasView = new FingerPaintCanvasView ();
			vertStackView.AddArrangedSubview (canvasView);

			// Add space at left to horizontal UIStackView
			horzStackView.AddArrangedSubview (new UILabel (new CGRect (0, 0, 10, 10)));

			// Construct UIPickerView for choosing color, but don't add it to any view
			PickerDataModel<UIColor> colorModel = new PickerDataModel<UIColor> {
				Items =
				{
					new NamedValue<UIColor>("Red", UIColor.Red),
					new NamedValue<UIColor>("Green", UIColor.Green),
					new NamedValue<UIColor>("Blue", UIColor.Blue),
					new NamedValue<UIColor>("Cyan", UIColor.Cyan),
					new NamedValue<UIColor>("Magenta", UIColor.Magenta),
					new NamedValue<UIColor>("Yellow", UIColor.Yellow),
					new NamedValue<UIColor>("Black", UIColor.Black),
					new NamedValue<UIColor>("Gray", UIColor.Gray),
					new NamedValue<UIColor>("White", UIColor.White)
				}
			};

			UIPickerView colorPicker = new UIPickerView {
				Model = colorModel
			};

			// Ditto for UIPickerView for stroke thickness
			PickerDataModel<float> thicknessModel = new PickerDataModel<float> {
				Items =
				{
					new NamedValue<float>("Thin", 2),
					new NamedValue<float>("Thinish", 5),
					new NamedValue<float>("Medium", 10),
					new NamedValue<float>("Thickish", 20),
					new NamedValue<float>("Thick", 50)
				}
			};

			UIPickerView thicknessPicker = new UIPickerView {
				Model = thicknessModel
			};

			// Create UIToolbar for dismissing picker when it's displayed
			var toolbar = new UIToolbar (new CGRect (0, 0, UIScreen.MainScreen.Bounds.Width, 44)) {
				BarStyle = UIBarStyle.Default,
				Translucent = true
			};

			// Set Font to be used in tools
			UIFont font = UIFont.SystemFontOfSize (24);

			// Create a NoCaretField text field for invoking color picker & add to horizontal UIStackView
			//  (technique from Xamarin.Forms iOS PickerRenderer
			UITextField colorTextField = new NoCaretField {
				Text = "Red",
				InputView = colorPicker,
				InputAccessoryView = toolbar,
				Font = font
			};
			horzStackView.AddArrangedSubview (colorTextField);

			// Use ValueChanged handler to change the color
			colorModel.ValueChanged += (sender, args) => {
				colorTextField.Text = colorModel.SelectedItem.Name;
				canvasView.StrokeColor = colorModel.SelectedItem.Value.CGColor;
			};

			// Ditto for the thickness
			UITextField thicknessTextField = new NoCaretField {
				Text = "Thin",
				InputView = thicknessPicker,
				InputAccessoryView = toolbar,
				Font = font
			};
			horzStackView.AddArrangedSubview (thicknessTextField);

			thicknessModel.ValueChanged += (sender, args) => {
				thicknessTextField.Text = thicknessModel.SelectedItem.Name;
				canvasView.StrokeWidth = thicknessModel.SelectedItem.Value;
			};

			// Now add a Done button to the toolbar to rest text fields
			var spacer = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
			var doneButton = new UIBarButtonItem (UIBarButtonSystemItem.Done, (o, a) => {
				colorTextField.ResignFirstResponder ();
				thicknessTextField.ResignFirstResponder ();
			});

			toolbar.SetItems (new [] { spacer, doneButton }, false);

			// Create the Clear button 
			UIButton button = new UIButton (UIButtonType.RoundedRect) {
				Font = font
			};
			horzStackView.AddArrangedSubview (button);

			button.Layer.BorderColor = UIColor.Black.CGColor;
			button.Layer.BorderWidth = 1;
			button.Layer.CornerRadius = 10;
			button.SetTitle ("Clear", UIControlState.Normal);
			button.SetTitleColor (UIColor.Black, UIControlState.Normal);

			button.TouchUpInside += (sender, args) => {
				canvasView.Clear ();
			};

			// Add space at right to horizontal UIStackView
			horzStackView.AddArrangedSubview (new UILabel (new CGRect (0, 0, 10, 10)));
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}
