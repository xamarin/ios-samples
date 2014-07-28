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
	public class SimpleCoreTextView : UIView
	{
		string text;
		UIFont font;
		bool is_editing;
		NSRange markedTextRange;
		NSRange selectedTextRange;
		
		CTStringAttributes attributes;
		CTFramesetter framesetter;
		CTFrame frame;
		SimpleCaretView caretView;
		
		// Note that for this sample for simplicity, the selection color and
		// insertion point "caret" color are fixed and cannot be changed.
		public static UIColor SelectionColor = new UIColor (0.25f, 0.5f, 1.0f, 0.5f);
		public static UIColor CaretColor = new UIColor (0.25f, 0.5f, 1.0f, 1.0f);
		
		public SimpleCoreTextView (CGRect frame)
			: base (frame)
		{
			Layer.GeometryFlipped = true;  // For ease of interaction with the CoreText coordinate system.
			attributes = new CTStringAttributes ();
			Text = string.Empty;
			Font = UIFont.SystemFontOfSize (18);
			BackgroundColor = UIColor.Clear;
			caretView = new SimpleCaretView (CGRect.Empty);
		}
		
		protected override void Dispose (bool disposing)
		{
			Font = null;
			attributes = null;
			caretView = null;
			base.Dispose (disposing);
		}
		
		void ClearPreviousLayoutInformation ()
		{
			if (framesetter != null) {
				framesetter.Dispose ();
				framesetter = null;
			}
			
			if (frame != null) {
				frame.Dispose ();
				frame = null;
			}
		}
		
		public UIFont Font {
			get {
				return font;
			}
			set {
				font = value;
				attributes.Font = new CTFont (font.Name, font.PointSize);
				TextChanged ();
			}
		}
		
		NSAttributedString attributedString;
		void TextChanged ()
		{
			SetNeedsDisplay ();
			ClearPreviousLayoutInformation ();
			
			attributedString = new NSAttributedString (Text, attributes);
			framesetter = new CTFramesetter (attributedString);
			
			UIBezierPath path = UIBezierPath.FromRect (Bounds);
			frame = framesetter.GetFrame (new NSRange (0, 0), path.CGPath, null);
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				TextChanged ();
			}
		}
		
		// Helper method for obtaining the intersection of two ranges (for handling
		// selection range across multiple line ranges in drawRangeAsSelection below)
		NSRange RangeIntersection (NSRange first, NSRange second)
		{
			NSRange result = new NSRange (NSRange.NotFound, 0);
			
			// Ensure first range does not start after second range
			if (first.Location > second.Location) {
				NSRange tmp = first;
				first = second;
				second = tmp;
			}
			
			// Find the overlap intersection range between first and second
			if (second.Location < first.Location + first.Length) {
				result.Location = second.Location;
				nint end = (nint) Math.Min (first.Location + first.Length, second.Location + second.Length);
				result.Length = end - result.Location;
			}
			
			return result;
		}
		
		// Helper method for drawing the current selection range (as a simple filled rect)
		void DrawRangeAsSelection (NSRange selectionRange)
		{
			
			// If not in editing mode, we do not draw selection rects
			if (!IsEditing)
				return;
					
			// If selection range empty, do not draw
			if (selectionRange.Length == 0 || selectionRange.Location == NSRange.NotFound)
				return;
    
			// set the fill color to the selection color
			SelectionColor.SetFill ();
    
			// Iterate over the lines in our CTFrame, looking for lines that intersect
			// with the given selection range, and draw a selection rect for each intersection
			var lines = frame.GetLines ();

			for (int i = 0; i < lines.Length; i++) {
				CTLine line = lines [i];
				NSRange lineRange = line.StringRange;
				NSRange range = new NSRange (lineRange.Location, lineRange.Length);
				NSRange intersection = RangeIntersection (range, selectionRange);
				if (intersection.Location != NSRange.NotFound && intersection.Length > 0) {
				// The text range for this line intersects our selection range
					nfloat xStart = line.GetOffsetForStringIndex (intersection.Location);
					nfloat xEnd = line.GetOffsetForStringIndex (intersection.Location + intersection.Length);
					CGPoint [] origin = new CGPoint [lines.Length];
					// Get coordinate and bounds information for the intersection text range
					frame.GetLineOrigins (new NSRange (i, 0), origin);
					nfloat ascent, descent, leading;
					line.GetTypographicBounds (out ascent, out descent, out leading);
					// Create a rect for the intersection and draw it with selection color
					CGRect selectionRect = new CGRect (xStart, origin [0].Y - descent, xEnd - xStart, ascent + descent);
					UIGraphics.RectFill (selectionRect);
				}
			}
		}
		
		// Standard UIView drawRect override that uses Core Text to draw our text contents
		public override void Draw (CGRect rect)
		{
			// First draw selection / marked text, then draw text
			DrawRangeAsSelection (SelectedTextRange);
			DrawRangeAsSelection (MarkedTextRange);
			frame.Draw (UIGraphics.GetCurrentContext ());
		}
		
		// Public method to find the text range index for a given CGPoint
		public nint ClosestIndex (CGPoint point)
		{
			// Use Core Text to find the text index for a given CGPoint by
			// iterating over the y-origin points for each line, finding the closest
			// line, and finding the closest index within that line.
			var lines = frame.GetLines ();
			CGPoint[] origins = new CGPoint [lines.Length];
			frame.GetLineOrigins (new NSRange (0, lines.Length), origins);
			
			for (int i = 0; i < lines.Length; i++) {
				if (point.Y > origins [i].Y) {
					// This line origin is closest to the y-coordinate of our point,
					// now look for the closest string index in this line.
					return lines [i].GetStringIndexForPosition (point);
				}
			}
			
			return text.Length;
		}
		
		// Public method to determine the CGRect for the insertion point or selection, used
		// when creating or updating our SimpleCaretView instance
		public CGRect CaretRect (nint index)
		{
			var lines = frame.GetLines ();

			// Special case, no text
			if (text.Length == 0) {
				CGPoint origin = new CGPoint (Bounds.GetMinX (), Bounds.GetMinY () - font.Leading);
				// Note: using fabs() for typically negative descender from fonts
				return new CGRect (origin.X, origin.Y - (nfloat)Math.Abs (font.Descender), 3, font.Ascender + (nfloat)Math.Abs (font.Descender));
			}
			
			// Special case, insertion point at final position in text after newline
			if (index == text.Length && text.EndsWith ("\n")) {
				CTLine line = lines [lines.Length - 1];
				NSRange range = line.StringRange;
				nfloat xPos = line.GetOffsetForStringIndex (range.Location);
				CGPoint[] origins = new CGPoint [lines.Length];
				nfloat ascent, descent, leading;
				line.GetTypographicBounds (out ascent, out descent, out leading);
				frame.GetLineOrigins (new NSRange (lines.Length - 1, 0), origins);
				// Place point after last line, including any font leading spacing if applicable
				origins[0].Y -= font.Leading;
				return new CGRect (xPos, origins [0].Y - descent, 3, ascent + descent);        
			}
			
			// Regular case, caret somewhere within our text content range
			for (int i = 0; i < lines.Length; i++) {
				CTLine line = lines [i];
				NSRange range = line.StringRange;
				nint localIndex = index - range.Location;
				if (localIndex >= 0 && localIndex <= range.Length) {
					// index is in the range for this line
					nfloat xPos = line.GetOffsetForStringIndex (index);
					CGPoint[] origins = new CGPoint [lines.Length];
					nfloat ascent, descent, leading;
					line.GetTypographicBounds (out ascent, out descent, out leading);
					frame.GetLineOrigins (new NSRange (i, 0), origins);
					// Make a small "caret" rect at the index position
					return new CGRect (xPos, origins [0].Y - descent, 3, ascent + descent);
				}
			}
			
			return CGRect.Empty;
		}
		
		// Public method to create a rect for a given range in the text contents
		// Called by our EditableTextRange to implement the required 
		// UITextInput:firstRectForRange method
		public CGRect FirstRect (NSRange range)
		{    
			nint index = range.Location;
			
			// Iterate over our CTLines, looking for the line that encompasses the given range
			var lines = frame.GetLines ();
			for (int i = 0; i < lines.Length; i++) {
				CTLine line = lines [i];
				NSRange lineRange = line.StringRange;
				nint localIndex = index - lineRange.Location;
				if (localIndex >= 0 && localIndex < lineRange.Length) {
					// For this sample, we use just the first line that intersects range
					nint finalIndex = (nint) (Math.Min (lineRange.Location + lineRange.Length, range.Location + range.Length));
					// Create a rect for the given range within this line
					nfloat xStart = line.GetOffsetForStringIndex (index);
					nfloat xEnd = line.GetOffsetForStringIndex (finalIndex);
					CGPoint[] origins = new CGPoint [lines.Length];
					frame.GetLineOrigins (new NSRange (i, 0), origins);
					nfloat ascent, descent, leading;
					line.GetTypographicBounds (out ascent, out descent, out leading);
					
					return new CGRect (xStart, origins [0].Y - descent, xEnd - xStart, ascent + descent);
				}
			}
			
			return CGRect.Empty;
		}
		
		// Helper method to update caretView when insertion point/selection changes
		void SelectionChanged ()
		{
			// If not in editing mode, we don't show the caret
			if (!IsEditing) {
				caretView.RemoveFromSuperview ();
				return;
			}
			
			// If there is no selection range (always true for this sample), find the
			// insert point rect and create a caretView to draw the caret at this position
			if (SelectedTextRange.Length == 0) {
				caretView.Frame = CaretRect (SelectedTextRange.Location);
				if (caretView.Superview == null) {
					AddSubview (caretView);
					SetNeedsDisplay ();
				}
				// Set up a timer to "blink" the caret
				caretView.DelayBlink ();
			} else {
				// If there is an actual selection, don't draw the insertion caret too
				caretView.RemoveFromSuperview ();
				SetNeedsDisplay ();
			}    
			
			if (MarkedTextRange.Location != NSRange.NotFound) {
				SetNeedsDisplay ();
			}
		}
		
		// markedTextRange property accessor overrides 
		public NSRange MarkedTextRange {
			get {
				return markedTextRange;
			}
			set {
				markedTextRange = value;
				// Call selectionChanged to update view if necessary
				SelectionChanged ();
			}
		}

		// selectedTextRange property accessor overrides
		public NSRange SelectedTextRange {
			get {
				return selectedTextRange;
			}
			set {
				selectedTextRange = value;
				// Call selectionChanged to update view if necessary
				SelectionChanged ();
			}
		}
		
		// editing property accessor overrides
		public bool IsEditing {
			get {
				return is_editing; 
			}
			set {
				is_editing = value;
				SelectionChanged ();
			}
		}
	}
	
	class SimpleCaretView : UIView
	{
		NSTimer blink_timer;
		const double InitialBlinkDelay = 0.7;
		const double BlinkRate = 0.5;
		
		public SimpleCaretView (CGRect frame)
			: base (frame)
		{
			BackgroundColor = SimpleCoreTextView.CaretColor;
		}
		
		// Helper method to toggle hidden state of caret view
		public void Blink ()
		{
			Hidden = !Hidden;
		}
		
		public void DelayBlink ()
		{
			Hidden = false;
			blink_timer.FireDate = NSDate.FromTimeIntervalSinceNow (InitialBlinkDelay);
		}
		
		// UIView didMoveToSuperview override to set up blink timers after caret view created in superview
		public override void MovedToSuperview ()
		{
			Hidden = false;
			
			if (Superview != null) {
				blink_timer = NSTimer.CreateRepeatingScheduledTimer (BlinkRate, (a) => Blink ());
				DelayBlink ();
			} else {
				blink_timer.Invalidate ();
				blink_timer.Dispose ();
				blink_timer = null;
			}
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}
	}
}

