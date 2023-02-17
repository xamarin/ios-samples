using CoreGraphics;
using Foundation;
using System;
using System.Drawing;
using UIKit;

namespace UICatalog {
	public partial class TextViewController : UIViewController, IUITextViewDelegate {
		public TextViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureTextView ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Listen for changes to keyboard visibility so that we can adjust the text view accordingly.
			willShowObserver = UIKeyboard.Notifications.ObserveWillShow (OnKeyboardWillChangeFrame);
			willHideObserver = UIKeyboard.Notifications.ObserveWillHide (OnKeyboardWillChangeFrame);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			if (willShowObserver != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (willShowObserver);
				willShowObserver.Dispose ();
				willShowObserver = null;
			}

			if (willHideObserver != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (willHideObserver);
				willHideObserver.Dispose ();
				willHideObserver = null;
			}
		}

		#region Keyboard 

		private NSObject willShowObserver;
		private NSObject willHideObserver;

		private void OnKeyboardWillChangeFrame (object sender, UIKeyboardEventArgs args)
		{
			// Convert the keyboard frame from screen to view coordinates.
			var keyboardViewBeginFrame = View.ConvertRectFromView (args.FrameBegin, View.Window);
			var keyboardViewEndFrame = View.ConvertRectFromView (args.FrameEnd, View.Window);
			var originDelta = keyboardViewEndFrame.Y - keyboardViewBeginFrame.Y;

			// The text view should be adjusted, update the constant for this constraint.
			textBottomConstraint.Constant -= originDelta;
			View.SetNeedsUpdateConstraints ();

			UIView.Animate (args.AnimationDuration, 0, UIViewAnimationOptions.BeginFromCurrentState, View.LayoutIfNeeded, null);

			// Scroll to the selected text once the keyboard frame changes.
			var selectedRange = textView.SelectedRange;
			textView.ScrollRangeToVisible (selectedRange);
		}

		#endregion

		private void ConfigureTextView ()
		{
			textView.Font = UIFont.FromDescriptor (UIFontDescriptor.PreferredBody, 0);

			textView.TextColor = UIColor.Black;
			textView.BackgroundColor = UIColor.White;
			textView.ScrollEnabled = true;

			// Let's modify some of the attributes of the attributed string.
			// You can modify these attributes yourself to get a better feel for what they do.
			// Note that the initial text is visible in the storyboard.
			var attributedText = new NSMutableAttributedString (textView.AttributedText);

			// Find the range of each element to modify.
			var text = textView.Text;
			var boldRange = GetRangeFor (text, "bold");
			var highlightedRange = GetRangeFor (text, "highlighted");
			var underlinedRange = GetRangeFor (text, "underlined");
			var tintedRange = GetRangeFor (text, "tinted");

			// Add bold. Take the current font descriptor and create a new font descriptor with an additional bold trait.
			var boldFontDescriptor = textView.Font.FontDescriptor.CreateWithTraits (UIFontDescriptorSymbolicTraits.Bold);
			var boldFont = UIFont.FromDescriptor (boldFontDescriptor, 0);
			attributedText.AddAttribute (UIStringAttributeKey.Font, boldFont, boldRange);

			// Add highlight.
			attributedText.AddAttribute (UIStringAttributeKey.BackgroundColor, UIColor.Green, highlightedRange);

			// Add underline.
			attributedText.AddAttribute (UIStringAttributeKey.UnderlineStyle, NSNumber.FromInt32 ((int) NSUnderlineStyle.Single), underlinedRange);

			// Add tint.
			attributedText.AddAttribute (UIStringAttributeKey.ForegroundColor, UIColor.Blue, tintedRange);

			// Add image attachment.
			var image = UIImage.FromBundle ("text_view_attachment");
			var textAttachment = new NSTextAttachment {
				Image = image,
				Bounds = new CGRect (PointF.Empty, image.Size),
			};

			var textAttachmentString = NSAttributedString.CreateFrom (textAttachment);
			attributedText.Append (new NSAttributedString (Environment.NewLine));
			attributedText.Append (textAttachmentString);

			textView.AttributedText = attributedText;

			image.Dispose ();
			attributedText.Dispose ();
			image = null;
			attributedText = null;

			NSRange GetRangeFor (string source, string substring)
			{
				return new NSRange {
					Location = source.IndexOf (substring, StringComparison.OrdinalIgnoreCase),
					Length = substring.Length
				};
			}
		}

		#region IUITextViewDelegate

		[Export ("textViewDidBeginEditing:")]
		public void EditingStarted (UITextView textView)
		{
			// Provide a "Done" button for the user to select to signify completion with writing text in the text view.
			var doneBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, (sender, e) => {
				// Dismiss the keyboard by removing it as the first responder.
				textView.ResignFirstResponder ();
				NavigationItem.SetRightBarButtonItem (null, true);
			});

			NavigationItem.SetRightBarButtonItem (doneBarButtonItem, true);
			AdjustTextViewSelection (textView);
		}

		[Export ("textViewDidEndEditing:")]
		public void EditingEnded (UITextView textView)
		{
			AdjustTextViewSelection (textView);
		}

		private static void AdjustTextViewSelection (UITextView textView)
		{
			// Ensure that the text view is visible by making the text view frame smaller as text can be slightly cropped at the bottom.
			// Note that this is a workwaround to a bug in iOS.
			textView.LayoutIfNeeded ();

			var caretRect = textView.GetCaretRectForPosition (textView.SelectedTextRange.End);
			caretRect.Height += textView.TextContainerInset.Bottom;
			textView.ScrollRectToVisible (caretRect, false);
		}

		#endregion
	}
}
