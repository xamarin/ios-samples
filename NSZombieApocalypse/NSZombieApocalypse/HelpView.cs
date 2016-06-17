using System.Threading.Tasks;

using UIKit;
using Foundation;
using CoreGraphics;

namespace NSZombieApocalypse
{
	public sealed class HelpView : UIView
	{
		UITextView textView;
		SymbolMarkView nextButton;

		public event  HelpDidCloseHandler HelpDidClose;

		public HelpView (CGRect frame): base(frame)
		{
			BackgroundColor = UIColor.White;
			Layer.CornerRadius = 8;

			var closeFrame = new CGRect (10, frame.Size.Height - 140, 80, 80);
			var closeView = new SymbolMarkView (closeFrame);
			closeView.TouchUpInside += async (s, e) => await Hide ();
			AddSubview (closeView);
			closeView.Symbol = "X";
			closeView.AccessibilityLabel = "Close";

			var label = new UILabel (new CGRect (0, 20, frame.Size.Width, 40)) {
				Font = UIFont.FromName ("HelveticaNeue-Italic", 82),
				Text = "NSZombieApocalypse",
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Center,
			};
			label.SizeToFit ();
			var labelFrame = label.Frame;
			labelFrame.X = (frame.Size.Width - labelFrame.Size.Width) / 2;
			label.Frame = labelFrame;
			AddSubview (label);
			label.AccessibilityTraits = UIAccessibilityTrait.Header;

			var nextFrame = new CGRect (frame.Size.Width - 90, frame.Size.Height - 140, 80, 80);
			nextButton = new SymbolMarkView (nextFrame);
			nextButton.TouchUpInside += (s, e) => NextSlide ();
			AddSubview (nextButton);
			nextButton.Symbol = "->";
			nextButton.AccessibilityLabel = "Next";

			float width = (float) frame.Size.Width * 0.6f;
			var textViewFrame = new CGRect (
				200 + ((frame.Size.Width - 200) - width) / 2,
				label.Frame.GetMaxY () + 30,
				width,
				frame.Size.Height * 0.6f
			);
			textView = new UITextView (textViewFrame.Integral ());
			AddSubview (textView);
			textView.Editable = false;
			textView.Font = UIFont.FromName ("HelveticaNeue", 36);
			textView.Text = NSBundle.MainBundle.LocalizedString ("helpText1", null, "Strings");

			var imageView = new UIImageView (UIImage.FromBundle ("smaller-zombie1.png"));
			var imageFrame = new CGRect (label.Frame.X - 20, textViewFrame.Y, imageView.Frame.Width, imageView.Frame.Height);
			imageView.Frame = imageFrame;
			AddSubview (imageView);

			imageView.IsAccessibilityElement = true;
			imageView.AccessibilityLabel = "Poorly drawn, yet oddly menancing, zombie";

		}

		public void PreviousSlide ()
		{
			textView.Text = NSBundle.MainBundle.LocalizedString ("helpText1", null, "Strings");
			nextButton.TouchUpInside += (s, e) => NextSlide ();
			nextButton.Symbol = "->";
			nextButton.AccessibilityLabel = "Next";
		}

		public void NextSlide ()
		{
			textView.Text = NSBundle.MainBundle.LocalizedString ("helpText2", null, "Strings");
			nextButton.Symbol = "<-";
			nextButton.TouchUpInside += (s, e) => PreviousSlide ();
			nextButton.AccessibilityLabel = "Previous";
		}

		public async Task Hide ()
		{
			CGRect frame = Frame;
			frame.Y = -frame.Size.Height;
			await UIView.AnimateAsync (.35, () => {
				Frame = frame;
			});
			HelpDidClose (this);
		}

		public override void Draw (CGRect rect)
		{
			rect.Height -= 40;
			UIColor.White.SetFill ();
			var path = UIBezierPath.FromRoundedRect (rect, UIRectCorner.BottomRight | UIRectCorner.BottomLeft, new CGSize (8, 8));
			path.Fill ();
		}

		public void Show ()
		{
			CGRect frame = Frame;
			frame.Y = 0;
			UIView.Animate (.35, () => {
				Frame = frame;});
		}
	}
	public delegate void HelpDidCloseHandler (HelpView helpView);
}

