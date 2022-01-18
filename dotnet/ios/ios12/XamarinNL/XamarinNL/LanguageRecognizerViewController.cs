namespace XamarinNL;

public partial class LanguageRecognizerViewController : UIViewController, IUITextFieldDelegate
{
    const string ShowLanguageProbabilitiesSegue = "ShowLanguageProbabilitiesSegue";

    NSDictionary<NSString, NSNumber>? probabilities;

    protected LanguageRecognizerViewController (IntPtr handle) : base (handle) { }

    partial void HandleLanguageProbabilitiesButtonTap (UIButton sender)
    {
        UserInput.ResignFirstResponder ();
        if (!String.IsNullOrWhiteSpace (UserInput.Text))
        {
            var recognizer = new NLLanguageRecognizer ();
            recognizer.Process (UserInput.Text);
            probabilities = recognizer.GetNativeLanguageHypotheses (10);
            PerformSegue (ShowLanguageProbabilitiesSegue, this);
        }
    }

    partial void HandleDetermineLanguageButtonTap (UIButton sender)
    {
        UserInput.ResignFirstResponder ();
        if (!String.IsNullOrWhiteSpace (UserInput.Text))
        {
            var lang = NLLanguageRecognizer.GetDominantLanguage (UserInput.Text);
            DominantLanguageLabel.Text = lang.ToString ();
        }
    }

    public override void PrepareForSegue (UIStoryboardSegue segue, NSObject? sender)
    {
        base.PrepareForSegue (segue, sender);
        if (segue.DestinationViewController is LanguageProbabilityTableViewController destination) {
            if (probabilities is not null)
            {
                destination.Probabilities = probabilities;
            }
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
}
