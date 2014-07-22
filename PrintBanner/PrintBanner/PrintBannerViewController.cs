using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace PrintBanner {

	public partial class PrintBannerViewController : UIViewController {

		const float DefaultFontSize = 48;
		const float PaddingFactor = 0.1f;

		UISimpleTextPrintFormatter textformatter;

		public PrintBannerViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			textField.EditingDidEndOnExit += (sender, e) => {
				textField.ResignFirstResponder ();
			};

			printButton.TouchUpInside += Print;
		}

		public void Print (object sender, EventArgs args)
		{
			UIPrintInteractionController controller = UIPrintInteractionController.SharedPrintController;
			if (controller == null) {
				Console.WriteLine ("Couldn't get shared UIPrintInteractionController");
				return;
			}

			controller.CutLengthForPaper = delegate (UIPrintInteractionController printController, UIPrintPaper paper) {
				// Create a font with arbitrary size so that you can calculate the approximate
  				// font points per screen point for the height of the text. 
				UIFont font = textformatter.Font;

				NSString str = new NSString (textField.Text);
				UIStringAttributes attributes = new UIStringAttributes ();
				attributes.Font = font;
				CGSize size = str.GetSizeUsingAttributes (attributes);

				nfloat approximateFontPointPerScreenPoint = font.PointSize / size.Height;

				// Create a new font using a size  that will fill the width of the paper 
				font = SelectFont ((float)(paper.PrintableRect.Size.Width * approximateFontPointPerScreenPoint));

				// Calculate the height and width of the text with the final font size
				attributes.Font = font;
				CGSize finalTextSize = str.GetSizeUsingAttributes (attributes);

				// Set the UISimpleTextFormatter font to the font with the size calculated
				textformatter.Font = font;

				// Calculate the margins of the roll. Roll printers may have unprintable areas
			    // before and after the cut.  We must add this to our cut length to ensure the
			    // printable area has enough room for our text.
				nfloat lengthOfMargins = paper.PaperSize.Height - paper.PrintableRect.Size.Height;

				// The cut length is the width of the text, plus margins, plus some padding 
				return (float)(finalTextSize.Width + lengthOfMargins + paper.PrintableRect.Size.Width * PaddingFactor);
			};

			UIPrintInfo printInfo = UIPrintInfo.PrintInfo;
			printInfo.OutputType = UIPrintInfoOutputType.General;
			printInfo.Orientation = UIPrintInfoOrientation.Landscape;
			printInfo.JobName = textField.Text;

			textformatter = new UISimpleTextPrintFormatter (textField.Text) {
				Color = SelectedColor,
				Font = SelectFont ()
			};

			controller.PrintInfo = printInfo;
			controller.PrintFormatter = textformatter;

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
				controller.PresentFromRectInView (printButton.Frame, View, true, PrintingComplete);
			else
				controller.Present (true, PrintingComplete);
		}

		void PrintingComplete (UIPrintInteractionController printInteractionController, bool completed, NSError error)
		{
			if (completed && error != null) {
				string message = String.Format ("Due to error in domain `{0}` with code: {1}", error.Domain, error.Code);
				Console.WriteLine ("FAILED! {0}", message);
				new UIAlertView ("Failed!", message, null, "OK", null).Show ();
			}
		}

		static string[] font_names = { "American Typewriter", "Snell Roundhand", "Courier", "Arial" };

		UIFont SelectFont (float size = DefaultFontSize)
		{
			var font = UIFont.FromName (font_names [fontSelection.SelectedSegment], size);
			if (font == null)
				font = UIFont.SystemFontOfSize (size);
			return font;
		}

		static UIColor[] colors = { UIColor.Black, UIColor.Orange, UIColor.Purple, UIColor.Red };

		UIColor SelectedColor {
			get { return colors [colorSelection.SelectedSegment]; }
		}
	}
}