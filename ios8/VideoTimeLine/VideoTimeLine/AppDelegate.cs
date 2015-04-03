using Foundation;
using UIKit;

namespace VideoTimeLine {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {
		public override UIWindow Window {
			get;
			set;
		}

		// There is no need for a FinishedLaunching method here as the
		// Main.storyboard is automagically loaded since it is specified
		// in the Info.plist -> <key>UIMainStoryboardFile~ipad</key>
	}
}