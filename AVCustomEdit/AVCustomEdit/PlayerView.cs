using AVFoundation;
using Foundation;
using ObjCRuntime;
using System;
using UIKit;

namespace AVCustomEdit {
	[Register ("PlayerView")]
	public class PlayerView : UIView {
		public PlayerView (IntPtr handle) : base (handle) { }

		public AVPlayer Player {
			get {
				return (this.Layer as AVPlayerLayer).Player;
			}
			set {
				(this.Layer as AVPlayerLayer).Player = value;
			}
		}

		public static Class LayerClass {
			[Export ("layerClass")]
			get {
				return new Class (typeof (AVPlayerLayer));
			}
		}
	}
}
