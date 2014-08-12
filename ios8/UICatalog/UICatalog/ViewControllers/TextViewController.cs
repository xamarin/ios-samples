using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class TextViewController : UIViewController
	{
		[Outlet]
		private UITextView TextView { get; set; }

		[Outlet] // Used to adjust the text view's height when the keyboard hides and shows.
		private NSLayoutConstraint TextViewBottomLayoutGuideConstraint { get; set; }

		private NSObject _willShowObserver;
		private NSObject _willHideObserver;

		public TextViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureTextView ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Listen for changes to keyboard visibility so that we can adjust the text view accordingly.
			_willShowObserver = UIKeyboard.Notifications.ObserveWillShow (OnKeyboardWillChangeFrame);
			_willHideObserver = UIKeyboard.Notifications.ObserveWillHide (OnKeyboardWillChangeFrame);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			NSNotificationCenter.DefaultCenter.RemoveObserver (_willShowObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (_willHideObserver);
		}

		#region Keyboard Event Notifications

		private void OnKeyboardWillChangeFrame(object sender, UIKeyboardEventArgs keyboardArg)
		{
			// Convert the keyboard frame from screen to view coordinates.
			var keyboardViewBeginFrame = View.ConvertRectFromView (keyboardArg.FrameBegin, fromView: View.Window);
			var keyboardViewEndFrame = View.ConvertRectFromView (keyboardArg.FrameEnd, fromView: View.Window);
			var originDelta = keyboardViewEndFrame.Y - keyboardViewBeginFrame.Y;

			// The text view should be adjusted, update the constant for this constraint.
			TextViewBottomLayoutGuideConstraint.Constant -= originDelta;
			View.SetNeedsUpdateConstraints ();

			UIView.Animate(keyboardArg.AnimationDuration, 0, UIViewAnimationOptions.BeginFromCurrentState, ()=> {
				View.LayoutIfNeeded();
			}, null);

			// Scroll to the selected text once the keyboard frame changes.
			NSRange selectedRange = TextView.SelectedRange;
			TextView.ScrollRangeToVisible (selectedRange);
		}

		#endregion

		private void ConfigureTextView()
		{
			TextView.Font = UIFont.FromDescriptor (UIFontDescriptor.PreferredBody, 0);

			TextView.TextColor = UIColor.Black;
			TextView.BackgroundColor = UIColor.White;
			TextView.ScrollEnabled = true;

			// Let's modify some of the attributes of the attributed string.
			// You can modify these attributes yourself to get a better feel for what they do.
			// Note that the initial text is visible in the storyboard.
			var attributedText = new NSMutableAttributedString (TextView.AttributedText);

			// Use NSString so the result of rangeOfString is an NSRange, not Range<String.Index>.
			NSString text = (NSString)TextView.Text;

			// Find the range of each element to modify.
			var boldRange = CalcRangeFor (text, "bold".Localize ());
			var highlightedRange = CalcRangeFor (text, "highlighted".Localize ());
			var underlinedRange = CalcRangeFor (text, "underlined".Localize ());
			var tintedRange = CalcRangeFor (text, "tinted".Localize ());

			// Add bold. Take the current font descriptor and create a new font descriptor with an additional bold trait.
			var boldFontDescriptor = TextView.Font.FontDescriptor.CreateWithTraits (UIFontDescriptorSymbolicTraits.Bold);
			var boldFont = UIFont.FromDescriptor (boldFontDescriptor, 0);
			attributedText.AddAttribute (UIStringAttributeKey.Font, value: boldFont, range: boldRange);

			// Add highlight.
			attributedText.AddAttribute (UIStringAttributeKey.BackgroundColor, value: ApplicationColors.Green, range: highlightedRange);

			// Add underline.
			attributedText.AddAttribute (UIStringAttributeKey.UnderlineStyle, value: NSNumber.FromInt32 ((int)NSUnderlineStyle.Single), range: underlinedRange);

			// Add tint.
			attributedText.AddAttribute (UIStringAttributeKey.ForegroundColor, value: ApplicationColors.Blue, range: tintedRange);

			// Add image attachment.
			var img = UIImage.FromBundle ("text_view_attachment");
			var textAttachment = new NSTextAttachment {
				Image = img,
				Bounds = new RectangleF(PointF.Empty, img.Size),
			};

			var textAttachmentString = NSAttributedString.CreateFrom (textAttachment);
			attributedText.Append (textAttachmentString);

			TextView.AttributedText = attributedText;
		}

		private NSRange CalcRangeFor(string source, string substring)
		{
			NSRange range = new NSRange {
				Location = source.IndexOf (substring),
				Length = substring.Length
			};

			return range;
		}

		private void AdjustTextViewSelection(UITextView textView)
		{
			// Ensure that the text view is visible by making the text view frame smaller as text can be slightly cropped at the bottom.
			// Note that this is a workwaround to a bug in iOS.
			textView.LayoutIfNeeded ();

			var caretRect = textView.GetCaretRectForPosition (textView.SelectedTextRange.End);
			caretRect.Height += textView.TextContainerInset.Bottom;
			textView.ScrollRectToVisible (caretRect, animated: false);
		}

		// MARK: UITextViewDelegate
		[Export("textViewDidBeginEditing:")]
		private void TextViewDidBeginEditing(UITextView textView)
		{
			// Provide a "Done" button for the user to select to signify completion with writing text in the text view.
			var doneBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, DoneBarButtonItemClicked);
			NavigationItem.SetRightBarButtonItem (doneBarButtonItem, animated: true);
			AdjustTextViewSelection (textView);
		}

		[Export("textViewDidChangeSelection:")]
		private void TextViewDidChangeSelection(UITextView textView)
		{
			AdjustTextViewSelection (textView);
		}

		private void DoneBarButtonItemClicked(object sender, EventArgs e)
		{
			// Dismiss the keyboard by removing it as the first responder.
			TextView.ResignFirstResponder ();
			NavigationItem.SetRightBarButtonItem (null, animated: true);
		}
	}
}
