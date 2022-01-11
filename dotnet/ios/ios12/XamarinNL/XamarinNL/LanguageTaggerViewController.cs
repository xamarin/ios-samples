namespace XamarinNL
{
    public partial class LanguageTaggerViewController : UIViewController, IUITextFieldDelegate
    {
        const string ShowEntitiesSegue = "ShowEntitiesSegue";

        NSString[]? tags;
        NSValue[]? tokenRanges;
        string? detailViewTitle;

        public LanguageTaggerViewController (IntPtr handle) : base (handle) { }

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
            var destination = segue.DestinationViewController as LanguageTaggerTableViewController;
            if (destination is not null)
            {
                if (UserInput is not null && UserInput.Text is not null)
                    destination.Text = UserInput.Text;

                if (tags is not null)
                    destination.Tags = tags;

                if (tokenRanges is not null)
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
                var tagger = new NLTagger (new NLTagScheme[] { tagScheme });
                var range = new NSRange (0, UserInput.Text.Length);
                tagger.String = UserInput.Text;

                tags = tagger.GetTags (range, NLTokenUnit.Word, tagScheme, NLTaggerOptions.OmitWhitespace, out NSValue[]? ranges);
                tokenRanges = ranges;
                detailViewTitle = tagScheme == NLTagScheme.NameType ? "Named Entities" : "Parts of Speech";

                PerformSegue (ShowEntitiesSegue, this);
            }
        }
    }
}