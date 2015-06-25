using UIKit;

namespace CodeOnlyDemo
{
	class CircleController : UIViewController
	{
		CircleView view;

		public override void LoadView()
		{
			view = new CircleView();
			View = view;
		}
	}
}

