using System;

using AVFoundation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace PictureInPicture
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
				return PlayerLayer.Player;
			}
			set {
				PlayerLayer.Player = value;
			}
		}

		public AVPlayerLayer PlayerLayer {
			get {
				return (AVPlayerLayer)Layer;
			}
		}

		[Export ("initWithFrame:")]
		public PlayerView (CGRect frame)
			: base (frame)
		{
		}

		public PlayerView (IntPtr handle)
			: base (handle)
		{
		}
	}
}