using AVFoundation;
using CoreMedia;
using Foundation;

namespace AVCustomEdit
{
    public class CustomVideoCompositionInstruction : AVVideoCompositionInstruction
    {
        private readonly NSNumber[] requiredSourceTrackIDs;
        private readonly bool enablePostProcessing;
        private readonly int passthroughTrackID;
        private readonly bool containsTweening;
        private readonly CMTimeRange timeRange;

        public CustomVideoCompositionInstruction(int passthroughTrackId, CMTimeRange timeRange)
        {
            this.passthroughTrackID = passthroughTrackId;
            this.requiredSourceTrackIDs = null;
            this.timeRange = timeRange;
            this.containsTweening = false;
            this.enablePostProcessing = false;
        }

        public CustomVideoCompositionInstruction(NSNumber[] sourceTracksIDS, CMTimeRange timeRange)
        {
            this.requiredSourceTrackIDs = sourceTracksIDS;
            this.passthroughTrackID = 0;
            this.timeRange = timeRange;
            this.containsTweening = true;
            this.enablePostProcessing = false;
        }

        public int ForegroundTrackId { get; set; }

        public int BackgroundTrackId { get; set; }

        public override int PassthroughTrackID => this.passthroughTrackID;

        public override NSNumber[] RequiredSourceTrackIDs => this.requiredSourceTrackIDs;

        public override CMTimeRange TimeRange => this.timeRange;

        public override bool EnablePostProcessing => this.enablePostProcessing;

        public override bool ContainsTweening => this.containsTweening;
    }
}