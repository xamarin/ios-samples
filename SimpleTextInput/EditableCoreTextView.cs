using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using ObjCRuntime;

using CoreText;

namespace SimpleTextInput
{
	[Adopts ("UITextInput")]
	[Adopts ("UIKeyInput")]
	[Adopts ("UITextInputTraits")]
	[Register ("EditableCoreTextView")]
	public class EditableCoreTextView : UIView
	{
		StringBuilder text = new StringBuilder ();
		UITextInputStringTokenizer tokenizer;
		SimpleCoreTextView textView;
		NSDictionary markedTextStyle;
		IUITextInputDelegate inputDelegate;
		
		public delegate void ViewWillEditDelegate (EditableCoreTextView editableCoreTextView);
		public event ViewWillEditDelegate ViewWillEdit;
		
		public EditableCoreTextView (CGRect frame)
			: base (frame)
		{
			// Add tap gesture recognizer to let the user enter editing mode
			UITapGestureRecognizer tap = new UITapGestureRecognizer (Tap) {
				ShouldReceiveTouch = delegate(UIGestureRecognizer recognizer, UITouch touch) {
					// If gesture touch occurs in our view, we want to handle it
					return touch.View == this;
				}
			};
			AddGestureRecognizer (tap);
        
			// Create our tokenizer and text storage
			tokenizer = new UITextInputStringTokenizer (this);

			// Create and set up our SimpleCoreTextView that will do the drawing
			textView = new SimpleCoreTextView (Bounds.Inset (5, 5));
			textView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			UserInteractionEnabled = true;
			AutosizesSubviews = true;
			AddSubview (textView);
			textView.Text = string.Empty;
			textView.UserInteractionEnabled = false;
		}

		protected override void Dispose (bool disposing)
		{
			markedTextStyle = null;
			tokenizer = null;
			text = null;
			textView = null;
			base.Dispose (disposing);
		}
		
#region Custom user interaction

		// UIResponder protocol override - our view can become first responder to 
		// receive user text input
		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}

		// UIResponder protocol override - called when our view is being asked to resign 
		// first responder state (in this sample by using the "Done" button)  
		public override bool ResignFirstResponder ()
		{
			textView.IsEditing = false;
			return base.ResignFirstResponder ();
		}

		// Our tap gesture recognizer selector that enters editing mode, or if already
		// in editing mode, updates the text insertion point
		[Export ("Tap:")]
		public void Tap (UITapGestureRecognizer tap)
		{
			if (!IsFirstResponder) {
				// Inform controller that we're about to enter editing mode
				if (ViewWillEdit != null)
					ViewWillEdit (this);
				// Flag that underlying SimpleCoreTextView is now in edit mode
				textView.IsEditing = true;
			// Become first responder state (which shows software keyboard, if applicable)
				BecomeFirstResponder ();
			} else {
				// Already in editing mode, set insertion point (via selectedTextRange)
				// Find and update insertion point in underlying SimpleCoreTextView
				nint index = textView.ClosestIndex (tap.LocationInView (textView));
				textView.MarkedTextRange = new NSRange (NSRange.NotFound, 0);
				textView.SelectedTextRange = new NSRange (index, 0);
			}
		}
#endregion
		
#region UITextInput methods
		[Export ("inputDelegate")]
		public IUITextInputDelegate InputDelegate {
			get {
				return inputDelegate;
			}
			set {
				inputDelegate = value;
			}
		}
		
		[Export ("markedTextStyle")]
		public NSDictionary MarkedTextStyle {
			get {
				return markedTextStyle;
			}
			set {
				markedTextStyle = value;
			}
		}
		
#region UITextInput - Replacing and Returning Text
		// UITextInput required method - called by text system to get the string for
		// a given range in the text storage
		[Export ("textInRange:")]
		string TextInRange (UITextRange range)
		{
			IndexedRange r = (IndexedRange) range;
	
			return text.ToString ().Substring ((int)r.Range.Location, (int)r.Range.Length);
		}

		// UITextInput required method - called by text system to replace the given
		// text storage range with new text
		[Export ("replaceRange:withText:")]
		void ReplaceRange (UITextRange range, string text)
		{
			IndexedRange r = (IndexedRange) range;
			NSRange selectedNSRange = textView.SelectedTextRange;
			// Determine if replaced range intersects current selection range
			// and update selection range if so.
			if (r.Range.Location + r.Range.Length <= selectedNSRange.Location) {
				// This is the easy case. 
				selectedNSRange.Location -= (r.Range.Length - text.Length);
			} else {
				// Need to also deal with overlapping ranges.  Not addressed
				// in this simplified sample.
			}
			
			// Now replace characters in text storage
			this.text.Remove ((int)r.Range.Location, (int)r.Range.Length);
			this.text.Insert ((int)r.Range.Location, text);
			
			// Update underlying SimpleCoreTextView
			textView.Text = this.text.ToString ();
			textView.SelectedTextRange = selectedNSRange;
		}
#endregion
#region UITextInput - Working with Marked and Selected Text

		// UITextInput selectedTextRange property accessor overrides
		// (access/update underlaying SimpleCoreTextView)
		[Export ("selectedTextRange")]
		IndexedRange SelectedTextRange {
			get {
				return IndexedRange.GetRange (textView.SelectedTextRange);
			}
			set {
				textView.SelectedTextRange = value.Range;
			}
		}

		// UITextInput markedTextRange property accessor overrides
		// (access/update underlaying SimpleCoreTextView)
		[Export ("markedTextRange")]
		IndexedRange MarkedTextRange {
			get {
				return IndexedRange.GetRange (textView.MarkedTextRange);
			}
		}

		// UITextInput required method - Insert the provided text and marks it to indicate 
		// that it is part of an active input session. 
		[Export ("setMarkedText:selectedRange:")]
		void SetMarkedText (string markedText, NSRange selectedRange)
		{
			NSRange selectedNSRange = textView.SelectedTextRange;
			NSRange markedTextRange = textView.MarkedTextRange;
			
			if (markedText == null)
				markedText = string.Empty;
			
			if (markedTextRange.Location != NSRange.NotFound) {
				// Replace characters in text storage and update markedText range length
				text.Remove ((int)markedTextRange.Location, (int)markedTextRange.Length);
				text.Insert ((int)markedTextRange.Location, markedText);
				markedTextRange.Length = markedText.Length;
			} else if (selectedNSRange.Length > 0) {
				// There currently isn't a marked text range, but there is a selected range,
				// so replace text storage at selected range and update markedTextRange.
				text.Remove ((int)selectedNSRange.Location, (int)selectedNSRange.Length);
				text.Insert ((int)selectedNSRange.Location, markedText);
				markedTextRange.Location = selectedNSRange.Location;
				markedTextRange.Length = markedText.Length;
			} else {
				// There currently isn't marked or selected text ranges, so just insert
				// given text into storage and update markedTextRange.
				text.Insert ((int)selectedNSRange.Location, markedText);
				markedTextRange.Location = selectedNSRange.Location;
				markedTextRange.Length = markedText.Length;
		    }
				
				
			// Updated selected text range and underlying SimpleCoreTextView
			selectedNSRange = new NSRange (selectedRange.Location + markedTextRange.Location, selectedRange.Length);
			textView.Text = text.ToString ();
			textView.MarkedTextRange = markedTextRange;
			textView.SelectedTextRange = selectedNSRange;
		}
		
		// UITextInput required method - Unmark the currently marked text.
		[Export ("unmarkText")]
		void UnmarkText () 
		{
		    NSRange markedTextRange = textView.MarkedTextRange;
		    
		    if (markedTextRange.Location == NSRange.NotFound)
		        return;
		
			// unmark the underlying SimpleCoreTextView.markedTextRange
		    markedTextRange.Location = NSRange.NotFound;
		    textView.MarkedTextRange = markedTextRange;    

		}
#endregion
#region UITextInput - Computing Text Ranges and Text Positions

		// UITextInput beginningOfDocument property accessor override
		[Export ("beginningOfDocument")]
		IndexedPosition BeginningOfDocument {
			get { 
				// For this sample, the document always starts at index 0 and is the full
				// length of the text storage
				return IndexedPosition.GetPosition (0);
			}
		}

		// UITextInput endOfDocument property accessor override
		[Export ("endOfDocument")]
		IndexedPosition EndOfDocument {
			get { 
				// For this sample, the document always starts at index 0 and is the full
				// length of the text storage
				return IndexedPosition.GetPosition (text.Length); 
			}
		}

		// UITextInput protocol required method - Return the range between two text positions
		// using our implementation of UITextRange
		[Export ("textRangeFromPosition:toPosition:")]
		IndexedRange GetTextRange (UITextPosition fromPosition, UITextPosition toPosition)
		{
			// Generate IndexedPosition instances that wrap the to and from ranges
			IndexedPosition @from = (IndexedPosition) fromPosition;
			IndexedPosition @to = (IndexedPosition) toPosition;
			NSRange range = new NSRange (Math.Min (@from.Index, @to.Index), Math.Abs (to.Index - @from.Index));
			return IndexedRange.GetRange (range);
		}
		
		// UITextInput protocol required method - Returns the text position at a given offset 
		// from another text position using our implementation of UITextPosition
		[Export ("positionFromPosition:offset:")]
		IndexedPosition GetPosition (UITextPosition position, int offset)
		{
			// Generate IndexedPosition instance, and increment index by offset
			IndexedPosition pos = (IndexedPosition) position;
			int end = pos.Index + offset;
			// Verify position is valid in document
			if (end > text.Length || end < 0)
				return null;
			
			return IndexedPosition.GetPosition (end);
		}

		// UITextInput protocol required method - Returns the text position at a given offset 
		// in a specified direction from another text position using our implementation of
		// UITextPosition.
		[Export ("positionFromPosition:inDirection:offset:")]
		IndexedPosition GetPosition (UITextPosition position, UITextLayoutDirection direction, int offset)
		{
			// Note that this sample assumes LTR text direction
			IndexedPosition pos = (IndexedPosition) position;
			int newPos = pos.Index;
			
			switch (direction) {
			case UITextLayoutDirection.Right:
				newPos += offset;
				break;
			case UITextLayoutDirection.Left:
				newPos -= offset;
				break;
			case UITextLayoutDirection.Up:
			case UITextLayoutDirection.Down:
				// This sample does not support vertical text directions
				break;
			}
			
			// Verify new position valid in document
			
			if (newPos < 0)
				newPos = 0;
			
			if (newPos > text.Length)
				newPos = text.Length;
			
			return IndexedPosition.GetPosition (newPos);
		}
#endregion
#region UITextInput - Evaluating Text Positions

		// UITextInput protocol required method - Return how one text position compares to another 
		// text position.  
		[Export ("comparePosition:toPosition:")]
		NSComparisonResult ComparePosition (UITextPosition position, UITextPosition other)
		{
			IndexedPosition pos = (IndexedPosition) position;
			IndexedPosition o = (IndexedPosition) other;
			
			// For this sample, we simply compare position index values
			if (pos.Index == o.Index) {
				return NSComparisonResult.Same;
			} else if (pos.Index < o.Index) {
				return NSComparisonResult.Ascending;
			} else {
				return NSComparisonResult.Descending;
			}
		}
		
		// UITextInput protocol required method - Return the number of visible characters 
		// between one text position and another text position.
		[Export ("offsetFromPosition:toPosition:")]
		int GetOffset (IndexedPosition @from, IndexedPosition toPosition)
		{
			return @from.Index - toPosition.Index;
		}
#endregion
#region UITextInput - Text Input Delegate and Text Input Tokenizer

		// UITextInput tokenizer property accessor override
		//
		// An input tokenizer is an object that provides information about the granularity 
		// of text units by implementing the UITextInputTokenizer protocol.  Standard units 
		// of granularity include characters, words, lines, and paragraphs. In most cases, 
		// you may lazily create and assign an instance of a subclass of 
		// UITextInputStringTokenizer for this purpose, as this sample does. If you require 
		// different behavior than this system-provided tokenizer, you can create a custom 
		// tokenizer that adopts the UITextInputTokenizer protocol.
		[Export ("tokenizer")]
		UITextInputTokenizer Tokenizer {
			get { 
				return tokenizer; 
			}
		}
#endregion
#region UITextInput - Text Layout, writing direction and position related methods
		// UITextInput protocol method - Return the text position that is at the farthest 
		// extent in a given layout direction within a range of text.
		[Export ("positionWithinRange:farthestInDirection:")]
		IndexedPosition GetPosition (UITextRange range, UITextLayoutDirection direction)
		{
			// Note that this sample assumes LTR text direction
			IndexedRange r = (IndexedRange) range;
			nint pos = r.Range.Location;
			
			// For this sample, we just return the extent of the given range if the
			// given direction is "forward" in a LTR context (UITextLayoutDirectionRight
			// or UITextLayoutDirectionDown), otherwise we return just the range position
			switch (direction) {
			case UITextLayoutDirection.Up:
			case UITextLayoutDirection.Left:
				pos = r.Range.Location;
				break;
			case UITextLayoutDirection.Right:
			case UITextLayoutDirection.Down:
				pos = r.Range.Location + r.Range.Length;
				break;
			}
			
			// Return text position using our UITextPosition implementation.
			// Note that position is not currently checked against document range.
			return IndexedPosition.GetPosition ((int)pos);
		}

		// UITextInput protocol required method - Return a text range from a given text position 
		// to its farthest extent in a certain direction of layout.
		[Export ("characterRangeByExtendingPosition:inDirection:")]
		IndexedRange GetCharacterRange (UITextPosition position, UITextLayoutDirection direction)
		{
			// Note that this sample assumes LTR text direction
			IndexedPosition pos = (IndexedPosition) position;
			NSRange result = new NSRange (pos.Index, 1);
			
			switch (direction) {
			case UITextLayoutDirection.Up:
			case UITextLayoutDirection.Left:
				result = new NSRange (pos.Index - 1, 1);
				break;
			case UITextLayoutDirection.Right:
			case UITextLayoutDirection.Down:
				result = new NSRange (pos.Index, 1);
				break;
			}
			
			// Return range using our UITextRange implementation
			// Note that range is not currently checked against document range.
			return IndexedRange.GetRange (result);
		}

		// UITextInput protocol required method - Return the base writing direction for 
		// a position in the text going in a specified text direction.
		[Export ("baseWritingDirectionForPosition:inDirection:")]
		UITextWritingDirection GetBaseWritingDirection (UITextPosition position, UITextStorageDirection direction)
		{
			return UITextWritingDirection.LeftToRight;
		}

		// UITextInput protocol required method - Set the base writing direction for a 
		// given range of text in a document.
		[Export ("setBaseWritingDirection:forRange:")]
		void SetBaseWritingDirection (UITextWritingDirection writingDirection, UITextRange range)
		{
			// This sample assumes LTR text direction and does not currently support BiDi or RTL.
		}
#endregion
#region UITextInput - Geometry methods
		// UITextInput protocol required method - Return the first rectangle that encloses 
		// a range of text in a document.
		[Export ("firstRectForRange:")]
		CGRect FirstRect (UITextRange range)
		{
			// FIXME: the Objective-C code doesn't get a null range
			// This is the reason why we don't get the autocorrection suggestions
			// (it'll just autocorrect without showing any suggestions).
			// Possibly due to http://bugzilla.xamarin.com/show_bug.cgi?id=265
			IndexedRange r = (IndexedRange) (range ?? IndexedRange.GetRange (new NSRange (0, 1)));
			// Use underlying SimpleCoreTextView to get rect for range
			CGRect rect = textView.FirstRect (r.Range);
			// Convert rect to our view coordinates
			return ConvertRectFromView (rect, textView);
		}
		
		// UITextInput protocol required method - Return a rectangle used to draw the caret
		// at a given insertion point.
		[Export ("caretRectForPosition:")]
		CGRect CaretRect (UITextPosition position)
		{
			// FIXME: the Objective-C code doesn't get a null position
			// This is the reason why we don't get the autocorrection suggestions
			// (it'll just autocorrect without showing any suggestions).
			// Possibly due to http://bugzilla.xamarin.com/show_bug.cgi?id=265
			IndexedPosition pos = (IndexedPosition) (position ?? IndexedPosition.GetPosition (0));
			// Get caret rect from underlying SimpleCoreTextView
			CGRect rect = textView.CaretRect (pos.Index);
			// Convert rect to our view coordinates
			return ConvertRectFromView (rect, textView);
		}
#endregion
#region UITextInput - Hit testing
		// Note that for this sample hit testing methods are not implemented, as there
		// is no implemented mechanic for letting user select text via touches.  There
		// is a wide variety of approaches for this (gestures, drag rects, etc) and
		// any approach chosen will depend greatly on the design of the application.
		
		// UITextInput protocol required method - Return the position in a document that 
		// is closest to a specified point. 
		[Export ("closestPositionToPoint:")]
		UITextPosition ClosestPosition (CGPoint point)
		{
			// Not implemented in this sample.  Could utilize underlying 
			// SimpleCoreTextView:closestIndexToPoint:point
			return null;
		}

		// UITextInput protocol required method - Return the position in a document that 
		// is closest to a specified point in a given range.
		[Export ("closestPositionToPoint:withinRange:")]
		UITextPosition ClosestPosition (CGPoint point, UITextRange range)
		{
			// Not implemented in this sample.  Could utilize underlying 
			// SimpleCoreTextView:closestIndexToPoint:point
			return null;
		}

		// UITextInput protocol required method - Return the character or range of 
		// characters that is at a given point in a document.
		[Export ("characterRangeAtPoint:")]
		UITextRange CharacterRange (CGPoint point)
		{
			// Not implemented in this sample.  Could utilize underlying 
			// SimpleCoreTextView:closestIndexToPoint:point
			return null;
		}
#endregion
#region UITextInput - Returning Text Styling Information
		// UITextInput protocol method - Return a dictionary with properties that specify 
		// how text is to be style at a certain location in a document.
		[Export ("textStylingAtPosition:inDirection:")]
		NSDictionary TextStyling (UITextPosition position, UITextStorageDirection direction)
		{
			// This sample assumes all text is single-styled, so this is easy.
			return NSDictionary.FromObjectAndKey (textView.Font, CTStringAttributeKey.Font);
		}
#endregion
#region UIKeyInput methods
		// UIKeyInput required method - A Boolean value that indicates whether the text-entry 
		// objects have any text.
		[Export ("hasText")]
		bool HasText {
			get { return text.Length > 0; }
		}
		
		// UIKeyInput required method - Insert a character into the displayed text.
		// Called by the text system when the user has entered simple text
		[Export ("insertText:")]
		void InsertText (string text)
		{
			NSRange selectedNSRange = textView.SelectedTextRange;
			NSRange markedTextRange = textView.MarkedTextRange;
			
			// Note: While this sample does not provide a way for the user to
			// create marked or selected text, the following code still checks for
			// these ranges and acts accordingly.
			if (markedTextRange.Location != NSRange.NotFound) {
				// There is marked text -- replace marked text with user-entered text
				this.text.Remove ((int)markedTextRange.Location, (int)markedTextRange.Length);
				this.text.Insert ((int)markedTextRange.Location, text);
				selectedNSRange.Location = markedTextRange.Location + text.Length;
				selectedNSRange.Length = 0;
				markedTextRange = new NSRange (NSRange.NotFound, 0);
			} else if (selectedNSRange.Length > 0) {
				// Replace selected text with user-entered text
				this.text.Remove ((int)selectedNSRange.Location, (int)selectedNSRange.Length);
				this.text.Insert ((int)selectedNSRange.Location, text);
				selectedNSRange.Length = 0;
				selectedNSRange.Location += text.Length;
			} else {
				// Insert user-entered text at current insertion point
				this.text.Insert ((int)selectedNSRange.Location, text);
				selectedNSRange.Location += text.Length;
			}
			
			// Update underlying SimpleCoreTextView
			textView.Text = this.text.ToString ();
			textView.MarkedTextRange = markedTextRange;
			textView.SelectedTextRange = selectedNSRange;
		}

		// UIKeyInput required method - Delete a character from the displayed text.
		// Called by the text system when the user is invoking a delete (e.g. pressing
		// the delete software keyboard key)
		[Export ("deleteBackward")]
		void DeleteBackward ()
		{
			NSRange selectedNSRange = textView.SelectedTextRange;
			NSRange markedTextRange = textView.MarkedTextRange;
			
			// Note: While this sample does not provide a way for the user to
			// create marked or selected text, the following code still checks for
			// these ranges and acts accordingly.
			if (markedTextRange.Location != NSRange.NotFound) {
				// There is marked text, so delete it
				text.Remove ((int)markedTextRange.Location, (int)markedTextRange.Length);
				selectedNSRange.Location = markedTextRange.Location;
				selectedNSRange.Length = 0;
				markedTextRange = new NSRange (NSRange.NotFound, 0);
			} else if (selectedNSRange.Length > 0) {
				// Delete the selected text
				text.Remove ((int)selectedNSRange.Location, (int)selectedNSRange.Length);
				selectedNSRange.Length = 0;
			} else if (selectedNSRange.Location > 0) {
				// Delete one char of text at the current insertion point
				selectedNSRange.Location--;
				selectedNSRange.Length = 1;
				text.Remove ((int)selectedNSRange.Location, (int)selectedNSRange.Length);
				selectedNSRange.Length = 0;
			}
			
			// Update underlying SimpleCoreTextView
			textView.Text = text.ToString ();
			textView.MarkedTextRange = markedTextRange;
			textView.SelectedTextRange = selectedNSRange;
		}
	}
#endregion
#endregion
}
