using System;
using UIKit;
using PdfKit;
using CoreGraphics;
using Foundation;

namespace PDFAnnotationWidgetsAdvanced {
	/**
	 ViewController first loads a path to our MyForm.pdf file through the application's main bundle. This URL
	 is then used to instantiate a PDFDocument. On success, the document is assigned to our PDFView, which
	 was setup in InterfaceBuilder. Once the document has been successfully loaded, we can extract the first
	 page in order to begin adding our widget annotations.
	 
	 ViewController adds the following widget types to the extracted PDFPage: three text fields, two radio
	 buttons, three checkboxes, and one push button. To tell PDFKit which type of interactive element to add
	 to your document, you must explicitly set the widgetFieldType. Similarly, for button widgets you must
	 explicitly set the widgetControlType.
	 
	 This class also includes a few extra widget-specific properties which are worth mentioning:
	 maximumLength & hasComb, fieldName & buttonWidgetStateString, isMultiline, and action & PDFActionResetForm.
	 
	 See the README for more detail on both widget annotation creation, and in-depth explanations regarding the
	 widget-specific properties.
	*/
	public partial class ViewController : UIViewController {
		#region Constructors
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Load our simple PDF document, retrieve the first page
			var documentURL = NSBundle.MainBundle.GetUrlForResource ("MyForm", "pdf");
			if (documentURL != null) {
				var document = new PdfDocument (documentURL);
				var page = document.GetPage (0);

				// Set our document to the view, center it, and set a background color
				PDFView.Document = document;
				PDFView.AutoScales = true;
				PDFView.BackgroundColor = UIColor.LightGray;

				// Add Name: and Date: fields
				InsertFormFields (page);

				// Add Question 1 widgets: "Have you been to a music festival before?"
				InsertRadioButtons (page);

				// Add Question 2 widgets: "Which of the following music festivals have you attended?"
				InsertCheckBoxes (page);

				// Question 3: "Give one recommendation to improve a music festival:"
				InsertMultilineTextBox (page);

				// Reset Form
				InsertResetButton (page);
			}
		}
		#endregion

		#region Private methods
		private void InsertFormFields (PdfPage page)
		{

			var bounds = page.GetBoundsForBox (PdfDisplayBox.Crop);

			var textFieldNameBox = new CGRect (169, bounds.Height - 102, 371, 23);
			var textFieldName = new PdfAnnotation (textFieldNameBox, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Text.GetConstant (),
				BackgroundColor = UIColor.Blue.ColorWithAlpha (0.25f),
				Font = UIFont.SystemFontOfSize (18)
			};
			page.AddAnnotation (textFieldName);

			var textFieldDateBounds = new CGRect (283, bounds.Height - 135, 257, 22);
			var textFieldDate = new PdfAnnotation (textFieldDateBounds, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Text.GetConstant (),
				BackgroundColor = UIColor.Blue.ColorWithAlpha (0.25f),
				Font = UIFont.SystemFontOfSize (18),
				MaximumLength = 5,
				Comb = true
			};
			page.AddAnnotation (textFieldDate);
		}

		private void InsertRadioButtons (PdfPage page)
		{

			var bounds = page.GetBoundsForBox (PdfDisplayBox.Crop);

			// Yes button
			var radioButtonYesBounds = new CGRect (135, bounds.Height - 249, 24, 24);
			var radioButtonYes = new PdfAnnotation (radioButtonYesBounds, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Button.GetConstant (),
				WidgetControlType = PdfWidgetControlType.RadioButton,
				FieldName = "Radio Button",
				ButtonWidgetStateString = "Yes"
			};
			page.AddAnnotation (radioButtonYes);

			// Yes button
			var radioButtonNoBounds = new CGRect (210, bounds.Height - 249, 24, 24);
			var radioButtonNo = new PdfAnnotation (radioButtonNoBounds, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Button.GetConstant (),
				WidgetControlType = PdfWidgetControlType.RadioButton,
				FieldName = "Radio Button",
				ButtonWidgetStateString = "No"
			};
			page.AddAnnotation (radioButtonNo);

		}

		private void InsertCheckBoxes (PdfPage page)
		{

			var bounds = page.GetBoundsForBox (PdfDisplayBox.Crop);

			var checkboxLoremFestivalBounds = new CGRect (255, bounds.Height - 370, 24, 24);
			var checkboxLoremFestival = new PdfAnnotation (checkboxLoremFestivalBounds, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Button.GetConstant (),
				WidgetControlType = PdfWidgetControlType.CheckBox
			};
			page.AddAnnotation (checkboxLoremFestival);

			var checkboxIpsumFestivalBounds = new CGRect (255, bounds.Height - 417, 24, 24);
			var checkboxIpsumFestival = new PdfAnnotation (checkboxIpsumFestivalBounds, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Button.GetConstant (),
				WidgetControlType = PdfWidgetControlType.CheckBox
			};
			page.AddAnnotation (checkboxIpsumFestival);

			var checkboxDolumFestivalBounds = new CGRect (255, bounds.Height - 464, 24, 24);
			var checkboxDolumFestival = new PdfAnnotation (checkboxDolumFestivalBounds, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Button.GetConstant (),
				WidgetControlType = PdfWidgetControlType.CheckBox
			};
			page.AddAnnotation (checkboxDolumFestival);

		}

		public void InsertMultilineTextBox (PdfPage page)
		{

			var bounds = page.GetBoundsForBox (PdfDisplayBox.Crop);

			var textFieldMultilineBox = new CGRect (90, bounds.Height - 632, 276, 80);
			var textFieldMultiline = new PdfAnnotation (textFieldMultilineBox, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Text.GetConstant (),
				BackgroundColor = UIColor.Blue.ColorWithAlpha (0.25f),
				Font = UIFont.SystemFontOfSize (24),
				Multiline = true
			};
			page.AddAnnotation (textFieldMultiline);
		}

		public void InsertResetButton (PdfPage page)
		{

			var bounds = page.GetBoundsForBox (PdfDisplayBox.Crop);

			var resetButtonBounds = new CGRect (90, bounds.Height - 680, 106, 32);
			var resetButton = new PdfAnnotation (resetButtonBounds, PdfAnnotationSubtype.Widget.GetConstant (), null) {
				WidgetFieldType = PdfAnnotationWidgetSubtype.Button.GetConstant (),
				WidgetControlType = PdfWidgetControlType.PushButton,
				Caption = "Obliviate!"
			};
			page.AddAnnotation (resetButton);

			// Create PDFActionResetForm action to clear form fields.
			var resetFormAction = new PdfActionResetForm () {
				FieldsIncludedAreCleared = false
			};
			resetButton.Action = resetFormAction;

		}
		#endregion
	}
}
