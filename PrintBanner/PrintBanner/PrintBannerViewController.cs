using Foundation;
using System;
using UIKit;

namespace PrintBanner {
	public partial class PrintBannerViewController : UIViewController {
		private static readonly string [] FontNames = { "American Typewriter", "Snell Roundhand", "Courier", "Arial" };

		private static readonly UIColor [] Colors = { UIColor.Black, UIColor.Orange, UIColor.Purple, UIColor.Red };

		private const float DefaultFontSize = 48f;

		private const float PaddingFactor = 0.1f;

		private UISimpleTextPrintFormatter textFormatter;

		public PrintBannerViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			textField.EditingDidEndOnExit += (sender, e) => textField.ResignFirstResponder ();
		}

		partial void Print (UIButton sender)
		{
			var controller = UIPrintInteractionController.SharedPrintController;
			controller.CutLengthForPaper = delegate (UIPrintInteractionController printController, UIPrintPaper paper)
			{
				// Create a font with arbitrary size so that you can calculate the approximate
				// font points per screen point for the height of the text.
				var font = textFormatter.Font;

				var text = new NSString (textField.Text);
				var attributes = new UIStringAttributes { Font = font };
				var size = text.GetSizeUsingAttributes (attributes);

				nfloat approximateFontPointPerScreenPoint = font.PointSize / size.Height;

				// Create a new font using a size  that will fill the width of the paper
				font = GetSelectedFont ((float) (paper.PrintableRect.Size.Width * approximateFontPointPerScreenPoint));

				// Calculate the height and width of the text with the final font size
				attributes.Font = font;
				var finalTextSize = text.GetSizeUsingAttributes (attributes);

				// Set the UISimpleTextFormatter font to the font with the size calculated
				textFormatter.Font = font;

				// Calculate the margins of the roll. Roll printers may have unprintable areas
				// before and after the cut.  We must add this to our cut length to ensure the
				// printable area has enough room for our text.
				nfloat lengthOfMargins = paper.PaperSize.Height - paper.PrintableRect.Size.Height;

				// The cut length is the width of the text, plus margins, plus some padding
				return finalTextSize.Width + lengthOfMargins + paper.PrintableRect.Size.Width * PaddingFactor;
			};

			var printInfo = UIPrintInfo.PrintInfo;
			printInfo.OutputType = UIPrintInfoOutputType.General;
			printInfo.Orientation = UIPrintInfoOrientation.Landscape;
			printInfo.JobName = textField.Text;

			textFormatter = new UISimpleTextPrintFormatter (textField.Text) {
				Color = GetSelectedColor (),
				Font = GetSelectedFont ()
			};

			controller.PrintInfo = printInfo;
			controller.PrintFormatter = textFormatter;
			controller.Present (true, OnPrintingComplete);

			printInfo.Dispose ();
			printInfo = null;
		}

		private void OnPrintingComplete (UIPrintInteractionController controller, bool completed, NSError error)
		{
			if (completed && error != null) {
				var message = $"Due to error in domain `{error.Domain}` with code: {error.Code}";
				Console.WriteLine ($"FAILED! {message}");

				var alert = UIAlertController.Create ("Failed!", message, UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
				ShowViewController (alert, this);
			}
		}

		private UIFont GetSelectedFont (float size = DefaultFontSize)
		{
			return UIFont.FromName (FontNames [fontSelection.SelectedSegment], size) ?? UIFont.SystemFontOfSize (size);
		}

		private UIColor GetSelectedColor ()
		{
			return Colors [colorSelection.SelectedSegment];
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (textFormatter != null) {
				textFormatter.Dispose ();
				textFormatter = null;
			}
		}
	}
}
