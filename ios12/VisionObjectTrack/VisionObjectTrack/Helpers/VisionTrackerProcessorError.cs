
namespace VisionObjectTrack {
	using Foundation;
	using VisionObjectTrack.Enums;

	public class VisionTrackerProcessorError : NSError {
		public VisionTrackerProcessorError (VisionTrackerProcessorErrorType type)
		{
			this.Type = type;
		}

		public VisionTrackerProcessorErrorType Type { get; private set; }
	}
}
