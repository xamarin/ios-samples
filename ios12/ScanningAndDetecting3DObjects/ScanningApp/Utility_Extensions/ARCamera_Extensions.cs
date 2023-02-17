using ARKit;

namespace ScanningAndDetecting3DObjects {
	internal static class ARCamera_Extensions {
		internal static string PresentationString (this ARCamera self)
		{
			var tracking = self.TrackingState;
			switch (tracking) {
			case ARTrackingState.NotAvailable: return "ARKit tracking UNAVAILABLE";
			case ARTrackingState.Normal: return "ARKit tracking NORMAL";
			case ARTrackingState.Limited:
				switch (self.TrackingStateReason) {
				case ARTrackingStateReason.ExcessiveMotion: return "ARKit tracking LIMITED : Excessive motion";
				case ARTrackingStateReason.InsufficientFeatures: return "ARKit tracking LIMITED : Low detail";
				case ARTrackingStateReason.Initializing: return "ARKit is initializing";
				case ARTrackingStateReason.Relocalizing: return "ARKit is relocalizing";
				}
				break;
			}
			// Can't actually get here
			return "";
		}

		internal static string Recommendation (this ARCamera self)
		{
			if (self.TrackingState == ARTrackingState.Limited) {
				switch (self.TrackingStateReason) {
				case ARTrackingStateReason.ExcessiveMotion: return "Try slowing down your movement or reset the session.";
				case ARTrackingStateReason.InsufficientFeatures: return "Try pointing at a flat surface or reset the session.";
				case ARTrackingStateReason.Initializing: return "Try moving left or right or reset the session.";
				case ARTrackingStateReason.Relocalizing: return "Try returning to the location where you left off.";
				}
			}
			return null;
		}
	}
}
