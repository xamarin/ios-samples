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
	// A UITextPosition object represents a position in a text container; in other words, it is 
	// an index into the backing string in a text-displaying view.
	// 
	// Classes that adopt the UITextInput protocol must create custom UITextPosition objects 
	// for representing specific locations within the text managed by the class. The text input 
	// system uses both these objects and UITextRange objects for communicating text-layout information.
	//
	// We could use more sophisticated objects, but for demonstration purposes it suffices to wrap integers.
	class IndexedPosition : UITextPosition
	{
		public int Index { get; private set; }
		
		private IndexedPosition (int index)
		{
			this.Index = index;
		}
		
		// We need to keep all the IndexedPositions we create accessible from managed code,
		// otherwise the garbage collector will collect them since it won't know that native
		// code has references to them.
		static Dictionary<int, IndexedPosition> indexes = new Dictionary<int, IndexedPosition> ();
		public static IndexedPosition GetPosition (int index)
		{
			IndexedPosition result;
			
			if (!indexes.TryGetValue (index, out result)) {
				result = new IndexedPosition (index);
				indexes [index] = result;
			}
			return result;
		}
	}
}

