using CoreGraphics;
using System;
using System.Reflection;

using UIKit;

namespace FingerPaint
{
    internal class NoCaretField : UITextField
    {
        public NoCaretField() : base(new CGRect())
        {
        }

        public override CGRect GetCaretRectForPosition(UITextPosition position)
        {
            return new CGRect();
        }
    }

    class ColorName
    {
        public ColorName(string name, UIColor color)
        {
            Name = name;
            Color = color;
        }

        public string Name { private set; get; }

        public UIColor Color { private set; get; }

        public override string ToString()
        {
            return Name;
        }
    }




    public partial class FingerPaintViewController : UIViewController
    {
        public FingerPaintViewController() // base("FingerPaintViewController", null)
        {
       //     this.EdgesForExtendedLayout = UIRectEdge.

            
        }

        public override void LoadView()
        {
            base.LoadView();

            //UIView contentView = new UIView()
            //{
            //    BackgroundColor = UIColor.White
            //};
            //View = contentView;

      //      CGRect rect = UIScreen.MainScreen.Bounds;
      //      rect.X += 20;
      //      rect.Height -= 20;

            UIStackView vertStackView = new UIStackView // (rect)
            {
                Axis = UILayoutConstraintAxis.Vertical,


                
                //         BackgroundColor = UIColor.White
             //   LayoutMargins = new UIEdgeInsets(20, 0, 0, 0)
            };
        //    contentView.AddSubview(vertStackView);

           View = vertStackView;


            UIView view = new UIView // (new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 20))
            {
                BackgroundColor = UIColor.White,
                Bounds = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 20)
            };
            vertStackView.AddArrangedSubview(view);



            UIStackView horzStackView = new UIStackView
            {
                Axis = UILayoutConstraintAxis.Horizontal,
                Alignment = UIStackViewAlignment.Center,
                Distribution = UIStackViewDistribution.FillProportionally,
      //          BackgroundColor = UIColor.White


        };
            vertStackView.AddArrangedSubview(horzStackView);


            FingerPaintCanvasView canvasView = new FingerPaintCanvasView();

            vertStackView.AddArrangedSubview(canvasView);
            // new FingerPaintCanvasView()); //  UIScreen.MainScreen.Bounds)

        //    Tuple<string, UIColor> y;

           


            var x = new { name = "Red", value = UIColor.Red };

            var n = x.name;
            var v = x.value;


            PickerDataModel<ColorName> colorModel = new PickerDataModel<ColorName>
            {
                Items =
                {
                    new ColorName("Red", UIColor.Red),
                    new ColorName("Green", UIColor.Green),
                    new ColorName("Blue", UIColor.Blue),
                    new ColorName("Cyan", UIColor.Cyan),
                    new ColorName("Magenta", UIColor.Magenta),
                    new ColorName("Yellow", UIColor.Yellow),
                    new ColorName("Black", UIColor.Black),
                    new ColorName("Gray", UIColor.Gray),
                    new ColorName("White", UIColor.White)
                }
            };
            colorModel.ValueChanged += (sender, args) =>
            {
                return;

                if (colorModel.SelectedItem != null)
                {


              //      var a = typeof(UIColor);
                    var b = a.GetProperty(colorModel.SelectedItem);
                    var c = b.GetValue(null);
                    var d = (UIColor)c;
                    var e = d.CGColor;


                 //   canvasView.StrokeColor = ((UIColor)(typeof(UIColor).GetProperty(colorModel.SelectedItem).GetValue(null))).CGColor;
                }
            };

            UIPickerView colorPicker = new UIPickerView
            {
                Model = colorModel,
                BackgroundColor = UIColor.White
            };

            var width = UIScreen.MainScreen.Bounds.Width;
            var toolbar = new UIToolbar(new CoreGraphics.CGRect(0, 0, width, 44))
            {
                BarStyle = UIBarStyle.Default,
                Translucent = true
            };



            UITextField colorTextField = new NoCaretField //  new UITextField
            {
                Text = "Color = Red",
                BackgroundColor = UIColor.White,
                TextColor = UIColor.Black,
                InputView = colorPicker,
                InputAccessoryView = toolbar,

                Font = UIFont.FromName("Courier", 48)
              //  MinimumFontSize = 24
               
            };

            horzStackView.AddArrangedSubview(colorTextField);





            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) =>
            {
                //             var s = (PickerSource)_picker.Model;
                //             if (s.SelectedIndex == -1 && Element.Items != null && Element.Items.Count > 0)
                //                UpdatePickerSelectedIndex(0);
                //            UpdatePickerFromModel(s);

                colorTextField.Text = "Color = " + colorModel.SelectedItem;

                //    canvasView.StrokeColor = (UIColor)(typeof(UIColor).GetProperty(colorModel.SelectedItem).GetValue(null))).CGColor;

                canvasView.StrokeColor = colorModel.SelectedItem.Color.CGColor;

                colorTextField.ResignFirstResponder();
            });

            toolbar.SetItems(new[] { spacer, doneButton }, false);






            //===================================

            PickerDataModel<string> tryoutModel = new PickerDataModel<string>
            {
                Items =
                {
                    "Red",
                    "Green",
                    "Blue",
                    "Cyan",
                    "Magenta",
                    "Yellow",
                    "Black",
                    "Gray",
                    "White"
                }
            };


            UIPickerView tryoutPicker = new UIPickerView
            {




                Model = tryoutModel,
                BackgroundColor = UIColor.White
            };


            UITextField textField = new UITextField
            {
                
                Text = "whatsit is it going?",
                BackgroundColor = UIColor.White,

                InputView = tryoutPicker,
                InputAccessoryView = toolbar

            };
            horzStackView.AddArrangedSubview(textField);


/*
            PickerDataModel thicknessModel = new PickerDataModel
            {
                Items =
                {
                    "Thin",
                    "Thinish",
                    "Medium",
                    "Thickish",
                    "Thick"
                }
            };
            thicknessModel.ValueChanged += (sender, args) =>
            {
                canvasView.StrokeWidth = new float[] { 2, 5, 10, 20, 50 }
                    [thicknessModel.Items.IndexOf(thicknessModel.SelectedItem)];

            };

            UIPickerView thicknessPicker = new UIPickerView
            {
                Model = thicknessModel,
                BackgroundColor = UIColor.White
            };

            horzStackView.AddArrangedSubview(thicknessPicker);
*/








            //   View = stackView;

            //      View = contentView;

            /* contentView. */
            //  );



            UIButton button = new UIButton(); //  new CoreGraphics.CGRect(100, 100, 100, 100));
            button.SetTitle("Clear", UIControlState.Normal);
            button.TouchUpInside += (sender, args) =>
            {
                canvasView.Clear();
            };

            horzStackView.AddArrangedSubview(button);







        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}