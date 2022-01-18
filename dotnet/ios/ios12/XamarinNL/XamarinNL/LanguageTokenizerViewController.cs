namespace XamarinNL;

public partial class LanguageTokenizerViewController : UIViewController, IUITextFieldDelegate
{
	const string ShowTokensSegue = "ShowTokensSegue";

	NSValue[] tokens = Array.Empty<NSValue> ();

	protected LanguageTokenizerViewController (IntPtr handle) : base (handle) { }

	partial void HandleFindSentencesTap (UIButton sender)
	{
		ShowTokens (NLTokenUnit.Sentence);
	}

	partial void HandleFindWordsButtonTap (UIButton sender)
	{
		ShowTokens (NLTokenUnit.Word);
	}

	public override void PrepareForSegue (UIStoryboardSegue segue, NSObject? sender)
	{
		base.PrepareForSegue (segue, sender);
		if (segue.DestinationViewController is LanguageTokenizerTableViewController destination) {
			destination.Tokens = tokens;
			if (UserInput.Text is not null)
				destination.Text = UserInput.Text;
		}
	}

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		UserInput.Delegate = this;
	}

	[Export ("textFieldShouldReturn:")]
	public bool ShouldReturn (UITextField textField)
	{
		UserInput.ResignFirstResponder ();
		return true;
	}

	void ShowTokens (NLTokenUnit unit)
	{
		if (!String.IsNullOrWhiteSpace (UserInput.Text))
		{
			var tokenizer = new NLTokenizer (unit) {
				String = UserInput.Text,
			};
			var range = new NSRange (0, UserInput.Text.Length);
			tokens = tokenizer.GetTokens (range);
			PerformSegue (ShowTokensSegue, this);
		}
	}
}
