using AVFoundation;
using CoreMedia;
using Foundation;

namespace AVCustomEdit
{
	public class CustomVideoCompositionInstruction : AVVideoCompositionInstruction
	{
		public int ForegroundTrackID;
		public int BackgroundTrackID;

		CMTimeRange timeRange;
		readonly int passthroughTrackID;
		readonly NSNumber [] requiredSourceTrackIDs;
		readonly bool enablePostProcessing;
		readonly bool containsTweening;

		public override int PassthroughTrackID {
			get {
				return passthroughTrackID;
			}
		}

		public override NSNumber [] RequiredSourceTrackIDs {
			get {
				return requiredSourceTrackIDs;
			}
		}

		public override CMTimeRange TimeRange {
			get {
				return timeRange;
			}
		}
		public override bool EnablePostProcessing {
			get {
				return enablePostProcessing;
			}
		}
		public override bool ContainsTweening {
			get {
				return containsTweening;
			}
		}

		public CustomVideoCompositionInstruction ()
		{
		}

		public CustomVideoCompositionInstruction (int passthroughTrackID, CMTimeRange timeRange)
		{
			this.passthroughTrackID = passthroughTrackID;
			requiredSourceTrackIDs = null;
			this.timeRange = timeRange;
			containsTweening = false;
			enablePostProcessing = false;
		}

		public CustomVideoCompositionInstruction(NSNumber [] sourceTracksIDS, CMTimeRange timeRange)
		{
			requiredSourceTrackIDs = sourceTracksIDS;
			passthroughTrackID = 0;
			this.timeRange = timeRange;
			containsTweening = true;
			enablePostProcessing = false;
		}
	}
}