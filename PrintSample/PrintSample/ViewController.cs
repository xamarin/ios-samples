using System;
using UIKit;

namespace PrintSample {
	public partial class ViewController : UIViewController {
		protected ViewController (IntPtr handle) : base (handle) { }

		partial void Print (UIButton sender)
		{
			var printInfo = UIPrintInfo.PrintInfo;
			printInfo.JobName = "My first Print Job";
			printInfo.OutputType = UIPrintInfoOutputType.General;

			var textFormatter = new UISimpleTextPrintFormatter ("Once upon a time...") {
				StartPage = 0,
				MaximumContentWidth = 6 * 72,
				PerPageContentInsets = new UIEdgeInsets (72, 72, 72, 72),
			};

			var printer = UIPrintInteractionController.SharedPrintController;
			printer.PrintInfo = printInfo;
			printer.PrintFormatter = textFormatter;
			printer.ShowsPageRange = true;
			printer.Present (true, (handler, completed, error) => {
				if (!completed && error != null) {
					Console.WriteLine ($"Error: {error.LocalizedDescription ?? ""}");
				}
			});

			printInfo.Dispose ();
			textFormatter.Dispose ();
		}
	}
}
