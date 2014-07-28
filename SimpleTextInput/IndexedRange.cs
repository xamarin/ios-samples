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
	// A UITextRange object represents a range of characters in a text container; in other words,
	// it identifies a starting index and an ending index in string backing a text-entry object.
	//
	// Classes that adopt the UITextInput protocol must create custom UITextRange objects for 
	// representing ranges within the text managed by the class. The starting and ending indexes 
	// of the range are represented by UITextPosition objects. The text system uses both UITextRange 
	// and UITextPosition objects for communicating text-layout information.
	class IndexedRange : UITextRange 
	{
		public NSRange Range { get; private set; }
		
		private IndexedRange ()
		{
		}
		
		public override UITextPosition start {
			get {
				return IndexedPosition.GetPosition ((int)Range.Location);
			}
		}
		
		public override UITextPosition end {
			get {
				return IndexedPosition.GetPosition ((int)(Range.Location + Range.Length));
			}
		}
		
		public override bool IsEmpty {
			get {
				return Range.Length == 0;
			}
		}
		
		// We need to keep all the IndexedRanges we create accessible from managed code,
		// otherwise the garbage collector will collect them since it won't know that native
		// code has references to them.
		private static Dictionary<NSRange, IndexedRange> ranges = new Dictionary<NSRange, IndexedRange> (new NSRangeEqualityComparer ());
		public static IndexedRange GetRange (NSRange theRange)
		{
			IndexedRange result;
			
			if (theRange.Location == NSRange.NotFound)
				return null;
			
			if (!ranges.TryGetValue (theRange, out result)) {
				result = new IndexedRange ();
				result.Range = new NSRange (theRange.Location, theRange.Length);
			}
			return result;
		}
		
		class NSRangeEqualityComparer : IEqualityComparer<NSRange>
		{
			#region IEqualityComparer[NSRange] implementation
			public bool Equals (NSRange x, NSRange y)
			{
				return x.Length == y.Length && x.Location == y.Location;
			}

			public int GetHashCode (NSRange obj)
			{
				return obj.Location.GetHashCode () ^ obj.Length.GetHashCode ();
			}
			#endregion
		}
		
	}
}
