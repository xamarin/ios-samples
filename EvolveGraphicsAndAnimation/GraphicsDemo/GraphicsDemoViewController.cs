using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GraphicsDemo
{
	public partial class GraphicsDemoViewController : UIViewController
	{
		DemoView demo;

		public GraphicsDemoViewController ()
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			demo = new DemoView{Frame = UIScreen.MainScreen.Bounds};
			View = demo;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}
	}
}

