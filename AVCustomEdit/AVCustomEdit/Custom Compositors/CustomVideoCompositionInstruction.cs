using AVFoundation;
using CoreMedia;
using Foundation;

namespace AVCustomEdit
{
    public class CustomVideoCompositionInstruction : AVVideoCompositionInstruction
    {
        private readonly NSNumber[] requiredSourceTrackIds;
        private readonly bool enablePostProcessing;
        private readonly int passthroughTrackId;
        private readonly bool containsTweening;
        private readonly CMTimeRange timeRange;

        public CustomVideoCompositionInstruction(int passthroughTrackId, CMTimeRange timeRange) : base()
        {
            this.passthroughTrackId = passthroughTrackId;
            this.requiredSourceTrackIds = null;
            this.timeRange = timeRange;
            this.containsTweening = false;
            this.enablePostProcessing = false;
        }

        public CustomVideoCompositionInstruction(NSNumber[] sourceTracksIds, CMTimeRange timeRange) : base()
        {
            this.requiredSourceTrackIds = sourceTracksIds;
            this.passthroughTrackId = 0;
            this.timeRange = timeRange;
            this.containsTweening = true;
            this.enablePostProcessing = false;
        }

        public int ForegroundTrackId { get; set; }

        public int BackgroundTrackId { get; set; }

        public override int PassthroughTrackID => this.passthroughTrackId;

        public override NSNumber[] RequiredSourceTrackIDs => this.requiredSourceTrackIds;

        public override CMTimeRange TimeRange => this.timeRange;

        public override bool EnablePostProcessing => this.enablePostProcessing;

        public override bool ContainsTweening => this.containsTweening;
    }
}