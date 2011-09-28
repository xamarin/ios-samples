using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;

namespace SimpleTextInput
{
	// We use a tap gesture recognizer to allow the user to tap to invoke text edit mode
	// @interface EditableCoreTextView () <UIGestureRecognizerDelegate>
	class TapGestureRecognizerDelegate : UIGestureRecognizerDelegate
	{
		EditableCoreTextView parent;
	
		public TapGestureRecognizerDelegate (EditableCoreTextView parent)
		{
			this.parent = parent;
		}
		
		// UIGestureRecognizerDelegate method - called to determine if we want to handle
		// a given gesture
		public override bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
		{
			// If gesture touch occurs in our view, we want to handle it
			return touch.View == parent;
		}
	}
}
