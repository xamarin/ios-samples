using UIKit;

namespace XMBindingLibrarySample
{
	public class RootViewController : UINavigationController
	{
		UtilitiesViewController utilViewController;

		public override void LoadView()
		{
			base.LoadView();

			utilViewController = new UtilitiesViewController();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			PushViewController(utilViewController, true);
		}
	}
}
