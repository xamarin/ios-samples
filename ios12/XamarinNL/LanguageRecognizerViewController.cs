using Foundation;
using System;
using UIKit;
using NaturalLanguage;

namespace XamarinNL
{
    public partial class LanguageRecognizerViewController : UIViewController, IUITextFieldDelegate
    {
        const string ShowLanguageProbabilitiesSegue = "ShowLanguageProbabilitiesSegue";

        NSDictionary<NSString, NSNumber> probabilities;

        public LanguageRecognizerViewController(IntPtr handle) : base(handle) { }

        partial void HandleLanguageProbabilitiesButtonTap(UIButton sender)
        {
            UserInput.ResignFirstResponder();
            if (!String.IsNullOrWhiteSpace(UserInput.Text))
            {
                var recognizer = new NLLanguageRecognizer();
                recognizer.Process(UserInput.Text);
                probabilities = recognizer.GetNativeLanguageHypotheses(10);
                PerformSegue(ShowLanguageProbabilitiesSegue, this);
            }
        }

        partial void HandleDetermineLanguageButtonTap(UIButton sender)
        {
            UserInput.ResignFirstResponder();
            if (!String.IsNullOrWhiteSpace(UserInput.Text))
            {
                NLLanguage lang = NLLanguageRecognizer.GetDominantLanguage(UserInput.Text);
                DominantLanguageLabel.Text = lang.ToString();
            }
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            var destination = segue.DestinationViewController as LanguageProbabilityTableViewController;
            if (destination != null)
            {
                destination.Probabilities = probabilities;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UserInput.Delegate = this;
        }

        [Export("textFieldShouldReturn:")]
        public bool ShouldReturn(UITextField textField)
        {
            UserInput.ResignFirstResponder();
            return true;
        }
    }
}