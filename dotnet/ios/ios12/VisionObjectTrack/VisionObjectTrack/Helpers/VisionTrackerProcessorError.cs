
namespace VisionObjectTrack;

public class VisionTrackerProcessorError : NSError
{
	public VisionTrackerProcessorError (VisionTrackerProcessorErrorType type)
	{
		Type = type;
	}

	public VisionTrackerProcessorErrorType Type { get; private set; }
}
