using System;

using UIKit;
using Foundation;
using AVFoundation;

namespace AutoWait
{
	public partial class PlaybackDetailsViewController : UIViewController
	{
		// TODOL port
		public AVPlayer Player { get; set; }

		public PlaybackDetailsViewController (IntPtr handle) : base (handle)
		{
		}
	}
}
