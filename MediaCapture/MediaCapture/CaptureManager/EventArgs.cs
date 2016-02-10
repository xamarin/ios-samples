using System;

using UIKit;

namespace MediaCapture {
	public class MovieSegmentRecordingStartedEventArgs : EventArgs {
		public string Path { get; set; }
	}

	public class MovieSegmentRecordingCompleteEventArgs : EventArgs {
		public string Path { get; set; }
		public int Length { get; set; }
		public bool ErrorOccured { get; set; }
	}

	public class MovieSegmentCapturedEventArgs : EventArgs {
		public DateTime StartedAt { get; set; }
		public int DurationMilliSeconds { get; set; }
		public string File { get; set; }
	}

	public class CaptureErrorEventArgs : EventArgs {
		public string ErrorMessage { get; set; }
	}

	public class ImageCaptureEventArgs : EventArgs {
		public UIImage Image { get; set; }
		public DateTime CapturedAt { get; set; }
	}

}

