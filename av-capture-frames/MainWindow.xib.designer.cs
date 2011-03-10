
namespace avcaptureframes
{
	// Base type probably should be MonoTouch.Foundation.NSObject or subclass
	[MonoTouch.Foundation.Register("AppDelegate")]
	public partial class AppDelegate
	{

		private MonoTouch.UIKit.UIWindow __mt_window;

		[MonoTouch.Foundation.Connect("window")]
		private MonoTouch.UIKit.UIWindow window {
			get {
				this.__mt_window = ((MonoTouch.UIKit.UIWindow)(this.GetNativeField ("window")));
				return this.__mt_window;
			}
			set {
				this.__mt_window = value;
				this.SetNativeField ("window", value);
			}
		}
	}
}

