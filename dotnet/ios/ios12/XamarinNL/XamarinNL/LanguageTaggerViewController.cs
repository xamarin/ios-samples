namespace XamarinNL;

public partial class LanguageTaggerViewController : UIViewController, IUITextFieldDelegate
{
	const string ShowEntitiesSegue = "ShowEntitiesSegue";

	NSString[] tags = Array.Empty<NSString> ();
	NSValue[] tokenRanges = Array.Empty<NSValue> ();
	string detailViewTitle = string.Empty;

	protected LanguageTaggerViewController (IntPtr handle) : base (handle) { }

	partial void HandlePartsOfSpeechButtonTap (UIButton sender)
	{
		ShowTags (NLTagScheme.LexicalClass);
	}

	partial void HandleNamedEntitiesButtonTap (UIButton sender)
	{
		ShowTags (NLTagScheme.NameType);
	}

	public override void PrepareForSegue (UIStoryboardSegue segue, NSObject? sender)
	{
		base.PrepareForSegue (segue, sender);
		if (segue.DestinationViewController is LanguageTaggerTableViewController destination) {
			if (UserInput.Text is not null)
				destination.Text = UserInput.Text;

			destination.Tags = tags;
			destination.TokenRanges = tokenRanges;
			destination.Title = detailViewTitle;
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

	void ShowTags (NLTagScheme tagScheme)
	{
		if (!String.IsNullOrWhiteSpace (UserInput.Text))
		{
			var tagger = new NLTagger (new NLTagScheme[] { tagScheme }){
				String = UserInput.Text,
			};
			var range = new NSRange (0, UserInput.Text.Length);

			tags = tagger.GetTags (range, NLTokenUnit.Word, tagScheme, NLTaggerOptions.OmitWhitespace, out NSValue[]? ranges);
			if (ranges is not null)
				tokenRanges = ranges;
			detailViewTitle = tagScheme == NLTagScheme.NameType ? "Named Entities" : "Parts of Speech";

			PerformSegue (ShowEntitiesSegue, this);
		}
	}
}
