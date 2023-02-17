using System;
using Foundation;
using UIKit;
namespace ScanningAndDetecting3DObjects {
	internal class Snapping {
		internal static void PlayHapticFeedback ()
		{
			var feedbackGenerator = new UIImpactFeedbackGenerator (UIImpactFeedbackStyle.Light);
			feedbackGenerator.ImpactOccurred ();
		}
	}
}
