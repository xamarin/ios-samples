using Foundation;
using UIKit;

namespace UICatalog {
	public partial class TextFieldsViewController : UIViewController {

		[Export ("initWithCoder:")]
		public TextFieldsViewController (NSCoder coder): base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			RegularKeyboardTextField.InputAccessoryView = new CustomInputAccessoryView ("Regular Text");
			EmailKeyboardTextField.InputAccessoryView = new CustomInputAccessoryView ("Email Address");
			NumberPadTextField.InputAccessoryView = new CustomInputAccessoryView ("Number Pad");
			NumbersAndPunctuationTextField.InputAccessoryView = new CustomInputAccessoryView ("Numbers and Punctation");
		}
	}
}
