using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace UICatalog
{
    public partial class PickerViewController : UIViewController, IUIPickerViewDataSource, IUIPickerViewDelegate
    {
        public PickerViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Show that a given row is selected. This is off by default.
            pickerView.ShowSelectionIndicator = true;
            ConfigurePickerView();
        }

        private void ConfigurePickerView()
        {
            // Set the default selected rows (the desired rows to initially select will vary from app to app).
            var selectedRows = new Dictionary<ColorComponent, int> 
            {
                { ColorComponent.Red, 13 },
                { ColorComponent.Green, 41 },
                { ColorComponent.Blue, 24 }
            };

            // kvp - means KeyValuePair
            foreach (var kvp in selectedRows)
            {
                var selectedRow = kvp.Value;
                var colorComponent = (int)kvp.Key;

                // Note that the delegate method on UIPickerViewDelegate is not triggered when manually
                // calling UIPickerView.selectRow(:inComponent:animated:). To do this, we fire off delegate
                // method manually.
                pickerView.Select(selectedRow, colorComponent, true);
                Selected(pickerView, selectedRow, colorComponent);
            }
        }

        #region IUIPickerViewDelegate

        [Export("pickerView:attributedTitleForRow:forComponent:")]
        public NSAttributedString GetAttributedTitle(UIPickerView pickerView, nint row, nint component)
        {
            var colorValue = row * RGB.Offset;

            var value = colorValue / RGB.Max;
            var redColorComponent = RGB.Min;
            var greenColorComponent = RGB.Min;
            var blueColorComponent = RGB.Min;

            switch ((ColorComponent)(int)component)
            {
                case ColorComponent.Red:
                    redColorComponent = value;
                    break;

                case ColorComponent.Green:
                    greenColorComponent = value;
                    break;

                case ColorComponent.Blue:
                    blueColorComponent = value;
                    break;

                default:
                    throw new InvalidOperationException("Invalid row/component combination for picker view.");
            }

            // Set the foreground color for the entire attributed string.
            var foregroundColor = new UIColor(redColorComponent, greenColorComponent, blueColorComponent, 1f);
            var attributes = new UIStringAttributes { ForegroundColor = foregroundColor};
            return new NSAttributedString($"{colorValue}", attributes);
        }

        [Export("pickerView:didSelectRow:inComponent:")]
        public void Selected(UIPickerView pickerView, nint row, nint component)
        {
            var colorComponentValue = RGB.Offset * row / RGB.Max;
            var color = colorView.BackgroundColor;
            color.GetRGBA(out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);

            switch ((ColorComponent)(int)component)
            {
                case ColorComponent.Red:
                    color = new UIColor(colorComponentValue, green, blue, alpha);
                    break;

                case ColorComponent.Green:
                    color = new UIColor(red, colorComponentValue, blue, alpha);
                    break;

                case ColorComponent.Blue:
                    color = new UIColor(red, green, colorComponentValue, alpha);
                    break;
            }

            colorView.BackgroundColor = color;
        }

        #endregion

        #region IUIPickerViewDataSource

        public nint GetComponentCount(UIPickerView pickerView)
        {
            return Enum.GetValues(typeof(ColorComponent)).Length;
        }

        public nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return (int)RGB.Max / (int)RGB.Offset + 1; 
        }

        #endregion

        /* helpers */

        enum ColorComponent
        {
            Red,
            Green,
            Blue
        }

        struct RGB
        {
            public const float Max = 255f;
            public const float Min = 0f;
            public const float Offset = 5f;
        }
    }
}