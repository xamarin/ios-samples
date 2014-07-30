using System;
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreText;

namespace TextKitDemo
{
	/*
	 * The view controller for the inteactive color sample.
	 */
	public partial class InteractiveTextColoringViewController : TextViewController
	{
		InteractiveTextColoringTextStorage textStorage;

		public InteractiveTextColoringViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			textStorage = new InteractiveTextColoringTextStorage ();
			CGRect newTextViewRect = View.Bounds;
			newTextViewRect.X += 8;
			newTextViewRect.Width -= 16;

			var layoutManager = new NSLayoutManager ();
			var container = new NSTextContainer (new CGSize (newTextViewRect.Size.Width, float.MaxValue));
			container.WidthTracksTextView = true;

			layoutManager.AddTextContainer (container);
			textStorage.AddLayoutManager (layoutManager);


			var newTextView = new UITextView (newTextViewRect, container);
			newTextView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			newTextView.ScrollEnabled  = true;
			newTextView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;

			View.Add (newTextView);

			var tokens = new Dictionary<string, NSDictionary> ();
			tokens.Add ("Alice", NSDictionary.FromObjectAndKey (UIColor.Red, UIStringAttributeKey.ForegroundColor));
			tokens.Add ("Rabbit", NSDictionary.FromObjectAndKey (UIColor.Orange, UIStringAttributeKey.ForegroundColor));
			tokens.Add ("DefaultTokenName", NSDictionary.FromObjectAndKey (UIColor.Black, UIStringAttributeKey.ForegroundColor));
			textStorage.Tokens = tokens;

			SetText ();
		}

		void SetText()
		{
			textStorage.BeginEditing ();
			if (model != null)
				textStorage.SetString (model.GetAttributedText ());
			else
				textStorage.SetString (new NSAttributedString ("Alice was beginning to get very tired of sitting " +
					"by her sister on the bank, and of having nothing to do: once or twice she had peeped into the " +
				    "book her sister was reading, but it had no pictures or conversations in it, 'and what is the " +
					"use of a book,' thought Alice 'without pictures or conversation?'\n\nSo she was considering " +
					"in her own mind (as well as she could, for the hot day made her feel very sleepy and stupid), " +
					"whether the pleasure of making a daisy-chain would be worth the trouble of getting up and " +
					"picking the daisies, when suddenly a White Rabbit with pink eyes ran close by her.\n\nThere was " +
					"nothing so VERY remarkable in that; nor did Alice think it so VERY much out of the way to hear the " +
					"Rabbit say to itself, 'Oh dear! Oh dear! I shall be late!' (when she thought it over afterwards, it " +
					"occurred to her that she ought to have wondered at this, but at the time it all seemed quite natural); " +
					"but when the Rabbit actually TOOK A WATCH OUT OF ITS WAISTCOAT-POCKET, and looked at it, and then " +
					"hurried on, Alice started to her feet, for it flashed across her mind that she had never before seen " +
					"a rabbit with either a waistcoat-pocket, or a watch to take out of it, and burning with curiosity, " +
					"she ran across the field after it, and fortunately was just in time to see it pop down a large rabbit-hole " +
					"under the hedge.\n\nIn another moment down went Alice after it, never once considering how in the world " +
					"she was to get out again. \n\nThe rabbit-hole went straight on like a tunnel for some way, and then dipped " +
					"suddenly down, so suddenly that Alice had not a moment to think about stopping herself before she found " +
					"herself falling down a very deep well.  Either the well was very deep, or she fell very slowly, for she " +
					"had plenty of time as she went down to look about her and to wonder what was going to happen next. " +
					"First, she tried to look down and make out what she was coming to, but it was too dark to see anything; " +
					"then she looked at the sides of the well, and noticed that they were filled with cupboards and book-shelves; " +
					"here and there she saw maps and pictures hung upon pegs. She took down a jar from one of the shelves as she passed; " +
					"it was labelled 'ORANGE MARMALADE', but to her great disappointment it was empty: she did not like to drop the jar " +
					"for fear of killing somebody, so managed to put it into one of the cupboards as she fell past it. "));
			textStorage.EndEditing ();
		}
	}
}

