using System;

using UIKit;
using AVFoundation;
using ObjCRuntime;
using Foundation;

namespace AudioTapProcessor
{
	[Register("PlayerView")]
	public class PlayerView : UIView
	{
		static Class LayerClass {
			[Export("layerClass")]
			get {
				return new Class (typeof(AVPlayerLayer));
			}
		}

		public AVPlayer Player {
			get {
				return ((AVPlayerLayer)Layer).Player;
			}
			set {
				((AVPlayerLayer)Layer).Player = value;
			}
		}

		public PlayerView (IntPtr handle)
			: base(handle)
		{
		}
	}
}

