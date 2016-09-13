using System;

using AVFoundation;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace AutoWait
{
	public partial class PlayerView : UIView
	{
		static Class layerClass;

		public static Class LayerClass {
			[Export ("layerClass")]
			get {
				return layerClass = layerClass ?? new Class (typeof (AVPlayerLayer));
			}
		}

		public AVPlayer Player {
			set {
				((AVPlayerLayer)Layer).Player = value;
			}
			get {
				return ((AVPlayerLayer)Layer).Player;
			}
		}

		public PlayerView (IntPtr handle)
			: base (handle)
		{
		}
	}
}
