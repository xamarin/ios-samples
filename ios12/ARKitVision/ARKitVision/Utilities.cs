
namespace ARKitVision {
	using ARKit;
	using ImageIO;
	using UIKit;

	public static class ARTrackingStateExtensions {
		public static string GetPresentationString (this ARCamera self)
		{
			string result = null;

			switch (self.TrackingState) {
			case ARTrackingState.NotAvailable:
				result = "TRACKING UNAVAILABLE";
				break;

			case ARTrackingState.Normal:
				result = "TRACKING NORMAL";
				break;

			case ARTrackingState.Limited:
				switch (self.TrackingStateReason) {
				case ARTrackingStateReason.ExcessiveMotion:
					result = "TRACKING LIMITED\nExcessive motion";
					break;
				case ARTrackingStateReason.InsufficientFeatures:
					result = "TRACKING LIMITED\nLow detail";
					break;
				case ARTrackingStateReason.Initializing:
					result = "Initializing";
					break;
				case ARTrackingStateReason.Relocalizing:
					result = "Recovering from interruption";
					break;
				}
				break;
			}

			return result;
		}

		public static string GetRecommendation (this ARCamera self)
		{
			string result = null;

			switch (self.TrackingState) {
			case ARTrackingState.Limited:
				switch (self.TrackingStateReason) {
				case ARTrackingStateReason.ExcessiveMotion:
					result = "Try slowing down your movement, or reset the session.";
					break;
				case ARTrackingStateReason.InsufficientFeatures:
					result = "Try pointing at a flat surface, or reset the session.";
					break;
				case ARTrackingStateReason.Relocalizing:
					result = "Return to the location where you left off or try resetting the session.";
					break;
				}
				break;
			}

			return result;
		}
	}

	/// <summary>
	/// Utility functions and type extensions used throughout the projects.
	/// </summary>
	public static class CGImagePropertyOrientationExtensions {
		/// <summary>
		/// Convert device orientation to image orientation for use by Vision analysis.
		/// </summary>
		public static CGImagePropertyOrientation ConvertFrom (UIDeviceOrientation deviceOrientation)
		{
			CGImagePropertyOrientation result;
			switch (deviceOrientation) {
			case UIDeviceOrientation.PortraitUpsideDown:
				result = CGImagePropertyOrientation.Left;
				break;

			case UIDeviceOrientation.LandscapeLeft:
				result = CGImagePropertyOrientation.Up;
				break;

			case UIDeviceOrientation.LandscapeRight:
				result = CGImagePropertyOrientation.Down;
				break;

			default:
				result = CGImagePropertyOrientation.Right;
				break;
			}

			return result;
		}
	}
}
