using AVFoundation;
using CoreMedia;
using Foundation;
using ObjCRuntime;

namespace AVCustomEdit {
	public class CustomVideoCompositionInstruction : AVVideoCompositionInstruction {
		private readonly NSNumber [] requiredSourceTrackIds;
		private readonly bool enablePostProcessing;
		private readonly int passthroughTrackId;
		private readonly bool containsTweening;
		private readonly CMTimeRange timeRange;

		public CustomVideoCompositionInstruction () : base () { }

		public CustomVideoCompositionInstruction (int passthroughTrackId, CMTimeRange timeRange) : base ()
		{
			this.passthroughTrackId = passthroughTrackId;
			this.requiredSourceTrackIds = null;
			this.timeRange = timeRange;
			this.containsTweening = false;
			this.enablePostProcessing = false;
		}

		public CustomVideoCompositionInstruction (NSNumber [] sourceTracksIds, CMTimeRange timeRange) : base ()
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

		public override NSNumber [] RequiredSourceTrackIDs => this.requiredSourceTrackIds;

		public override CMTimeRange TimeRange => this.timeRange;

		public override bool EnablePostProcessing => this.enablePostProcessing;

		public override bool ContainsTweening => this.containsTweening;

		[return: Release]
		public override NSObject Copy ()
		{
			return CustomVideoCompositionInstruction.Copy (this);
		}

		[return: Release]
		public override NSObject MutableCopy ()
		{
			return CustomVideoCompositionInstruction.Copy (this);
		}

		public static CustomVideoCompositionInstruction Copy (CustomVideoCompositionInstruction current)
		{
			CustomVideoCompositionInstruction result = null;
			if (current.RequiredSourceTrackIDs != null && current.RequiredSourceTrackIDs.Length > 0) {
				result = new CustomVideoCompositionInstruction (current.RequiredSourceTrackIDs, current.TimeRange);
			} else {
				result = new CustomVideoCompositionInstruction (current.PassthroughTrackID, current.TimeRange);
			}

			result.ForegroundTrackId = current.ForegroundTrackId;
			result.BackgroundTrackId = current.BackgroundTrackId;

			return result;
		}
	}
}
